using System.Linq;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.UI;
using MvvmCross.ViewModels;
using Plugin.Connectivity;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Resources;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;
using Timer = Poprey.Core.Util.Timer;

namespace Poprey.Core.ViewModels
{
    public class HeaderViewModelSingleton : MvxViewModel
    {
        private Timer _timer;
        private readonly MessageTokenHelper _messenger;
        private readonly IPopupService _popupService;
        private readonly IInstagramService _instagramService;

        public MvxColor PopupBackgroundColor { get; set; }
        public bool IsErrorImage { get; set; }
        public string PopupText { get; set; }
        public int InstagramAccountsCount => InstagramAccounts.Count;
        public MvxObservableCollection<HeaderInstagramAccount> InstagramAccounts { get; } = new MvxObservableCollection<HeaderInstagramAccount>();

        private bool _isPopupVisible;
        public bool IsPopupVisible
        {
            get => _isPopupVisible;
            set
            {
                _isPopupVisible = value;
                RaisePropertyChanged(() => IsPopupVisible);
            }
        }

        private bool _isConnected = CrossConnectivity.Current.IsConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnConnectionChanged();
                RaisePropertyChanged(() => IsConnected);
            }
        }

        private bool _isDrawerMenuExpanded;
        public bool IsDrawerMenuExpanded
        {
            get => _isDrawerMenuExpanded;
            set
            {
                if (_isDrawerMenuExpanded == value)
                    return;

                _isDrawerMenuExpanded = value;

                RaisePropertyChanged(() => IsDrawerMenuExpanded);
            }
        }

        private bool _isCollapsed;
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set
            {
                _isCollapsed = value;

                RaisePropertyChanged(() => IsCollapsed);
            }
        }

        private bool _selectedInstagramPage = true;
        public bool SelectedInstagramPage
        {
            get => _selectedInstagramPage;
            set
            {
                if (_selectedInstagramPage == value)
                    return;

                _selectedInstagramPage = value;

                if (_selectedInstagramPage)
                {
                    _messenger.Publish(new SwitchPageMessage(this, typeof(InstagramViewModel)));
                }

                RaisePropertyChanged(() => SelectedInstagramPage);
            }
        }

        private bool _selectedTikTokPage;
        public bool SelectedTikTokPage
        {
            get => _selectedTikTokPage;
            set
            {
                if (_selectedTikTokPage == value)
                    return;

                _selectedTikTokPage = value;

                if (_selectedTikTokPage)
                {
                    _messenger.Publish(new SwitchPageMessage(this, typeof(AdditionalServicesViewModel)));
                }

                RaisePropertyChanged(() => SelectedTikTokPage);
            }
        }

        private bool _selectedHashtagPage;

        public bool SelectedHashtagPage
        {
            get => _selectedHashtagPage;
            set
            {
                if (_selectedHashtagPage == value)
                    return;

                _selectedHashtagPage = value;

                if (_selectedHashtagPage)
                {
                    _messenger.Publish(new SwitchPageMessage(this, typeof(HashtagsViewModel)));
                }

                RaisePropertyChanged(() => SelectedHashtagPage);
            }
        }

        private int _backgroundOverlayDesiredInNativeUnits;

        public int BackgroundOverlayWidthInNativeUnits
        {
            get => _backgroundOverlayDesiredInNativeUnits;
            set
            {
                if (_backgroundOverlayDesiredInNativeUnits == value)
                    return;

                _backgroundOverlayDesiredInNativeUnits = value;

                RaisePropertyChanged(() => BackgroundOverlayWidthInNativeUnits);
            }
        }

        public HeaderViewModelSingleton(MessageTokenHelper messenger, IPopupService popupService, IInstagramService instagramService)
        {
            _messenger = messenger;
            _popupService = popupService;
            _instagramService = instagramService;

            _messenger.Subscribe<AccountsListChangedMessage>(OnAccountsChanged);

            RefreshAccounts();
        }

        public void ExplicitlyShowError(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                PopupText = text;
            }
            PopupBackgroundColor = AppColors.InternetDisconnected;
            IsErrorImage = true;

            ShowPopupWithDelay();
        }

        private void OnConnectionChanged()
        {
            PopupBackgroundColor = IsConnected ? AppColors.InternetConnected : AppColors.InternetDisconnected;
            PopupText = IsConnected ? Strings.InternetConnected : Strings.InternetDisconnected;
            IsErrorImage = !IsConnected;

            ShowPopupWithDelay();
        }

        private void ShowPopupWithDelay()
        {
            RaisePropertyChanged(() => PopupBackgroundColor);
            RaisePropertyChanged(() => IsErrorImage);
            RaisePropertyChanged(() => PopupText);

            IsPopupVisible = true;

            if (_timer != null && !_timer.IsCancellationRequested)
            {
                _timer.Cancel();
                _timer = null;
            }

            _timer = new Timer(HandleTimerCallback, Constants.PopupMilliseconds);
        }

        private void HandleTimerCallback()
        {
            IsPopupVisible = false;
            RaisePropertyChanged(() => IsPopupVisible);
        }

        private void OnAccountsChanged(AccountsListChangedMessage accountsListChangedMessage)
        {
            RefreshAccounts();
        }

        private void RefreshAccounts()
        {
            var displayModels = _instagramService.SavedInstagramAccounts.Select(ia => new HeaderInstagramAccount
            {
                ShouldLoadImageFromWeb = true,
                ShouldShowInstagramIcon = true,
                AvatarImageUrl = ia.InstagramAvatarUri
            }).ToList();

            if (displayModels.Count < Constants.MaxSavedAccountsNumber)
            {
                displayModels.Insert(0, new HeaderInstagramAccount
                {
                    ShouldShowGrayBackground = true,
                    ShouldShowAddCross = true,
                    ShouldShowInstagramIcon = false
                });
            }

            InstagramAccounts.SwitchTo(displayModels);
            RaisePropertyChanged(() => InstagramAccountsCount);
        }

        #region Commands

        private IMvxCommand _toggleDrawerCommand;
        public IMvxCommand ToggleDrawerCommand => _toggleDrawerCommand = _toggleDrawerCommand ?? new MvxCommand(
                                                                        () =>
                                                                        {
                                                                            IsDrawerMenuExpanded = !IsDrawerMenuExpanded;
                                                                        });

        private IMvxCommand _showInstagramCommand;
        public IMvxCommand ShowInstagramCommand => _showInstagramCommand = _showInstagramCommand ?? new MvxCommand(
                                                                        () =>
                                                                        {
                                                                            SelectedInstagramPage = true;
                                                                            SelectedTikTokPage = false;
                                                                            SelectedHashtagPage = false;
                                                                        });

        private IMvxCommand _showTikTokCommand;
        public IMvxCommand ShowTikTokCommand => _showTikTokCommand = _showTikTokCommand ?? new MvxCommand(
                                                                         () =>
                                                                         {
                                                                             SelectedInstagramPage = false;
                                                                             SelectedTikTokPage = true;
                                                                             SelectedHashtagPage = false;
                                                                         });

        private IMvxCommand _showHastagCommand;
        public IMvxCommand ShowHastagCommand => _showHastagCommand = _showHastagCommand ?? new MvxCommand(
                                                                         () =>
                                                                         {
                                                                             SelectedInstagramPage = false;
                                                                             SelectedTikTokPage = false;
                                                                             SelectedHashtagPage = true;
                                                                         });

        private IMvxCommand _showAuthenticationCommand;
        public IMvxCommand ShowAuthenticationCommand => _showAuthenticationCommand = _showAuthenticationCommand ?? new MvxCommand(
                                                                        () =>
                                                                        {
                                                                            _popupService.ShowAuthenticationDialog(Mvx.IoCProvider.Resolve<AuthenticationDialogViewModel>());
                                                                        });

        #endregion
    }
}