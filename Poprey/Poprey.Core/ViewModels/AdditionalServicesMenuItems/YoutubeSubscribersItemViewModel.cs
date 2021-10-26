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
    public class YoutubeSubscribersItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private readonly IOrderService _orderService;
        private readonly TariffService _currentService;

        private const string ServiceKey = Constants.YoutubeSubscribersServiceKey;
        private const string SystemKey = Constants.YoutubeSystemKey;

        private IEnumerable<TariffItem> TariffItems => _currentService.TariffItems.OrderBy(ti => ti.Name);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

        private int _subscribersCounter;
        public int SubscribersCounter
        {
            get => _subscribersCounter;
            set
            {
                if (_subscribersCounter == value)
                    return;

                if (!Initialized)
                    return;

                if (value == 0)
                    return;

                _subscribersCounter = value;

                RaisePropertyChanged(() => SubscribersCounter);
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


        private int _subscribersLimit;
        public int SubscribersLimit
        {
            get => _subscribersLimit;
            set
            {
                _subscribersLimit = value;

                RaisePropertyChanged(() => SubscribersLimit);
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


        private string _selectedAccountUrl;
        public string SelectedAccountUrl
        {
            get => _selectedAccountUrl;
            set
            {
                _selectedAccountUrl = value;


                if (!string.IsNullOrEmpty(_selectedAccountUrl))
                {
                    IsInErrorState = false;
                }

                RaisePropertyChanged(() => SelectedAccountUrl);
                RaisePropertyChanged(() => IsAdditionalHintVisible);
            }
        }

        public bool IsAdditionalHintVisible => !string.IsNullOrEmpty(_selectedAccountUrl);

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
        public YoutubeSubscribersItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, 
            IPopupService popupService, IOrderService orderService, ITariffsService tariffsService) 
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffsForAdditionalServices()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            var tariffItems = _currentService.TariffItems;

            _subscribersCounter = 100;
            _subscribersLimit = 100;

            _subscribersLimit = tariffItems.Max(t => t.Name);
            _subscribersCounter = tariffItems[tariffItems.Count / 2].Name;

            ValidateButtonsState();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        private void CollectOrderDataAndAddToBag(AddToBagButtonPressedMessage obj)
        {
            if (!HeaderItem.IsActive)
                return;

            var plan = _currentService.TariffItems.First(ti => ti.Name == SubscribersCounter);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    IsAdditionalService = true,
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffPlan = SubscribersCounter,
                    Price = plan.Price,
                    ResourceUrl = SelectedAccountUrl
                },
                IsInBagAlternativeMode = false,
                IsInBagCompactMode = true,
                ServicePseudonym = HeaderItem.Title,
                AdditionalOptionsHidden = true
            };

            _orderService.AddOrder(orderInfo);

            SelectedAccountUrl = string.Empty;
        }


        public void AdjustNumberOfDesiredSubscribers()
        {
            SubscribersCounter = TariffItemsValues.NearestTo(_subscribersCounter);
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_subscribersCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _subscribersCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_subscribersCounter);
        }

        private void DecrementAndValidateButtonsState()
        {
            var oldValue = _subscribersCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _subscribersCounter, skipping it and taking next
            SubscribersCounter = tariffItemsValues.GetNextTo(_subscribersCounter);

            //Validating availability
            IncrementCount = oldValue - _subscribersCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_subscribersCounter);
        }

        private void IncrementAndValidateButtonsState()
        {
            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _subscribersCounter, skipping it and taking next
            SubscribersCounter = tariffItemsValues.GetNextTo(_subscribersCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_subscribersCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _subscribersCounter;
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
            Title = "Subscribers"
        };

        public string UrlHintText => "Account Video";

        #endregion

        #region Commands

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState, () => IncrementButtonActive);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState, () => DecrementButtonActive);

        #endregion

    }
}