using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Models;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Resources;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class BagViewModelSingleton : BaseViewModel<object>
    {
        public enum BagState
        {
            Collapsed,
            Expanded,
            Fullscreen
        }

        private bool _ignoreKeyboardStateChanges = true;

        private readonly IWebBrowserService _webBrowserService;
        private readonly IOrderService _orderService;
        public InstagramAccount InstagramAccount { get; set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;

                RaisePropertyChanged(() => IsVisible);
            }
        }

        public BagState RecommendedBigBagState { get; set; } = BagState.Expanded;

        private BagState _currentBagState = BagState.Collapsed;
        public BagState CurrentBagState
        {
            get => _currentBagState;
            set
            {
                _currentBagState = value;

                if (_currentBagState == BagState.Fullscreen)
                {
                    HeaderViewModel.IsCollapsed = true;
                }
                else
                {
                    HeaderViewModel.IsCollapsed = false;
                }

                RaisePropertyChanged(() => CurrentBagState);
            }
        }

        #region CollapsedBag

        private readonly List<string> _servicesInTheCart = new List<string>();
        private string _tempServiceName;

        public string ServiceName
        {
            get
            {
                var builder = new StringBuilder();

                foreach (var service in _servicesInTheCart)
                {
                    if (service != _tempServiceName)
                    {
                        builder.Append(service + ", ");
                    }
                }

                builder.Append(_tempServiceName);

                return builder.ToString();
            }
        }

        private double _bagSum;
        public double BagSum
        {
            get => _bagSum;
            set
            {
                _bagSum = value;

                RaisePropertyChanged(() => BagSum);
            }
        }

        #endregion

        #region BagFooter

        private int _discountPercents;
        public int DiscountPercents
        {
            get => _discountPercents;
            set
            {
                _discountPercents = value;

                RaisePropertyChanged(() => DiscountPercents);
            }
        }

        public List<PaymentMethodItem> PaymentMethods { get; } = new List<PaymentMethodItem>
        {
            new PaymentMethodItem
            {
                Name = Strings.Bag_PayByCard,
                PaymentMethod = PaymentMethod.PayByCard
            },
            //new PaymentMethodItem
            //{
            //    Name = Strings.Bag_PayPal,
            //    PaymentMethod = PaymentMethod.PayPal
            //},
            //new PaymentMethodItem
            //{
            //    Name = Strings.Bag_Wallet,
            //    PaymentMethod = PaymentMethod.Wallet
            //}
        };

        #endregion

        #region BagList

        public MvxObservableCollection<BagItem> BagItems { get; } = new MvxObservableCollection<BagItem>();

        #endregion

        public EmailDialogViewModel EmailDialogViewModel { get; } = Mvx.IoCProvider.Resolve<EmailDialogViewModel>();

        public BagViewModelSingleton(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
                                    IPopupService popupService, IWebBrowserService webBrowserService, IOrderService orderService)
                                    : base(navigationService, analyticsService, messenger, popupService)
        {
            _webBrowserService = webBrowserService;
            _orderService = orderService;
            EmailDialogViewModel.ShowLicenseCommand = ShowLicenseCommand;
            EmailDialogViewModel.OnProceedWithEmail = ProceedWithEmail;

            ShowLicenseCommand = new MvxCommand(() =>
            {
                _webBrowserService.OpenWebPage(Constants.TermsOfServiceUrl);
            });

            Messenger.Subscribe<SetCollapsedBagDataMessage>(HandleBagDataMessage);
            Messenger.Subscribe<AddToBagMessage>(HandleAddToBagMessage);
            Messenger.Subscribe<EmptifyBagMessage>(EmptifyBag);
            Messenger.Subscribe<IgnoreKeyboardOnBagMessage>(HandleIgnoreKeyboardMessage);
        }

    
        public void NotifyAboutKeyboard(bool isAppeared)
        {
            if (_ignoreKeyboardStateChanges)
            {
                return;
            }

            //Hide only if it is in collapsed mode
            if (CurrentBagState == BagState.Collapsed && isAppeared)
            {
                IsVisible = false;
            }
            else if (!isAppeared)
            {
                IsVisible = true;
            }
        }
        //TODO: Handle disabled impressions
        private void HandleAddToBagMessage(AddToBagMessage message)
        {
            var bagItem = new BagItem
            {
                Order = message.OrderInfo.Order,
                ServiceName = message.OrderInfo.Order.ServiceName.Capitalize(),
                ServicePseudonym = message.OrderInfo.ServicePseudonym,
                IsInAlternativeMode = message.OrderInfo.IsInBagAlternativeMode,
                IsCompact = message.OrderInfo.IsInBagCompactMode,
                OptionsVisibleInTheCompactMode = !message.OrderInfo.AdditionalOptionsHidden,
                TariffPlan = message.OrderInfo.Order.TariffPlan,
                ServicePrice = message.OrderInfo.Order.Price,
                DiscountPercent = message.OrderInfo.DiscountPercent,
                AlternativeLabelText = message.OrderInfo.AlternativeModeBagItemLabelText,
                NormalLabelText = message.OrderInfo.NormalModeBagItemLabelText
            };

            bagItem.RemoveCommand = new MvxCommand(RemoveAction(bagItem));

            if (message.OrderInfo.OfferedImpressionsExtraItem != null)
            {
                bagItem.ImpressionsCount = message.OrderInfo.OfferedImpressionsExtraItem.Name;
                bagItem.ImpressionPrice = message.OrderInfo.OfferedImpressionsExtraItem.Price;
            }

            if (!message.OrderInfo.IsInBagCompactMode)
            {
                var itemsPerEachPost = message.OrderInfo.Order.TariffPlan / message.OrderInfo.Order.Posts.Count;

                bagItem.PostPreviews = new MvxObservableCollection<InstagramPostPreview>(
                    message.OrderInfo.Order.Posts.Select(
                        pp => new InstagramPostPreview
                        {
                            ImageUrl = pp.PostImageLink,
                            IsSelected = true,
                            Number = itemsPerEachPost
                        }));
            }

            BagItems.Insert(0, bagItem);

            if (!_servicesInTheCart.Contains(bagItem.ServicePseudonym))
            {
                _servicesInTheCart.Add(bagItem.ServicePseudonym);
                RaisePropertyChanged(() => ServiceName);
            }

            InvalidateSeparators();
            InvalidateDiscount();

            Messenger.Publish(new SetCollapsedBagDataMessage(this)
            {
                BagSum = RecalculateBagSum(),
                TempServiceName = message.OrderInfo.Order.ServiceName
            });
        }

        private Action RemoveAction(BagItem bagItem)
        {
            return () =>
            {
                BagItems.Remove(bagItem);
                InvalidateSeparators();
                InvalidateDiscount();
                _servicesInTheCart.Remove(bagItem.ServicePseudonym);
                BagSum = RecalculateBagSum();

                RaisePropertyChanged(() => ServiceName);
            };
        }

        private void EmptifyBag(EmptifyBagMessage obj)
        {
            BagItems.Clear();
            InvalidateSeparators();
            InvalidateDiscount();
            _servicesInTheCart.Clear();
            BagSum = RecalculateBagSum();

            RaisePropertyChanged(() => ServiceName);
        }

        private void InvalidateSeparators()
        {
            foreach (var bagItem in BagItems.Take(BagItems.Count - 1))
            {
                bagItem.IsSeparatorVisible = true;
            }

            if (BagItems.LastOrDefault() != null)
            {
                BagItems.Last().IsSeparatorVisible = false;
            }
        }

        private void InvalidateDiscount()
        {
            DiscountPercents = BagItems.Any() ? BagItems.Max(bi => bi.DiscountPercent) : 0;
        }

        private void HandleBagDataMessage(SetCollapsedBagDataMessage message)
        {
            if (message.IsBagVisible.HasValue)
            {
                IsVisible = message.IsBagVisible.Value;
            }

            if (message.TempServiceName != null)
            {
                _tempServiceName = message.TempServiceName;
                RaisePropertyChanged(() => ServiceName);
            }

            if (message.BagSum.HasValue)
            {
                BagSum = message.BagSum.Value;
            }

            if (message.SubstractFromSum.HasValue)
            {
                BagSum -= message.SubstractFromSum.Value;
            }

            if (message.AddToSum.HasValue)
            {
                BagSum += message.AddToSum.Value;
            }
        }

        private void HandleIgnoreKeyboardMessage(IgnoreKeyboardOnBagMessage message)
        {
            _ignoreKeyboardStateChanges = message.Ignore;
        }

        private double RecalculateBagSum()
        {
            return BagItems.Sum(o => o.Order.Price);
        }

        private async Task ProceedWithEmail()
        {
            foreach (var bagItem in BagItems)
            {
                bagItem.Order.UserNickname = InstagramAccount != null ? InstagramAccount.InstagramNickname : null;
                bagItem.Order.UserEmail = EmailDialogViewModel.EnteredEmail;

                if (bagItem.IsImpressionsAdded)
                {
                    bagItem.Order.Impressions = bagItem.ImpressionsCount;
                }
            }

            var orders = BagItems.Select(bi => bi.Order).ToList();

            OrderResponse orderResponse = null;

            var e = await WebRequest(async () =>
            {
                orderResponse = await _orderService.CreateOrders(orders, CtsHelper.CreateCts().Token);
            });

            switch (e)
            {
                case ServiceResolution.Success:
                    CurrentBagState = BagState.Collapsed;

                    var navigationArgument = new ThanYouPageArguments
                    {
                        HeaderText = "Thank you for\r\nyour purchase",
                        SubheaderText = "Order will start now",
                        PriceVisible = true,
                        Price = BagSum
                    };
                
                    if (DiscountPercents != 0)
                    {
                        navigationArgument.SubpriceText = "You saved on discounts";
                        navigationArgument.SubpriceVisible = true;
                    }

                    await NavigationService.Navigate<ThankYouViewModel, ThanYouPageArguments>(navigationArgument);

                    _webBrowserService.OpenWebPage(orderResponse.Redirect);
                    break;

                default:
                    HeaderViewModel.ExplicitlyShowError("Error during ordering. Please try again later");
                    break;
            }
        }

        public void InvalidateBigState()
        {
            if (CurrentBagState != BagState.Collapsed && CurrentBagState != RecommendedBigBagState)
            {
                CurrentBagState = RecommendedBigBagState;
            }
        }

        #region Labels

        public string TotalPriceLabelText => Strings.Bag_TotalPrice;
        public string FooterTermsAndConditionsText => Strings.Bag_FooterTermsAndConditions;
        public string FooterTermsAndConditionsUnderlinedText => Strings.Bag_FooterTermsAndConditionsUnderlined;

        #endregion

        #region Commands

        public IMvxCommand ShowLicenseCommand { get; set; }

        private IMvxCommand _bagPressedCommand;
        public IMvxCommand BagPressedCommand => _bagPressedCommand = _bagPressedCommand ?? new MvxCommand(() =>
        {
            Messenger.Publish(new AddToBagButtonPressedMessage(this));
        });

        private IMvxCommand _bagFooterPressedCommand;
        public IMvxCommand BagFooterPressedCommand => _bagFooterPressedCommand = _bagFooterPressedCommand ?? new MvxCommand(() =>
        {
            if (BagItems.Any())
            {
                CurrentBagState = RecommendedBigBagState;
            }
        });

        private IMvxCommand _paymentMethodSelectedCommand;
        public IMvxCommand PaymentMethodSelectedCommand => _paymentMethodSelectedCommand = _paymentMethodSelectedCommand ?? new MvxCommand<PaymentMethodItem>(
       (pmi) =>
        {
            EmailDialogViewModel.ShowLicenseCommand = ShowLicenseCommand;
            PopupService.ShowEmailDialog(EmailDialogViewModel);
        });

        private IMvxCommand _toggleDrawerCommand;
        public IMvxCommand ToggleDrawerCommand => _toggleDrawerCommand = _toggleDrawerCommand ?? new MvxCommand(
     () =>
        {
            HeaderViewModel.IsDrawerMenuExpanded = true;
        });

        private IMvxCommand _closeCommand;
        public IMvxCommand CloseCommand => _closeCommand = _closeCommand ?? new MvxCommand(() =>
        {
            CurrentBagState = BagState.Collapsed;
        });

        private IMvxCommand _emptyBagCommand;
        public IMvxCommand EmptyBagCommand => _emptyBagCommand = _emptyBagCommand ?? new MvxCommand(() =>
        {
            BagItems.Clear();
            InvalidateSeparators();
            _servicesInTheCart.Clear();
            BagSum = RecalculateBagSum();

            RaisePropertyChanged(() => ServiceName);
        });


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var bagItem in BagItems)
                {
                    bagItem.RemoveCommand = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}