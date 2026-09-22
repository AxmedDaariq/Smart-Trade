using System;
using System.Threading.Tasks;
using Declaration.Domain.Entities;

namespace Declaration.Domain.Services;

public interface IDeclarationNumberGenerator
{
    Task<string> GenerateAsync(CustomsDeclaration declaration);
}
