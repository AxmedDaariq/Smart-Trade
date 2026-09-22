using System.Threading.Tasks;

namespace Declaration.Domain.Services;

public interface IHSCodeReferenceProvider
{
    Task<bool> IsValidAsync(string hsCode);
    Task<string> GetDescriptionAsync(string hsCode);
}
