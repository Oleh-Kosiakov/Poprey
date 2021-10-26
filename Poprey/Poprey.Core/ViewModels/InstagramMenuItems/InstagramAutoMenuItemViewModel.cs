using System.Collections.Generic;
using System.Linq;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Models.Instagram.Tariffs;
using Poprey.Core.Resources;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels.InstagramMenuItems
{
    public class InstagramAutoMenuItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private InstagramAccount _instagramAccount;

        private const string SelectedTariffType = Constants.InstagramAutoLikesGradual;
        private const string SystemKey = Constants.InstagramSystemKey;

        private readonly IOrderService _orderService;
        private readonly ITariffsService _tariffsService;

        public string ServiceKey => IsInSubscriptionMode ? Constants.InstagramAutoLikesSubscriptionServiceKey : Constants.InstagramAutoLikesServiceKey;

        private IEnumerable<TariffItem> TariffItems => _currentService
                                                        .TariffTypes
                                                        .First(t => t.Name == SelectedTariffType)
                                                        .TariffItems
                                                        .Where(ti => ti.Price != default);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

        public InstagramAccount InstagramAccount
        {
            get => _instagramAccount;
            set
            {
                _instagramAccount = value;

                PrepareForAccountData();
            }
        }

        private void PrepareForAccountData()
        {
            _newPostsLimit = _instagramAccount.Posts.Count();
            _newPostsCount = _instagramAccount.Posts.Count() / Constants.NewPostsInitialValueDivider;

            if (_newPostsLimit > Constants.NewPostsLimit)
            {
                _newPostsLimit = Constants.NewPostsLimit;
            }
        }

        private int _overallCounter;
        public int OverallCounter
        {
            get => _overallCounter;
            set
            {
                if (_overallCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _overallCounter = value;

                RaisePropertyChanged(() => OverallCounter);
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

        private int _overallLimit;
        public int OverallLimit
        {
            get => _overallLimit;
            set
            {
                _overallLimit = value;

                RaisePropertyChanged(() => OverallLimit);
            }
        }

        private bool _isInSubscriptionMode;
        public bool IsInSubscriptionMode
        {
            get => _isInSubscriptionMode;
            set
            {
                _isInSubscriptionMode = value;

                RaisePropertyChanged(() => IsInSubscriptionMode);
                RaisePropertyChanged(() => IsInPerPostMode);
            }
        }

        public bool IsInPerPostMode => !IsInSubscriptionMode;

        private int _newPostsCount;
        public int NewPostsCount
        {
            get => _newPostsCount;
            set
            {
                if (value == _newPostsCount)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _newPostsCount = value;

                RaisePropertyChanged(() => NewPostsCount);
            }
        }

        private int _newPostsLimit;
        public int NewPostsLimit
        {
            get => _newPostsLimit;
            set
            {
                _newPostsLimit = value;

                RaisePropertyChanged(() => NewPostsLimit);
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

        private readonly TariffService _currentService;

        public InstagramAutoMenuItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
            IPopupService popupService, IOrderService orderService, ITariffsService tariffsService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            _tariffsService = tariffsService;

            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffs()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            if (!PerPostAvailable)
            {
                IsInSubscriptionMode = true;
            }

            _overallLimit = TariffItems.Max(t => t.Name);
            _overallCounter = TariffItems.First().Name;

            ValidateButtonsState();
            ValidateDiscount();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredLikes()
        {
            OverallCounter = TariffItemsValues.NearestTo(_overallCounter);
        }

        private void CollectOrderDataAndAddToBag(AddToBagButtonPressedMessage message)
        {
            if (!HeaderItem.IsActive)
                return;

            var plan = _currentService.TariffTypes.First(t => t.Name == SelectedTariffType).TariffItems.First(ti => ti.Name == OverallCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffType = SelectedTariffType,
                    TariffPlan = OverallCounter,
                    Price = plan.Price,
                    Discount = plan.Discount,
                    NewPostsCount = NewPostsCount
                },
                IsInBagAlternativeMode = IsInSubscriptionMode,
                IsInBagCompactMode = true,
                ServicePseudonym = HeaderItem.Title,
                NormalModeBagItemLabelText = Strings.Instagram_Perpost,
                AlternativeModeBagItemLabelText = Strings.Instagram_Subscription
            };

            _orderService.AddOrder(orderInfo);
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_overallCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _overallCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_overallCounter);
        }

        private void ValidateDiscount()
        {
            var tariffItem = TariffItems.First(ti => ti.Name == _overallCounter);
            DiscountPercent = tariffItem.DiscountPercent;
        }

        private void DecrementAndValidateButtonsState()
        {
            if(!DecrementButtonActive)
                return;

            var oldValue = _overallCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _overallCounter, skipping it and taking next
            OverallCounter = tariffItemsValues.GetNextTo(_overallCounter);

            //Validating availability
            IncrementCount = oldValue - _overallCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_overallCounter);

            ValidateDiscount();
        }

        private void IncrementAndValidateButtonsState()
        {
            if(!IncrementButtonActive)
                return;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _overallCounter, skipping it and taking next
            OverallCounter = tariffItemsValues.GetNextTo(_overallCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_overallCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _overallCounter;
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

        private bool SubscriptionAvailable
        {
            get
            {
                return _tariffsService.GetCachedTariffs()
                    .First(t => t.Name == SystemKey).TariffServices
                    .First(t => t.Name == Constants.InstagramAutoLikesServiceKey).TariffTypes
                    .First(tt => tt.Name == Constants.InstagramAutoLikesGradual).TariffItems.Any();
            }
        }

        private bool PerPostAvailable
        {
            get
            {
                return _tariffsService.GetCachedTariffs()
                    .First(t => t.Name == SystemKey).TariffServices
                    .First(t => t.Name == Constants.InstagramAutoLikesServiceKey).TariffTypes
                    .First(tt => tt.Name == Constants.InstagramAutoLikesGradual).TariffItems.Any();
            }
        }

        #region Labels
        public MenuHeaderItem HeaderItem { get; } = new MenuHeaderItem
        {
            Title = Strings.Instagram_Auto
        };
        public string NewPostsLabelText => Strings.Instagram_ForNewPosts;
        public string PerpostLabelText => Strings.Instagram_Perpost;
        public string SubscriptionLabelText => Strings.Instagram_Subscription;

        #endregion

        #region Commands

        private IMvxCommand _perpostModeSelectedCommand;
        public IMvxCommand PerpostModeSelectedCommand => _perpostModeSelectedCommand = _perpostModeSelectedCommand ?? new MvxCommand(
                                                                                           () =>
                                                                                           {
                                                                                               if (!PerPostAvailable)
                                                                                               {
                                                                                                   return;
                                                                                               }

                                                                                               IsInSubscriptionMode = false;
                                                                                               AdjustNumberOfDesiredLikes();
                                                                                           });

        private IMvxCommand _subscriptionModeSelectedCommand;
        public IMvxCommand SubscriptionModeSelectedCommand => _subscriptionModeSelectedCommand = _subscriptionModeSelectedCommand ?? new MvxCommand(
                                                                                                     () =>
                                                                                             {
                                                                                                 if (!SubscriptionAvailable)
                                                                                                 {
                                                                                                     return;
                                                                                                 }

                                                                                                 IsInSubscriptionMode = true;
                                                                                                 AdjustNumberOfDesiredLikes();
                                                                                             });
        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState, () => IncrementButtonActive);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState, () => DecrementButtonActive);

        #endregion
    }
}