using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Messages;
using Poprey.Core.Models.Bag;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IPopreyApiClient _apiClient;
        private readonly MessageTokenHelper _messenger;

        private IList<TenFreeFollowersInfo> SavedRecords => AppSettings.GetRecordsForModel<IList<TenFreeFollowersInfo>>(AppSettings.Keys.InstagramTenFollowersLastUsedDatesKey)
                                                                     ?? new List<TenFreeFollowersInfo>();


        public OrderService(IPopreyApiClient apiClient, MessageTokenHelper messenger)
        {
            _apiClient = apiClient;
            _messenger = messenger;
        }

        public void AddOrder(OrderInfo orderInfo)
        {
            _messenger.Publish(new AddToBagMessage(this)
            {
                OrderInfo = orderInfo
            });
        }

        public async Task<OrderResponse> CreateOrders(List<Order> orders, CancellationToken ct)
        {
            foreach (var order in orders)
            {
                if (order.IsAdditionalService)
                {
                    await _apiClient.CreateOrderForAdditionalService(order, ct);
                }
                else
                {
                    await _apiClient.CreateOrder(order, ct);
                }
            }

            return await _apiClient.ConfirmOrders(ct);
        }

        public Task GetTenFreeSubscribers(string instagramNickname, CancellationToken ct)
        {
            var order = new Order
            {
                UserNickname = instagramNickname,
                ServiceName = "Followers",
                SystemName = "Instagram",
                TariffType = "normal",
                TariffPlan = 10
            };

            var result = _apiClient.CreateTestOrder(order, ct);

            RefreshDataAboutTenFreeFollowersForAccount(instagramNickname);

            return result;
        }

        public bool TenFreeFollowersAvailableForAccount(string instagramNickname)
        {
            var savedAccountInfo = SavedRecords.FirstOrDefault(sa => sa.InstagramNickname == instagramNickname);

            if (savedAccountInfo == null)
            {
                return true;
            }

            return savedAccountInfo.GetDate() <= DateTime.UtcNow - TimeSpan.FromDays(1);
        }


        private void RefreshDataAboutTenFreeFollowersForAccount(string instagramNickname)
        {
            var savedAccountInfo = SavedRecords.FirstOrDefault(sa => sa.InstagramNickname == instagramNickname);

            if (savedAccountInfo == null)
            {
                var accountInfo = new TenFreeFollowersInfo
                {
                    InstagramNickname = instagramNickname
                };
                accountInfo.SetDate(DateTime.UtcNow);
                SavedRecords.Add(accountInfo);
            }
            else
            {
                savedAccountInfo.SetDate(DateTime.UtcNow);
            }

            AppSettings.SetRecordsForModel(SavedRecords, AppSettings.Keys.InstagramTenFollowersLastUsedDatesKey);
        }
    }
}