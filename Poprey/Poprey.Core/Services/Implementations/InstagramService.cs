using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Messages;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Rest.Models;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.Services.Implementations
{
    public class InstagramService : IInstagramService
    {
        private readonly IPopreyApiClient _popreyApiClient;
        private readonly ITariffsService _tariffsService;
        private readonly MessageTokenHelper _messageHelper;

        private bool _accountsUpToDate;
        public int SavedInstagramAccountsCount => SavedInstagramAccounts.Count;

        private IList<InstagramAccount> _instagramAccounts;
        public IList<InstagramAccount> SavedInstagramAccounts
        {
            get
            {
                if (!_accountsUpToDate)
                {
                    _accountsUpToDate = true;

                    _instagramAccounts =
                        AppSettings.GetRecordsForModel<IList<InstagramAccount>>(AppSettings.Keys.InstagramAccountsKey) ?? new List<InstagramAccount>();
                }

                return _instagramAccounts;
            }
        }

        public InstagramService(IPopreyApiClient popreyApiClient, MessageTokenHelper messageHelper, ITariffsService tariffsService)
        {
            _popreyApiClient = popreyApiClient;
            _messageHelper = messageHelper;
            _tariffsService = tariffsService;
        }

        public async Task<InstagramAccount> AddInstagramAccountAndLoadData(string instagramNickname, bool allowAlreadyAdded, CancellationToken ct)
        {
            if (!allowAlreadyAdded && SavedInstagramAccountsCount >= Constants.MaxSavedAccountsNumber)
            {
                throw new ServiceException(ServiceResolution.InstagramAccountsLimitReached);
            }

            var accountAlreadyAdded = SavedInstagramAccounts.Any(ia => ia.InstagramNickname == instagramNickname);

            if (!allowAlreadyAdded && accountAlreadyAdded)
            { 
                throw new ServiceException(ServiceResolution.SuchAccountAlreadyAdded);
            }

            InstagramAccount instagramAcc = null;

            try
            {
                instagramAcc = await LoadAccountData(instagramNickname, ct);
            }
            catch (ApiException exc)
            {
                _messageHelper.Publish(new InstagramShowCustomErrorMessage(this, exc.ErrorText));
                throw new ServiceException(ServiceResolution.ShowCustomErrorMessage);
            }


            if (!accountAlreadyAdded)
            {
                SavedInstagramAccounts.Add(instagramAcc);
                AppSettings.SetRecordsForModel(SavedInstagramAccounts, AppSettings.Keys.InstagramAccountsKey);
                _accountsUpToDate = false;

                _messageHelper.Publish(new AccountsListChangedMessage(this));
            }

            return instagramAcc;
        }

        public async Task<InstagramAccount> LoadAccountData(string instagramNickname, CancellationToken ct)
        {
            var tariffServices = await _tariffsService.GetTariffs(instagramNickname,  ct);
            var firstService= tariffServices.First().TariffServices.First();
            var plan = firstService.TariffTypes.First().TariffItems.First().Name;

            var instagramAcc = await _popreyApiClient.GetInstagramPosts(instagramNickname, firstService.Name, plan, ct);
            instagramAcc.InstagramNickname = instagramNickname;

            return instagramAcc;
        }

        public void RemoveInstagramAccount(string instagramNickName)
        {
            var accountToRemove = SavedInstagramAccounts.SingleOrDefault(ia => ia.InstagramNickname == instagramNickName);

            if (accountToRemove == null)
                return;

            SavedInstagramAccounts.Remove(accountToRemove);
            AppSettings.SetRecordsForModel(SavedInstagramAccounts, AppSettings.Keys.InstagramAccountsKey);
            _accountsUpToDate = false;

            _messageHelper.Publish(new AccountsListChangedMessage(this));
        }
    }
}
