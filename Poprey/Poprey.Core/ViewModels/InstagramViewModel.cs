using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Resources;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class InstagramViewModel : BaseViewModel<object>
    {
        private readonly IInstagramService _instagramService;

        public InstagramViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
                                    IPopupService popupService, IInstagramService instagramService)
                                   : base(navigationService, analyticsService, messenger, popupService)
        {
            _instagramService = instagramService;

            Messenger.Subscribe<InstagramShowCustomErrorMessage>((m) =>
            {
                ErrorText = m.ErrorMessage;
                IsInErrorState = true;

                RaisePropertyChanged(() => ErrorText);
                RaisePropertyChanged(() => IsInErrorState);
            });

#if DEBUG
            SelectedAccount = "olegenri1996";
#endif
        }

        public override void Prepare()
        {
            base.Prepare();

            if (_instagramService.SavedInstagramAccountsCount >= Constants.MaxSavedAccountsNumber)
            {
                ErrorText = string.Format(Strings.Instagram_AccountLimitReached, Constants.MaxSavedAccountsNumber);
                IsInErrorState = true;
            }
        }

        public bool IsGoButtonVisible => !string.IsNullOrEmpty(SelectedAccount);
        public bool IsInErrorState { get; set; }
        public string ErrorText { get; set; }

        private string _selectedAccount;
        public string SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;

                RaisePropertyChanged(() => IsGoButtonVisible);
                RaisePropertyChanged(() => SelectedAccount);
            }
        }

        private IMvxCommand _searchForAccountCommand;

        public IMvxCommand SearchForAccountCommand => _searchForAccountCommand =
            _searchForAccountCommand ?? new MvxAsyncCommand(
                async () =>
                {
                    InstagramAccount instagramAccount = null;

                    var e = await WebRequest(async () =>
                    {
                        instagramAccount = await _instagramService.AddInstagramAccountAndLoadData(SelectedAccount, true, CtsHelper.CreateCts().Token);
                    });

                    switch (e)
                    {
                        case ServiceResolution.Success:
                            {
                                ErrorText = string.Empty;
                                IsInErrorState = false;
                                Messenger.Publish(new EmptifyBagMessage(this));

                                await NavigationService.Navigate<InstagramMenuViewModel, InstagramAccount>(instagramAccount);
                                break;
                            }
                        case ServiceResolution.NoPostsInTheAccount:
                            {
                                ErrorText = Strings.Instagram_NoPostsError;
                                IsInErrorState = true;
                                break;
                            }
                        case ServiceResolution.InstagramAccountsLimitReached:
                            {
                                ErrorText = string.Format(Strings.Instagram_AccountLimitReached, Constants.MaxSavedAccountsNumber);
                                IsInErrorState = true;
                                break;
                            }
                        case ServiceResolution.AccountClosedOrDoesNotExists:
                            {
                                ErrorText = Strings.Instagram_ClosedOrDoesNotExistError;
                                IsInErrorState = true;
                                break;
                            }
                    }

                    await RaisePropertyChanged(() => ErrorText);
                    await RaisePropertyChanged(() => IsInErrorState);
                });

        #region Labels

        public string HeaderText => Strings.InstagramHeader;
        public string PlaceholderText => Strings.InstagramPlaceholder;

        #endregion
    }
}