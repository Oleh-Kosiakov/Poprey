using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Models.Instagram.Tariffs;
using Poprey.Core.Rest.Interfaces;

namespace Poprey.Core.Rest.Implementations
{
    public class PopreyApiClient : IPopreyApiClient
    {
        private readonly IRestClient _restClient;

        public PopreyApiClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public Task<IEnumerable<string>> GetHashtags(string keyword, CancellationToken ct)
        {
            var parameters = new
            {
                Keyword = keyword
            };

            return _restClient.MakeApiCall<IEnumerable<string>>(ct, "api/search_hashtags.php", HttpMethod.Post, parameters);
        }

        public Task<TariffsResponse> GetTariffs(string instagramNickName, CancellationToken ct)
        {
            var parameters = new
            {
                username = instagramNickName
            };

            return _restClient.MakeApiCall<TariffsResponse>(ct, "api/get_tariffs.php", HttpMethod.Post, parameters);
        }

        public Task<AddsTariffsResponse> GetTariffsForAdditionalServices(CancellationToken ct)
        {
            return _restClient.MakeApiCall<AddsTariffsResponse>(ct, "api/additional_services.php", HttpMethod.Post);
        }

        public Task<OrderResponse> CreateOrder(Order order, CancellationToken ct)
        {
            var adjustedTariffPlan = AddSpaceToNumberForTariffNames(order.TariffPlan);
            var adjustedComments = PrepareCommentsList(order.CommentsList);

            object parameters;

            if (order.Impressions.HasValue)
            {
                var adjustedImpressions = AddSpaceToNumberForTariffNames(order.Impressions.Value);

                parameters = new
                {
                    Cart = 1,
                    Email = order.UserEmail,
                    System = order.SystemName,
                    Service = order.ServiceName,
                    Tariff = order.TariffType,
                    Plan = adjustedTariffPlan,
                    Username = order.UserNickname,
                    Posts = order.Posts ?? new List<InstagramPost>(),
                    Impressions = adjustedImpressions,
                    Comment_List = adjustedComments
                };

            }
            else
            {
                parameters = new
                {
                    Cart = 1,
                    Email = order.UserEmail,
                    System = order.SystemName,
                    Service = order.ServiceName,
                    Tariff = order.TariffType,
                    Plan = adjustedTariffPlan,
                    Username = order.UserNickname,
                    Posts = order.Posts ?? new List<InstagramPost>(),
                    Comment_List = adjustedComments
                };
            }

            return _restClient.MakeApiCall<OrderResponse>(ct, "api/create_order.php", HttpMethod.Post, parameters);
        }

        public Task<OrderResponse> CreateOrderForAdditionalService(Order order, CancellationToken ct)
        {
            var adjustedTariffPlan = AddSpaceToNumberForTariffNames(order.TariffPlan);

            var parameters = new
            {
                Cart = 1,
                Email = order.UserEmail,
                Url = order.ResourceUrl,
                System = order.SystemName,
                Service = order.ServiceName,
                Plan = adjustedTariffPlan,
            };

            return _restClient.MakeApiCall<OrderResponse>(ct, "api/additional_create_order.php", HttpMethod.Post, parameters);
        }


        public Task<OrderResponse> ConfirmOrders(CancellationToken ct)
        {
            return _restClient.MakeApiCall<OrderResponse>(ct, "api/cart_pay.php", HttpMethod.Post);
        }

        public Task<InstagramAccount> GetInstagramPosts(string nickname, string service, int plan, CancellationToken ct)
        {
            var parameters = new
            {
                System = "Instagram",
                Service = service,
                Plan = plan,
                UserName = nickname
            };

            return _restClient.MakeApiCall<InstagramAccount>(ct, "api/get_posts.php", HttpMethod.Post, parameters);
        }

        public Task CreateTestOrder(Order order, CancellationToken ct)
        {
            var parameters = new
            {
                Username = order.UserNickname,
                System = order.SystemName,
                Service = order.ServiceName,
                Tariff = order.TariffType,
                Plan = order.TariffPlan
            };

            return _restClient.MakeApiCall<object>(ct, "api/create_test_order.php", HttpMethod.Post, parameters);
        }

        private string PrepareCommentsList(List<string> orderCommentsList)
        {
            var builder = new StringBuilder();

            foreach (var comment in orderCommentsList)
            {
                builder.AppendLine(comment);
            }

            return builder.ToString();
        }

        private string AddSpaceToNumberForTariffNames(int value)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = "+";

            return $"{value.ToString("#,0", nfi)}";
        }
    }
}