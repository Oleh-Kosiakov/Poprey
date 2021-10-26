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
    public class TikTokFunsItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private readonly IOrderService _orderService;
        private readonly TariffService _currentService;

        private const string ServiceKey = Constants.TikTokFunsServiceKey;
        private const string SystemKey = Constants.TikTokSystemKey;

        private IEnumerable<TariffItem> TariffItems => _currentService.TariffItems.OrderBy(ti => ti.Name);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

        private int _funsCounter;
        public int FunsCounter
        {
            get => _funsCounter;
            set
            {
                if (_funsCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _funsCounter = value;

                RaisePropertyChanged(() => FunsCounter);
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

        private int _funsLimit = 1;
        public int FunsLimit
        {
            get => _funsLimit;
            set
            {
                _funsLimit = value;

                RaisePropertyChanged(() => FunsLimit);
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
        public TikTokFunsItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
            IPopupService popupService, IOrderService orderService, ITariffsService tariffsService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffsForAdditionalServices()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            var tariffItems = _currentService.TariffItems;

            _funsLimit = tariffItems.Max(t => t.Name);
            _funsCounter = tariffItems[tariffItems.Count / 2].Name;

            ValidateButtonsState();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredFuns()
        {
            FunsCounter = TariffItemsValues.NearestTo(_funsCounter);
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

            var plan = _currentService.TariffItems.First(ti => ti.Name == FunsCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    IsAdditionalService = true,
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffPlan = FunsCounter,
                    Price = plan.Price,
                    ResourceUrl = SelectedVideoUrl
                },
                IsInBagAlternativeMode = false,
                IsInBagCompactMode = true,
                ServicePseudonym = HeaderItem.Title,
                AdditionalOptionsHidden = true
            };

            _orderService.AddOrder(orderInfo);
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_funsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _funsCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_funsCounter);
        }

        private void DecrementAndValidateButtonsState()
        {
            var oldValue = _funsCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _funsCounter, skipping it and taking next
            FunsCounter = tariffItemsValues.GetNextTo(_funsCounter);

            //Validating availability
            IncrementCount = oldValue - _funsCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_funsCounter);
        }

        private void IncrementAndValidateButtonsState()
        {
            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _funsCounter, skipping it and taking next
            FunsCounter = tariffItemsValues.GetNextTo(_funsCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_funsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _funsCounter;
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
            Title = "Funs"
        };

        public string UrlHintText => "URL Profile";

        #endregion

        #region Commands

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState, () => IncrementButtonActive);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState, () => DecrementButtonActive);


        #endregion
    }
}