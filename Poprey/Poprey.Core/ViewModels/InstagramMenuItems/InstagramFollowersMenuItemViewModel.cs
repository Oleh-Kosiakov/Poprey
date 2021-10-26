using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Models.Instagram.Tariffs;
using Poprey.Core.Resources;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels.InstagramMenuItems
{
    public class InstagramFollowersMenuItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private const string ServiceKey = Constants.InstagramFollowersServiceKey;
        private const string SystemKey = Constants.InstagramSystemKey;

        private readonly IOrderService _orderService;
        private readonly TariffService _currentService;

        private InstagramAccount _instagramAccount;
        public InstagramAccount InstagramAccount
        {
            get => _instagramAccount;
            set
            {
                _instagramAccount = value;

                TenFreeFollowersAvailable = _orderService.TenFreeFollowersAvailableForAccount(_instagramAccount.InstagramNickname);
            }
        }

        public string SelectedTariffType => IsInPremiumMode ? Constants.InstagramFollowersPremium : Constants.InstagramFollowersNormal;

        private IEnumerable<TariffItem> TariffItems => _currentService
                                                       .TariffTypes
                                                       .First(t => t.Name == SelectedTariffType)
                                                       .TariffItems
                                                       .Where(ti => ti.Price != default);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

        private int _followersCounter;
        public int FollowersCounter
        {
            get => _followersCounter;
            set
            {
                if (_followersCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _followersCounter = value;

                RaisePropertyChanged(() => FollowersCounter);
            }
        }

        private int _discountPercent;
        public int DiscountPercent
        {
            get => _discountPercent;
            set
            {
                _discountPercent = value;

                RaisePropertyChanged(() => DiscountPercent);
            }
        }

        private int _incrementCount;
        public int IncrementCount
        {
            get => _incrementCount;
            set
            {
                _incrementCount = value;

                RaisePropertyChanged(() => IncrementCount);
            }
        }

        private int _followersLimit;
        public int FollowersLimit
        {
            get => _followersLimit;
            set
            {
                _followersLimit = value;

                RaisePropertyChanged(() => FollowersLimit);
            }
        }

        private bool _isInPremiumMode;
        public bool IsInPremiumMode
        {
            get => _isInPremiumMode;
            set
            {
                _isInPremiumMode = value;

                AdjustNumberOfDesiredFollowers();

                RaisePropertyChanged(() => IsInPremiumMode);
            }
        }

        private bool _tenFreeFollowersAvailable = true;
        public bool TenFreeFollowersAvailable
        {
            get => _tenFreeFollowersAvailable;
            set
            {
                _tenFreeFollowersAvailable = value;

                RaisePropertyChanged(() => TenFreeFollowersAvailable);
            }
        }

        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                RaisePropertyChanged(() => IncrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                RaisePropertyChanged(() => DecrementButtonActive);
            }
        }

        public bool Initialized { get; set; }

        public InstagramFollowersMenuItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
                                                     IPopupService popupService, IOrderService orderService, ITariffsService tariffsService, IWebBrowserService webBrowserService)
                                                  : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffs()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            if (!NormalAvailable)
            {
                IsInPremiumMode = true;
            }

            _followersLimit = TariffItems.Max(t => t.Name);
            _followersCounter = TariffItems.First().Name;

            ValidateButtonsState();
            ValidateDiscount();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredFollowers()
        {
            FollowersCounter = TariffItemsValues.NearestTo(_followersCounter);
        }

        private void CollectOrderDataAndAddToBag(AddToBagButtonPressedMessage message)
        {
            if (!HeaderItem.IsActive)
                return;

            var plan = TariffItems.First(ti => ti.Name == FollowersCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffType = SelectedTariffType,
                    TariffPlan = FollowersCounter,
                    Price = plan.Price,
                    Discount = plan.Discount
                },
                IsInBagAlternativeMode = IsInPremiumMode,
                IsInBagCompactMode = true,
                ServicePseudonym = HeaderItem.Title,
                DiscountPercent = DiscountPercent,
                NormalModeBagItemLabelText = Strings.Instagram_Normal,
                AlternativeModeBagItemLabelText = Strings.Instagram_Premium
            };

            _orderService.AddOrder(orderInfo);
        }

        private async Task ProceedWithEmailForTestOrder()
        {
            var e = await WebRequest(async () =>
            {
                await _orderService.GetTenFreeSubscribers(InstagramAccount.InstagramNickname, CtsHelper.CreateCts().Token);
            });

            switch (e)
            {
                case ServiceResolution.Success:
                    var thankYouArgument = new ThanYouPageArguments
                    {
                        HeaderText = "Thanks for a shot",
                        SubheaderText = "Soon you will get your 10 free Followers, come back tomorrow to get new one.\r\nEnjoy it!",
                        PriceVisible = false
                    };

                    TenFreeFollowersAvailable = false;
                    await NavigationService.Navigate<ThankYouViewModel, ThanYouPageArguments>(thankYouArgument);

                    break;
                case ServiceResolution.EmailIsIncorrect:
                    PopupService.Alert($"Please enter correct email.");
                    break;
                case ServiceResolution.UserIsBanned:
                    PopupService.Alert($"User {InstagramAccount.InstagramNickname} was banned.");
                    break;
                case ServiceResolution.TestWasUsedDuringLast24Hours:
                    PopupService.Alert("You have already used this during last 24 hours.");
                    TenFreeFollowersAvailable = false;
                    break;
                case ServiceResolution.TestOrdersOverallLimitReached:
                    PopupService.Alert("System is overloaded now. Please try again later.");
                    break;
                default:
                    HeaderViewModel.ExplicitlyShowError("Unknown error occured");
                    break;
            }
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_followersCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _followersCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_followersCounter);
        }

        private void ValidateDiscount()
        {
            var tariffItem = TariffItems.First(ti => ti.Name == _followersCounter);
            DiscountPercent = tariffItem.DiscountPercent;
        }

        private void DecrementAndValidateButtonsState()
        {
            if(!DecrementButtonActive)
                return;

            var oldValue = _followersCounter;

            //Adjusting value
            var tariffItemsValues =TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _followersCounter, skipping it and taking next
            FollowersCounter = tariffItemsValues.GetNextTo(_followersCounter);

            //Validating availability
            IncrementCount = oldValue - _followersCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_followersCounter);

            ValidateDiscount();
        }

        private void IncrementAndValidateButtonsState()
        {
            if (!IncrementButtonActive)
                return;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _followersCounter, skipping it and taking next
            FollowersCounter = tariffItemsValues.GetNextTo(_followersCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_followersCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _followersCounter;
                IncrementButtonActive = true;
            }
            else
            {
                IncrementCount = 0;
                IncrementButtonActive = false;
            }

            DecrementButtonActive = true;

            ValidateDiscount();
        }

        private bool PremiumAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramFollowersPremium).TariffItems.Any(ti => ti.Price != default);
            }
        }

        private bool NormalAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramFollowersNormal).TariffItems.Any(ti => ti.Price != default);
            }
        }

        #region Labels

        public MenuHeaderItem HeaderItem { get; } = new MenuHeaderItem
        {
            Title = Strings.Instagram_Followers
        };

        public string GetTenFreeLabelText => Strings.Instagram_GetTenFree;
        public string NormalLabelText => Strings.Instagram_Normal;
        public string PremiumLabelText => Strings.Instagram_Premium;

        #endregion

        #region Commands

        private IMvxCommand _normalModeSelectedCommand;
        public IMvxCommand NormalModeSelectedCommand => _normalModeSelectedCommand = _normalModeSelectedCommand ?? new MvxCommand(
                                                                                         () =>
                                                                                         {
                                                                                             if (NormalAvailable)
                                                                                             {
                                                                                                 IsInPremiumMode = false;

                                                                                                 AdjustNumberOfDesiredFollowers();
                                                                                             }
                                                                                         });

        private IMvxCommand _premiumModeSelectedCommand;
        public IMvxCommand PremiumModeSelectedCommand => _premiumModeSelectedCommand = _premiumModeSelectedCommand ?? new MvxCommand(
                                                                                         () =>
                                                                                         {
                                                                                             if (PremiumAvailable)
                                                                                             {
                                                                                                 IsInPremiumMode = true;

                                                                                                 AdjustNumberOfDesiredFollowers();
                                                                                             }
                                                                                         });

        private IMvxCommand _getTenFreeCommand;
        public IMvxCommand GetTenFreeCommand => _getTenFreeCommand = _getTenFreeCommand ?? new MvxAsyncCommand(async () =>
                                                                         {
                                                                            await ProceedWithEmailForTestOrder();
                                                                         }, () => TenFreeFollowersAvailable);

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState);
       

        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState);

       

        #endregion
    }
}