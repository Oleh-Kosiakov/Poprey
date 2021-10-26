using System.Collections.Generic;
using System.Linq;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Messages;
using Poprey.Core.Models;
using Poprey.Core.Models.Bag;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Models.Instagram.Tariffs;
using Poprey.Core.Resources;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels.InstagramMenuItems
{
    public class InstagramLikeMenuItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private int _selectedPostsCount;

        private const string ServiceKey = Constants.InstagramLikesServiceKey;
        private const string SystemKey = Constants.InstagramSystemKey;

        private readonly IOrderService _orderService;
        private readonly ITariffsService _tariffsService;
        private readonly TariffService _currentService;

        public string SelectedTariffType
        {
            get
            {
                if (!IsInGradualMode)
                {
                    return Constants.InstagramLikesInstant;
                }
                else
                {
                    switch (GradualMode)
                    {
                        case GradualSpeed.Slow:
                            return Constants.InstagramLikesGradualSpeed1;
                        case GradualSpeed.Normal:
                            return Constants.InstagramLikesGradualSpeed;
                        case GradualSpeed.Fast:
                            return Constants.InstagramLikesGradualSpeed2;
                        case GradualSpeed.UltraFast:
                            return Constants.InstagramLikesGradualSpeed3;
                    }
                }

                return null;
            }
        }


        private IEnumerable<TariffItem> TariffItems => _currentService
                                                       .TariffTypes
                                                       .First(t => t.Name == SelectedTariffType)
                                                       .TariffItems
                                                       .Where(ti => ti.Price != default);
        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);


        public int Limit { get; set; }

        private InstagramAccount _instagramAccount;
        public InstagramAccount InstagramAccount
        {
            get => _instagramAccount;
            set
            {
                _instagramAccount = value;

                PrepareForAccountData();
            }
        }

        private int _counter;
        public int Counter
        {
            get => _counter;
            set
            {
                if (!Initialized)
                    return;

                if (_counter == value)
                    return;

                if (value == 0)
                    return;

                _counter = value;

                RecalculateNumberForEachPost();
                RaisePropertyChanged(() => Counter);
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

        private bool _isInGradualMode;
        public bool IsInGradualMode
        {
            get => _isInGradualMode;
            set
            {
                _isInGradualMode = value;

                RaisePropertyChanged(() => IsInGradualMode);
            }
        }

        private GradualSpeed _gradualMode = GradualSpeed.Normal;
        public GradualSpeed GradualMode
        {
            get => _gradualMode;
            set
            {
                _gradualMode = value;

                RaisePropertyChanged(() => GradualMode);
            }
        }

        public MvxObservableCollection<InstagramPostPreview> PostsPreviews { get; } = new MvxObservableCollection<InstagramPostPreview>();

        private int _currentlyShowingItemsCount = Constants.InstagramPostBulkSize;
        public int CurrentlyShowingItemsCount
        {
            get => _currentlyShowingItemsCount;
            set
            {
                _currentlyShowingItemsCount = value;
                RefreshNumberOfVisiblePosts(CurrentlyShowingItemsCount);
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

        public InstagramLikeMenuItemViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger,
                                                IPopupService popupService, IOrderService orderService, ITariffsService tariffsService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            _tariffsService = tariffsService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffs()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);


            HandleIfInstantIsNotAvailable();

            Limit = TariffItems.Max(t => t.Name);
            _counter = TariffItems.First(ti => ti.Price != default).Name;


            ValidateButtonsState();
            ValidateDiscount();
        }


        public void AdjustNumberOfDesiredLikes()
        {
            Counter = TariffItemsValues.NearestTo(_counter);
        }

        private void CollectOrderDataAndAddToBag(AddToBagButtonPressedMessage obj)
        {
            if (!HeaderItem.IsActive)
                return;

            if (!PostsPreviews.Any(pp => pp.IsSelected))
            {
                HeaderViewModel.ExplicitlyShowError("Please select at least one image");
                return;
            }

            if (PostsPreviews.Count(pp => pp.IsSelected) > Constants.MaxSelectedPostsPreviews)
            {
                HeaderViewModel.ExplicitlyShowError($"Please select no more then {Constants.MaxSelectedPostsPreviews} posts.");
                return;
            }

            var plan = _currentService.TariffTypes.First(t => t.Name == SelectedTariffType).TariffItems.First(ti => ti.Name == Counter);
            var offeredImpressions = _tariffsService.GetInstagramExtraImpressionsForTariff(plan, ServiceKey);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffType = SelectedTariffType,
                    TariffPlan = Counter,
                    Price = plan.Price,
                    Discount = plan.Discount
                },
                IsInBagAlternativeMode = IsInGradualMode,
                IsInBagCompactMode = false,
                ServicePseudonym = HeaderItem.Title,
                OfferedImpressionsExtraItem = offeredImpressions
            };

            var postPreviewsList = new List<InstagramPost>();

            foreach (var postPreview in PostsPreviews.Where(pp => pp.IsSelected))
            {
                var pp = new InstagramPost
                {
                    PostImageLink = postPreview.ImageUrl,
                    PostLink = postPreview.PostLink
                };

                postPreviewsList.Add(pp);
            }

            orderInfo.Order.Posts = postPreviewsList;

            _orderService.AddOrder(orderInfo);
        }

        private void PrepareForAccountData()
        {
            RefreshNumberOfVisiblePosts(CurrentlyShowingItemsCount);
        }

        private void RefreshNumberOfVisiblePosts(int requestedNumberOfVisiblePosts)
        {
            var numberOfItemsAlreadyInTheList = PostsPreviews.Count;

            var postItems = _instagramAccount.Posts
                .Skip(numberOfItemsAlreadyInTheList)
                .Take(requestedNumberOfVisiblePosts - numberOfItemsAlreadyInTheList)
                .Select(p => new InstagramPostPreview
                {
                    ImageUrl = p.PostImageLink,
                    PostLink = p.PostLink
                });

            PostsPreviews.AddRange(postItems);
        }

        private void RecalculateNumberForEachPost()
        {
            _selectedPostsCount = PostsPreviews.Count(p => p.IsSelected);

            if (_selectedPostsCount == 0)
            {
                return;
            }

            var numberOfLikesForEachPost = Counter / _selectedPostsCount;

            foreach (var postPreview in PostsPreviews)
            {
                postPreview.Number = numberOfLikesForEachPost;
            }
        }


        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_counter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _counter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_counter);
        }

        private void ValidateDiscount()
        {
            var tariffItem = TariffItems.First(ti => ti.Name == _counter);
            DiscountPercent = tariffItem.DiscountPercent;
        }

        private void DecrementAndValidateButtonsState()
        {
            if(!DecrementButtonActive)
                return;

            var oldValue = _counter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _counter, skipping it and taking next
            Counter = tariffItemsValues.GetNextTo(_counter);

            //Validating availability
            IncrementCount = oldValue - _counter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_counter);

            ValidateDiscount();
        }

        private void IncrementAndValidateButtonsState()
        {
            if (!IncrementButtonActive)
                return;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _counter, skipping it and taking next
            Counter = tariffItemsValues.GetNextTo(_counter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_counter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _counter;
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

        public override void ViewAppeared()
        {
            RaiseAllPropertiesChanged();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        private void HandleIfInstantIsNotAvailable()
        {
            if (!InstantAvailable)
            {
                IsInGradualMode = true;

                if (SlowGradualSpeedAvailable)
                {
                    GradualMode = GradualSpeed.Slow;
                }

                if (FastGradualSpeedAvailable)
                {
                    GradualMode = GradualSpeed.Fast;
                }

                if (UltraFastGradualSpeedAvailable)
                {
                    GradualMode = GradualSpeed.UltraFast;
                }

                if (NormalGradualSpeedAvailable)
                {
                    GradualMode = GradualSpeed.Normal;
                }
            }
        }


        private bool SlowGradualSpeedAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramLikesGradualSpeed1).TariffItems.Any();
            }
        }

        private bool FastGradualSpeedAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramLikesGradualSpeed2).TariffItems.Any();
            }
        }

        private bool UltraFastGradualSpeedAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramLikesGradualSpeed3).TariffItems.Any();
            }
        }

        private bool NormalGradualSpeedAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramLikesGradualSpeed).TariffItems.Any();
            }
        }

        private bool InstantAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramLikesInstant).TariffItems.Any();
            }
        }


        #region Labels

        public string InstantLabelText => Strings.Instagram_Instant;
        public string GradualLabelText => Strings.Instagram_Gradual;
        public string ShowMoreLabelText => Strings.Instagram_ShowMorePhotos;
        public string InstantStartLabelText => Strings.Instagram_InstantStart;
        public string InstantDeliveryLabelText => Strings.Instagram_InstantDelivery;
        public string PermanentLabelText => Strings.Instagram_Permanent;
        public string RealLookingLabelText => Strings.Instagram_RealLooking;
        public string NormalLookingLabelText => Strings.Instagram_NormalLooking;

        #endregion

        #region Commands

        private IMvxCommand _instantModeSelectedCommand;
        public IMvxCommand InstantModeSelectedCommand => _instantModeSelectedCommand = _instantModeSelectedCommand ?? new MvxCommand(
               () =>
               {
                   if (!InstantAvailable)
                   {
                       return;
                   }

                   IsInGradualMode = false;
                   AdjustNumberOfDesiredLikes();
               });



        private IMvxCommand _gradualModeSelectedCommand;
        public IMvxCommand GradualModeSelectedCommand => _gradualModeSelectedCommand = _gradualModeSelectedCommand ?? new MvxCommand(
                       () =>
                       {
                           if (SlowGradualSpeedAvailable)
                           {
                               IsInGradualMode = true;
                               GradualMode = GradualSpeed.Slow;
                           }

                           if (FastGradualSpeedAvailable)
                           {
                               IsInGradualMode = true;
                               GradualMode = GradualSpeed.Fast;
                           }

                           if (UltraFastGradualSpeedAvailable)
                           {
                               IsInGradualMode = true;
                               GradualMode = GradualSpeed.UltraFast;
                           }

                           if (NormalGradualSpeedAvailable)
                           {
                               IsInGradualMode = true;
                               GradualMode = GradualSpeed.Normal;
                           }

                           AdjustNumberOfDesiredLikes();
                       });


        private IMvxCommand _increaseNumberOfVisiblePostsCommand;
        public IMvxCommand IncreaseNumberOfVisiblePostsCommand => _increaseNumberOfVisiblePostsCommand = _increaseNumberOfVisiblePostsCommand ?? new MvxCommand(
                                                                                           () =>
                                                                                           {
                                                                                               CurrentlyShowingItemsCount += Constants.InstagramPostBulkSize;
                                                                                           });

        private IMvxCommand _setSlowModeCommand;
        public IMvxCommand SetSlowModeCommand => _setSlowModeCommand = _setSlowModeCommand ?? new MvxCommand(
            () =>
            {
                if (!SlowGradualSpeedAvailable)
                {
                    return;
                }

                if (GradualMode != GradualSpeed.Slow)
                {
                    GradualMode = GradualSpeed.Slow;
                    AdjustNumberOfDesiredLikes();
                }
            });

        private IMvxCommand _setNormalModeCommand;
        public IMvxCommand SetNormalModeCommand => _setNormalModeCommand = _setNormalModeCommand ?? new MvxCommand(
            () =>
            {
                if (!NormalGradualSpeedAvailable)
                {
                    return;
                }

                if (GradualMode != GradualSpeed.Normal)
                {
                    GradualMode = GradualSpeed.Normal;
                    AdjustNumberOfDesiredLikes();
                }

            });

        private IMvxCommand _setFastModeCommand;
        public IMvxCommand SetFastModeCommand => _setFastModeCommand = _setFastModeCommand ?? new MvxCommand(
            () =>
            {
                if (!FastGradualSpeedAvailable)
                {
                    return;
                }

                if (GradualMode != GradualSpeed.Fast)
                {
                    GradualMode = GradualSpeed.Fast;
                    AdjustNumberOfDesiredLikes();
                }
            });

        private IMvxCommand _setUltraFastModeCommand;
        public IMvxCommand SetUltraFastModeCommand => _setUltraFastModeCommand = _setUltraFastModeCommand ?? new MvxCommand(
            () =>
            {
                if (!UltraFastGradualSpeedAvailable)
                {
                    return;
                }

                if (GradualMode != GradualSpeed.UltraFast)
                {
                    GradualMode = GradualSpeed.UltraFast;
                    AdjustNumberOfDesiredLikes();
                }
            });

        private IMvxCommand _postClickedCommand;
        public IMvxCommand PostClickedCommand => _postClickedCommand = _postClickedCommand ?? new MvxCommand<InstagramPostPreview>(
            pp =>
            {
                pp.IsSelected = !pp.IsSelected;
                RecalculateNumberForEachPost();
            });

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState);

        #endregion

        public MenuHeaderItem HeaderItem { get; } = new MenuHeaderItem
        {
            Title = Strings.Instagram_Like
        };
    }
}