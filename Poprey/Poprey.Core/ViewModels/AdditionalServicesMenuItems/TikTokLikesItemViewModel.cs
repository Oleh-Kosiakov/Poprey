using System.Collections.Generic;
using System.Linq;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using Poprey.Core.Models.Bag;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels.AdditionalServicesMenuItems
{
    public class TikTokLikesItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private readonly IOrderService _orderService;
        private readonly TariffService _currentService;

        private const string ServiceKey = Constants.TikTokLikesServiceKey;
        private const string SystemKey = Constants.TikTokSystemKey;

        private IEnumerable<TariffItem> TariffItems => _currentService.TariffItems.OrderBy(ti => ti.Name);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

        private int _likesCounter;
        public int LikesCounter
        {
            get => _likesCounter;
            set
            {
                if (_likesCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _likesCounter = value;

                RaisePropertyChanged(() => LikesCounter);
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

        private int _likesLimit = 1;
        public int LikesLimit
        {
            get => _likesLimit;
            set
            {
                _likesLimit = value;

                RaisePropertyChanged(() => LikesLimit);
            }
        }

        private bool _isInErrorState;
        public bool IsInErrorState
        {
            get => _isInErrorState;
            set
            {
                _isInErrorState = value;

                RaisePropertyChanged(() => IsInErrorState);
            }
        }

        private string _selectedVideoUrl;
        public string SelectedVideoUrl
        {
            get => _selectedVideoUrl;
            set
            {
                _selectedVideoUrl = value;

                if (!string.IsNullOrEmpty(_selectedVideoUrl))
                {
                    IsInErrorState = false;
                }

                RaisePropertyChanged(() => SelectedVideoUrl);
                RaisePropertyChanged(() => IsAdditionalHintVisible);
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


        public bool IsAdditionalHintVisible => !string.IsNullOrEmpty(_selectedVideoUrl);

        public bool Initialized { get; set; }

        public TikTokLikesItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
                                        IPopupService popupService, IOrderService orderService, ITariffsService tariffsService) 
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffsForAdditionalServices()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            var tariffItems = _currentService.TariffItems;

            _likesLimit = tariffItems.Max(t => t.Name);
            _likesCounter = tariffItems[tariffItems.Count / 2].Name;

            ValidateButtonsState();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredLikes()
        {
            var tariffItemsValues = _currentService.TariffItems.Select(ti => ti.Name);

            LikesCounter = tariffItemsValues.NearestTo(_likesCounter);
        }

        private void CollectOrderDataAndAddToBag(AddToBagButtonPressedMessage obj)
        {
            if (!HeaderItem.IsActive)
                return;

            if (string.IsNullOrEmpty(SelectedVideoUrl))
            {
                IsInErrorState = true;

                return;
            }

            var plan = _currentService.TariffItems.First(ti => ti.Name == LikesCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    IsAdditionalService = true,
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffPlan = LikesCounter,
                    Price = plan.Price,
                    ResourceUrl = SelectedVideoUrl
                },
                IsInBagAlternativeMode = false,
                IsInBagCompactMode = true,
                ServicePseudonym = HeaderItem.Title,
                AdditionalOptionsHidden = true
            };

            _orderService.AddOrder(orderInfo);

            SelectedVideoUrl = string.Empty;
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_likesCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _likesCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_likesCounter);
        }

        private void DecrementAndValidateButtonsState()
        {
            var oldValue = _likesCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _likesCounter, skipping it and taking next
            LikesCounter = tariffItemsValues.GetNextTo(_likesCounter);

            //Validating availability
            IncrementCount = oldValue - _likesCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_likesCounter);
        }

        private void IncrementAndValidateButtonsState()
        {
            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _likesCounter, skipping it and taking next
            LikesCounter = tariffItemsValues.GetNextTo(_likesCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_likesCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _likesCounter;
                IncrementButtonActive = true;
            }
            else
            {
                IncrementCount = 0;
                IncrementButtonActive = false;
            }

            DecrementButtonActive = true;
        }

        #region Labels

        public MenuHeaderItem HeaderItem { get; } = new MenuHeaderItem
        {
            Title = "Likes"
        };

        public string UrlHintText => "URL Video";

        #endregion

        #region Commands

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState, () => IncrementButtonActive);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState, () => DecrementButtonActive);


        #endregion
    }
}