using System.Collections.Generic;
using System.Threading.Tasks;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Services.Interfaces;

namespace Poprey.Core.Services.Implementations
{
    public class AdditionalServicesService : IAdditionalServicesService
    {
        private readonly IPopreyApiClient _apiClient;

        public AdditionalServicesService(IPopreyApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}