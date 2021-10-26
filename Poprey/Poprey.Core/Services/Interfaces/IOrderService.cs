using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Models.Bag;

namespace Poprey.Core.Services.Interfaces
{
    public interface IOrderService
    {
        void AddOrder(OrderInfo orderInfo);

        Task<OrderResponse> CreateOrders(List<Order> orders, CancellationToken ct);

        Task GetTenFreeSubscribers(string instagramNickname, CancellationToken ct);

        bool TenFreeFollowersAvailableForAccount(string instagramNickname);
    }
}