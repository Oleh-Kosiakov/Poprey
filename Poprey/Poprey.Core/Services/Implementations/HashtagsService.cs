using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Services.Interfaces;

namespace Poprey.Core.Services.Implementations
{
    public class HashtagsService : IHashtagsService
    {
        private IEnumerable<string> _lastLoadedHashtags;
        private readonly IPopreyApiClient _popreyApiClient;

        public HashtagsService(IPopreyApiClient popreyApiClient)
        {
            _popreyApiClient = popreyApiClient;
        }

        public async Task<IEnumerable<string>> LoadSimilarHashtags(string keyword, CancellationToken ct)
        {
            _lastLoadedHashtags = await _popreyApiClient.GetHashtags(keyword, ct);

            return _lastLoadedHashtags;
        }

        public IEnumerable<string> GetLastLoadedHashtags()
        {
            return _lastLoadedHashtags;
        }
    }
}