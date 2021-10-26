using System.Collections.Generic;
using System.Linq;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
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
    public class InstagramCommentsItemViewModel : BaseViewModel<object>, IHeaderMenuItemViewModel
    {
        private int _selectedPostsCount;

        private const string ServiceKey = Constants.InstagramCommentsServiceKey;
        private const string SystemKey = Constants.InstagramSystemKey;

        private readonly IOrderService _orderService;
        private readonly ITariffsService _tariffsService;
        private readonly TariffService _currentService;

        public int CommentsLimit { get; private set; }
        public string SelectedTariffType => IsInCustomMode ? Constants.InstagramCommentsCustom : Constants.InstagramCommentsRandom;

        private IEnumerable<TariffItem> TariffItems => _currentService
                                        .TariffTypes
                                        .First(t => t.Name == SelectedTariffType)
                                        .TariffItems
                                        .Where(ti => ti.Price != default);

        private IEnumerable<int> TariffItemsValues => TariffItems.Select(ti => ti.Name);

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

        private int _commentsCounter;
        public int CommentsCounter
        {
            get => _commentsCounter;
            set
            {
                if (!Initialized)
                    return;

                if (_commentsCounter == value)
                    return;

                if (value == 0)
                    return;

                _commentsCounter = value;

                RecalculateNumberForEachPost();
                RaisePropertyChanged(() => CommentsCounter);
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


        private bool _isInCustomMode;
        public bool IsInCustomMode
        {
            get => _isInCustomMode;
            set
            {
                _isInCustomMode = value;

                AdjustNumberOfDesiredComments();

                RaisePropertyChanged(() => IsInCustomMode);
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

        private string _typedCommentText;
        public string TypedCommentText
        {
            get => _typedCommentText;
            set
            {
                _typedCommentText = value;

                RaisePropertyChanged(() => TypedCommentText);
            }
        }

        public MvxObservableCollection<CommentItem> CommentItems { get; } = new MvxObservableCollection<CommentItem>
        {
            new CommentItem
            {
                Text = "It is very cool!"
            },
            new CommentItem
            {
                Text = "Very nice, love the contrast"
            },
            new CommentItem
            {
                Text = "lets go to Vegas on your bday?"
            },
            new CommentItem
            {
                Text = "You are amazing buddy"
            },
            new CommentItem
            {
                Text = "Je m'abonne"
            }
        };

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

        public InstagramCommentsItemViewModel(IMvxNavigationService navigationService,
            IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService,IOrderService orderService, ITariffsService tariffsService)
                : base(navigationService, analyticsService, messenger, popupService)
        {
            _orderService = orderService;
            _tariffsService = tariffsService;
            Messenger.Subscribe<AddToBagButtonPressedMessage>(CollectOrderDataAndAddToBag);

            _currentService = tariffsService.GetCachedTariffs()
                .First(t => t.Name == SystemKey).TariffServices
                .First(t => t.Name == ServiceKey);

            if (!RandomAvailable)
            {
                IsInCustomMode = true;
            }

            CommentsLimit = TariffItems.Max(t => t.Name);
            _commentsCounter = TariffItems.First().Name;

            ValidateButtonsState();
            ValidateDiscount();
        }

        public override void ViewDisappeared()
        {
            HeaderItem.IsActive = false;
        }

        public void AdjustNumberOfDesiredComments()
        {
            CommentsCounter = TariffItemsValues.NearestTo(_commentsCounter);
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
                HeaderViewModel.ExplicitlyShowError($"Please select no more then {Constants.MaxSelectedPostsPreviews} posts");
                return;
            }

            if (!CommentItems.Any() && IsInCustomMode)
            {
                HeaderViewModel.ExplicitlyShowError($"Please enter at least one comment");
                return;
            }

            var plan = _currentService.TariffTypes.First(t => t.Name == SelectedTariffType).TariffItems.First(ti => ti.Name == CommentsCounter);
            var offeredImpressions = _tariffsService.GetInstagramExtraImpressionsForTariff(plan, ServiceKey);

            var orderInfo = new OrderInfo
            {
                Order = new Order
                {
                    SystemName = SystemKey,
                    ServiceName = ServiceKey,
                    TariffType = SelectedTariffType,
                    TariffPlan = CommentsCounter,
                    Price = plan.Price,
                    Discount = plan.Discount,
                    CommentsList = CommentItems.Select(ci => ci.Text).ToList()
                },
                IsInBagAlternativeMode = IsInCustomMode,
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
            RecalculateNumberForEachPost();
        }

        private void RecalculateNumberForEachPost()
        {
            _selectedPostsCount = PostsPreviews.Count(p => p.IsSelected);

            if (_selectedPostsCount == 0)
            {
                return;
            }

            var numberOfLikesForEachPost = CommentsCounter / _selectedPostsCount;

            foreach (var postPreview in PostsPreviews)
            {
                postPreview.Number = numberOfLikesForEachPost;
            }
        }

        private void ValidateButtonsState()
        {
            var tariffItemsValues = TariffItemsValues.ToList();

            var hasNextValue = tariffItemsValues.TryGetNextTo(_commentsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementButtonActive = true;
                IncrementCount = nextValue - _commentsCounter;
            }
            else
            {
                IncrementButtonActive = false;
                IncrementCount = 0;
            }

            tariffItemsValues.Reverse();
            DecrementButtonActive = tariffItemsValues.HasNextTo(_commentsCounter);
        }

        private void ValidateDiscount()
        {
            var tariffItem = TariffItems.First(ti => ti.Name == _commentsCounter);
            DiscountPercent = tariffItem.DiscountPercent;
        }

        private void DecrementAndValidateButtonsState()
        {
            if(!DecrementButtonActive)
                return;

            var oldValue = _commentsCounter;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            tariffItemsValues.Reverse();

            //Reversing items, finding current _commentsCounter, skipping it and taking next
            CommentsCounter = tariffItemsValues.GetNextTo(_commentsCounter);

            //Validating availability
            IncrementCount = oldValue - _commentsCounter;
            IncrementButtonActive = true;
            DecrementButtonActive = tariffItemsValues.HasNextTo(_commentsCounter);

            ValidateDiscount();
        }

        private void IncrementAndValidateButtonsState()
        {
            if(!IncrementButtonActive)
                return;

            //Adjusting value
            var tariffItemsValues = TariffItemsValues.ToList();
            //Reversing items, finding current _commentsCounter, skipping it and taking next
            CommentsCounter = tariffItemsValues.GetNextTo(_commentsCounter);

            //Validating availability
            var hasNextValue = tariffItemsValues.TryGetNextTo(_commentsCounter, out var nextValue);
            if (hasNextValue)
            {
                IncrementCount = nextValue - _commentsCounter;
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

        private bool RandomAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramCommentsRandom).TariffItems.Any();
            }
        }

        private bool CustomAvailable
        {
            get
            {
                return _currentService.TariffTypes.First(tt => tt.Name == Constants.InstagramCommentsCustom).TariffItems.Any();
            }
        }

        #region Labels

        public MenuHeaderItem HeaderItem { get; } = new MenuHeaderItem
        {
            Title = Strings.Instagram_Comments
        };

        public string RandomCommentsLabelText => Strings.Instagram_Random;
        public string CustomCommentsLabelText => Strings.Instagram_Custom;
        public string ShowMoreLabelText => Strings.Instagram_ShowMorePhotos;
        public string InstantStartLabelText => Strings.Instagram_InstantStart;
        public string InstantDeliveryLabelText => Strings.Instagram_InstantDelivery;
        public string PermanentLabelText => Strings.Instagram_Permanent;
        public string NormalLookingLabelText => Strings.Instagram_NormalLooking;
        public string AddCommentHint => Strings.Instagram_AddComment;

        #endregion

        #region Commands

        private IMvxCommand _randomModeSelectedCommand;
        public IMvxCommand RandomModeSelectedCommand => _randomModeSelectedCommand = _randomModeSelectedCommand ?? new MvxCommand(
                                                                                           () =>
                                                                                           {
                                                                                               if (RandomAvailable)
                                                                                               {
                                                                                                   IsInCustomMode = false;

                                                                                                   AdjustNumberOfDesiredComments();
                                                                                               }
                                                                                           });

        private IMvxCommand _customModeSelectedCommand;
        public IMvxCommand CustomModeSelectedCommand => _customModeSelectedCommand = _customModeSelectedCommand ?? new MvxCommand(
                                                                                           () =>
                                                                                           {
                                                                                               if (CustomAvailable)
                                                                                               {
                                                                                                   IsInCustomMode = true;

                                                                                                   AdjustNumberOfDesiredComments();
                                                                                               }
                                                                                           });

        private IMvxCommand _increaseNumberOfVisiblePostsCommand;
        public IMvxCommand IncreaseNumberOfVisiblePostsCommand => _increaseNumberOfVisiblePostsCommand = _increaseNumberOfVisiblePostsCommand ?? new MvxCommand(
                                                                                         () =>
                                                                                         {
                                                                                             CurrentlyShowingItemsCount += Constants.InstagramPostBulkSize;
                                                                                         });

        private IMvxCommand _postClickedCommand;

        public IMvxCommand PostClickedCommand => _postClickedCommand = _postClickedCommand ?? new MvxCommand<InstagramPostPreview>(
                                                                           pp =>
                                                                           {
                                                                               pp.IsSelected = !pp.IsSelected;
                                                                               RecalculateNumberForEachPost();
                                                                           });

        private IMvxCommand _addCommentCommand;
        public IMvxCommand AddCommentCommand => _addCommentCommand = _addCommentCommand ?? new MvxCommand(
                                                                         () =>
                                                                         {
                                                                             var comment = new CommentItem
                                                                             {
                                                                                 Text = TypedCommentText
                                                                             };

                                                                             CommentItems.Add(comment);

                                                                             TypedCommentText = string.Empty;
                                                                         });

        private IMvxCommand _commentClickedCommand;

        public IMvxCommand CommentClickedCommand => _commentClickedCommand = _commentClickedCommand ?? new MvxCommand<CommentItem>(
                                                                                 ci =>
                                                                                 {
                                                                                     CommentItems.Remove(ci);
                                                                                 });

        private IMvxCommand _incrementCommand;
        public IMvxCommand IncrementCommand => _incrementCommand = _incrementCommand ?? new MvxCommand(IncrementAndValidateButtonsState, () => IncrementButtonActive);


        private IMvxCommand _decrementCommand;
        public IMvxCommand DecrementCommand => _decrementCommand = _decrementCommand ?? new MvxCommand(DecrementAndValidateButtonsState, () => DecrementButtonActive);

        #endregion
    }
}