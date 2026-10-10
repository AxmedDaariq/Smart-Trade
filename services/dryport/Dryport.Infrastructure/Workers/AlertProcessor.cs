using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;
using Dryport.Domain.Entities;
using Dryport.Application.Services;
using Dryport.Infrastructure.Persistence;

namespace Dryport.Infrastructure.Messaging
{
    // 1. Qaybta Fariimaha loo dirayo RabbitMQ (RabbitMQ Publisher)
    public interface IAlertMessagePublisher
    {
        void PublishAlertEvent(DemurrageAlert alert);
    }

    public class RabbitMqAlertPublisher : IAlertMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqAlertPublisher(string hostName = "localhost")
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Abuurista Exchange si fariintu ugu dhacdo nidaamyada ogeysiiska (Notification Service)
            _channel.ExchangeDeclare(exchange: "dryport_alerts_exchange", type: ExchangeType.Topic);
        }

        public void PublishAlertEvent(DemurrageAlert alert)
        {
            var routingKey = alert.Level == AlertLevel.Critical ? "alert.demurrage.critical" : "alert.demurrage.warning";
            var message = JsonSerializer.Serialize(alert);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "dryport_alerts_exchange",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}

namespace Dryport.Infrastructure.Repositories
{
    // 2. Qaybta Xogta lagu kaydinayo MongoDB (MongoDB Repository)
    public interface IMongoAlertRepository
    {
        Task SaveAlertAsync(DemurrageAlert alert);
    }

    public class MongoDbAlertRepository : IMongoAlertRepository
    {
        private readonly IMongoCollection<DemurrageAlert> _alertsCollection;

        public MongoDbAlertRepository(IMongoDatabase mongoDatabase)
        {
            // Abuurista ururinta (Collection) ee MongoDB
            _alertsCollection = mongoDatabase.GetCollection<DemurrageAlert>("DemurrageAlerts");
        }

        public async Task SaveAlertAsync(DemurrageAlert alert)
        {
            await _alertsCollection.InsertOneAsync(alert);
        }
    }
}

namespace Dryport.Infrastructure.Workers
{
    using Dryport.Infrastructure.Messaging;
    using Dryport.Infrastructure.Repositories;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    // 3. Qaybta Isku xiraysa Postgres, MongoDB iyo RabbitMQ (Background Worker)
    public class DemurrageAlertWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DemurrageAlertWorker> _logger;

        public DemurrageAlertWorker(IServiceProvider serviceProvider, ILogger<DemurrageAlertWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Demurrage Alert Worker waa uu bilowday...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // 1. PostgreSQL - Kasoo qaado kunteenarada
                    var postgresDb = scope.ServiceProvider.GetRequiredService<IDryportDbContext>();
                    
                    // 2. MongoDB - Kaydi digniinaha
                    var mongoRepo = scope.ServiceProvider.GetRequiredService<IMongoAlertRepository>();
                    
                    // 3. RabbitMQ - U dir fariimaha
                    var rabbitMq = scope.ServiceProvider.GetRequiredService<IAlertMessagePublisher>();
                    var demurrageService = scope.ServiceProvider.GetRequiredService<IDemurrageService>();
                    
                    var currentDate = DateTime.UtcNow;

                    var activeContainers = postgresDb.Containers
                        .Where(c => c.Status == ContainerStatus.Arrived || c.Status == ContainerStatus.InStorage)
                        .ToList();

                    foreach (var container in activeContainers)
                    {
                        int daysInPort = (currentDate - container.ArrivalDate).Days;
                        int freeDays = container.DemurrageRule?.FreeDays ?? 0;
                        int daysRemaining = freeDays - daysInPort;

                        DemurrageAlert? newAlert = null;

                        if (daysRemaining > 0 && daysRemaining <= 2)
                        {
                            newAlert = new DemurrageAlert
                            {
                                Id = Guid.NewGuid(),
                                ContainerRecordId = container.Id,
                                AlertDate = currentDate,
                                Level = AlertLevel.Warning,
                                Message = $"Kunteenarka {container.ContainerNumber} ayaa qarka u saaran Demurrage."
                            };
                        }
                        else if (daysRemaining <= 0)
                        {
                            newAlert = new DemurrageAlert
                            {
                                Id = Guid.NewGuid(),
                                ContainerRecordId = container.Id,
                                AlertDate = currentDate,
                                Level = AlertLevel.Critical,
                                Message = $"Kunteenarka {container.ContainerNumber} Demurrage ayaa billowday!"
                            };
                        }

                        if (newAlert != null)
                        {
                            // A. Ku xafid MongoDB (Digniinta)
                            await mongoRepo.SaveAlertAsync(newAlert);

                            // B. Ku dhex dir RabbitMQ (Si Message Service-ku SMS/Email ugu diro)
                            rabbitMq.PublishAlertEvent(newAlert);
                            
                            _logger.LogInformation("Alert loo diray RabbitMQ lana kaydiyay MongoDB: {Container}", container.ContainerNumber);
                        }
                    }
                }

                // Seexo 24 saac (Worker-ku habeenkii buu shaqaynayaa)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
