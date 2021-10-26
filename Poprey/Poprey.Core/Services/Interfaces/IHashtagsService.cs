using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Poprey.Core.Services.Interfaces
{
    public interface IHashtagsService
    {
        Task<IEnumerable<string>> LoadSimilarHashtags(string keyword, CancellationToken ct);

        IEnumerable<string> GetLastLoadedHashtags();
    }
}