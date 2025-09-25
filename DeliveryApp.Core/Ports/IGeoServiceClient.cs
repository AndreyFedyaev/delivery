using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IGeoServiceClient
    {
        /// <summary>
        ///     Получить информацию о локации
        /// </summary>
        Task<Result<Location, Error>> GetGeolocationAsync(string street, CancellationToken cancellationToken);
    }
}
