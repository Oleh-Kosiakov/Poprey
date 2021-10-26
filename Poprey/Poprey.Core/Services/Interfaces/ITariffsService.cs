using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Models.Instagram.Tariffs;

namespace Poprey.Core.Services.Interfaces
{
    public interface ITariffsService
    {
        Task<List<Models.Instagram.Tariffs.TariffSystem>> GetTariffs(string instagramNickName, CancellationToken ct);

        Task<List<Models.AdditionalServices.Tariffs.TariffSystem>> GetTariffsForAdditionalServices(CancellationToken ct);

        List<Models.Instagram.Tariffs.TariffSystem> GetCachedTariffs();

        List<Models.AdditionalServices.Tariffs.TariffSystem> GetCachedTariffsForAdditionalServices();

        bool IsTariffServiceAvailable(string tariffServiceName);

        ExtraItem GetInstagramExtraImpressionsForTariff(TariffItem tariffItem, string serviceName);
    }
}