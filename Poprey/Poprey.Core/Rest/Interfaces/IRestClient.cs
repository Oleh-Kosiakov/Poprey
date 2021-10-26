using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Poprey.Core.Rest.Interfaces
{
    public interface IRestClient
    {
        Task<TResult> MakeApiCall<TResult>(CancellationToken ct, string url, HttpMethod method, object data = null)
                        where TResult : class;

        CookieContainer GetCookieContainer();
    }
}
