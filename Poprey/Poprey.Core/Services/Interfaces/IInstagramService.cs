using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Models.Instagram;

namespace Poprey.Core.Services.Interfaces
{
    public interface IInstagramService
    {
        IList<InstagramAccount> SavedInstagramAccounts { get; }

        int SavedInstagramAccountsCount { get; }

        Task<InstagramAccount> AddInstagramAccountAndLoadData(string nickName, bool allowAlreadyAdded, CancellationToken ct);

        Task<InstagramAccount> LoadAccountData(string instagramNickname, CancellationToken ct);

        void RemoveInstagramAccount(string instagramNickName);
    }
}
