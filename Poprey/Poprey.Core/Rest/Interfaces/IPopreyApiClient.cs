using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Models.Instagram.Tariffs;

namespace Poprey.Core.Rest.Interfaces
{
    public interface IPopreyApiClient
    {
        Task<IEnumerable<string>> GetHashtags(string keyword, CancellationToken ct);

        Task<TariffsResponse> GetTariffs(string instagramNickname, CancellationToken ct);

        Task<AddsTariffsResponse> GetTariffsForAdditionalServices(CancellationToken ct);

        Task<OrderResponse> CreateOrder(Order order, CancellationToken ct);

        Task<OrderResponse> CreateOrderForAdditionalService(Order order, CancellationToken ct);

        Task<OrderResponse> ConfirmOrders(CancellationToken ct);

        Task<InstagramAccount> GetInstagramPosts(string nickname, string service, int plan, CancellationToken ct);

        Task CreateTestOrder(Order order, CancellationToken ct);
    }
}