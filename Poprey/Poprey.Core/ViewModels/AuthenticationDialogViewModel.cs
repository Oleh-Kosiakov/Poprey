using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Resources;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class AuthenticationDialogViewModel : BaseViewModel<object>
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IInstagramService _instagramService;

        public MvxObservableCollection<AddInstagramAccount> Accounts { get; private set; } = new MvxObservableCollection<AddInstagramAccount>();

        private string _newAccountName;
        public string NewAccountName
        {
            get => _newAccountName;
            set
            {
                _newAccountName = value;

                RaisePropertyChanged(() => NewAccountName);
                RaisePropertyChanged(() => IsArrowVisible);
            }
        }

        private bool _dismissRequested;
        public bool DismissRequested
        {
            get => _dismissRequested;
            set
            {
                _dismissRequested = value;

                RaisePropertyChanged(() => DismissRequested);
            }
        }

        public bool IsArrowVisible => !string.IsNullOrEmpty(NewAccountName);

        public AuthenticationDialogViewModel(IMvxNavigationService navigationService, MessageTokenHelper tokenHelper, IPopupService popupService,
                                             IAnalyticsService analyticsService, IInstagramService instagramService) : base(navigationService, analyticsService, tokenHelper, popupService)
        {
            _analyticsService = analyticsService;
            _instagramService = instagramService;

            ReloadAccounts();
        }

        private void RemoveAccount(AddInstagramAccount account)
        {
            _instagramService.RemoveInstagramAccount(account.AccountName);
            Accounts.Remove(Accounts.Single(ia => ia.AccountName == account.AccountName));

            InvalidateSeparators();
        }

        private async Task SwitchAccount(AddInstagramAccount obj)
        {
            InstagramAccount instagramAccount = null;

            var e = await WebRequest(async () =>
            {
                instagramAccount = await _instagramService.AddInstagramAccountAndLoadData(obj.AccountName, true, CtsHelper.CreateCts().Token);
            });

            switch (e)
            {
                case ServiceResolution.Success:
                    {
                        DismissRequested = true;
                        await NavigationService.Navigate<InstagramMenuViewModel, InstagramAccount>(instagramAccount);
                        break;
                    }
                default:
                    {
                        PopupService.Alert(Strings.FailedToAddAccount);
                        break;
                    }
            }
        }

        private void ReloadAccounts()
        {
            var collectionOfDisplayModels = _instagramService.SavedInstagramAccounts.Select(CreateDisplayModel).ToList();

            Accounts.SwitchTo(collectionOfDisplayModels);

            InvalidateSeparators();
        }

        private void InvalidateSeparators()
        {
            var lastAccount = Accounts.LastOrDefault();

            if (lastAccount == null)
                return;

            foreach (var account in Accounts.Except(new List<AddInstagramAccount> { lastAccount }))
            {
                account.ShowSeparator = true;
            }

            lastAccount.ShowSeparator = false;
        }

        private AddInstagramAccount CreateDisplayModel(InstagramAccount instagramAccount)
        {
            var newDisplayModel = new AddInstagramAccount
            {
                AccountName = instagramAccount.InstagramNickname,
                ImageUrl = instagramAccount.InstagramAvatarUri,
                OnRemoveCommand = RemoveAccount,
                OnSwitchCommand = SwitchAccount
            };
            return newDisplayModel;
        }

       
        private IMvxCommand _addNewCommand;

        public IMvxCommand AddNewAccountCommand => _addNewCommand = _addNewCommand ?? new MvxAsyncCommand(async () =>
        {
            InstagramAccount addedInstagramAccount = null;

            var e = await WebRequest(async () =>
            {
                addedInstagramAccount = await _instagramService.AddInstagramAccountAndLoadData(NewAccountName, false, CtsHelper.CreateCts().Token);
            });

            switch (e)
            {
                case ServiceResolution.Success:
                    {
                        var displayModel = CreateDisplayModel(addedInstagramAccount);

                        Accounts.Add(displayModel);
                        InvalidateSeparators();

                        NewAccountName = string.Empty;
                        await RaisePropertyChanged(() => NewAccountName);
                        break;
                    }
                case ServiceResolution.SuchAccountAlreadyAdded:
                    {
                        PopupService.Alert(Strings.Instagram_AccountAlreadySaved);
                        break;
                    }
                case ServiceResolution.InstagramAccountsLimitReached:
                    {
                        PopupService.Alert(string.Format(Strings.Instagram_AccountLimitReached, Constants.MaxSavedAccountsNumber));
                        break;
                    }
                default:
                    {
                        PopupService.Alert(Strings.FailedToAddAccount);
                        break;
                    }
            }
        });

        private IMvxCommand _dismissCommand;
        public IMvxCommand DismissCommand => _dismissCommand = _dismissCommand ?? new MvxCommand(() =>
       {
           DismissRequested = true;
       });

        public string NewAccountHint => "add account";

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var account in Accounts)
                {
                    account.OnRemoveCommand = null;
                    account.OnSwitchCommand = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}