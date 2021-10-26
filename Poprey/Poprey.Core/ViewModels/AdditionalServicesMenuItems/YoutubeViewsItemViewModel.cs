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
    public class YoutubeViewsItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private readonly IOrderService _orderService;
        private readonly TariffService _currentService;

        private const string ServiceKey = Constants.YoutubeViewsServiceKey;
        private const string SystemKey = Constants.YoutubeSystemKey;

        private IEnumerable<TariffItem> TariffItems => _currentService.TariffItems.OrderBy(ti => ti.Name);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);


        private int _viewsCounter;
        public int ViewsCounter
        {
            get => _viewsCounter;
            set
            {
                if (_viewsCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _viewsCounter = value;

                RaisePropertyChanged(() => ViewsCounter);
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

        private int _viewsLimit;
        public int ViewsLimit
        {
            get => _viewsLimit;
            set
            {
                _viewsLimit = value;

                RaisePropertyChanged(() => ViewsLimit);
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

        public bool IsAdditionalHintVisible => !string.IsNullOrEmpty(_selectedVideoUrl) || IsInErrorState;

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

        public YoutubeViewsItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, 
                                            IPopupService popupService, IOrderService orderService, ITariffsService tariffsService) 
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffsForAdditionalServices()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            var tariffItems = _currentService.TariffItems;

            _viewsLimit = tariffItems.Max(t => t.Name);
            _viewsCounter = tariffItems[tariffItems.Count / 2].Name;

            ValidateButtonsState();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredSubscribers()
        {
            ViewsCounter = TariffItemsValues.NearestTo(_viewsCounter);
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

            var plan = _currentService.TariffItems.First(ti => ti.Name == ViewsCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    IsAdditionalService = true,
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffPlan = ViewsCounter,
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

            var hasNextValue = tariffItemsValues.TryGetNextTo(_viewsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _viewsCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_viewsCounter);
        }

        private void DecrementAndValidateButtonsState()
        {
            var oldValue = _viewsCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _viewsCounter, skipping it and taking next
            ViewsCounter = tariffItemsValues.GetNextTo(_viewsCounter);

            //Validating availability
            IncrementCount = oldValue - _viewsCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_viewsCounter);
        }

        private void IncrementAndValidateButtonsState()
        {
            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _viewsCounter, skipping it and taking next
            ViewsCounter = tariffItemsValues.GetNextTo(_viewsCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_viewsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _viewsCounter;
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
            Title = "Views"
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