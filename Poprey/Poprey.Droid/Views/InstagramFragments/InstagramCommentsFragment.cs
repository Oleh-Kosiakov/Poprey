using System.Linq;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.ViewModels.InstagramMenuItems;
using Poprey.Droid.Adapters;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views.InstagramFragments
{
    public class InstagramCommentsFragment : MenuItemBaseFragment<InstagramCommentsItemViewModel>
    {
        //Head Control Panel
        private AnyFontTextView _commentsCounter;
        private AnyFontTextView _instagramFollowersDiscount;
        private AnyFontTextView _randomCommentsLabel;
        private AnyFontTextView _customCommentsLabel;

        private AnyFontTextView _instagramFollowersIncrement;
        private AnyFontTextView _instagramFollowersDecrement;

        //Show More Button
        private AnyFontTextView _showMoreLabel;

        //Instant Control Panel
        private AnyFontTextView _instantStartLabel;
        private AnyFontTextView _instantDeliveryLabel;
        private AnyFontTextView _permanentLabel;
        private AnyFontTextView _normalLookingLabel;

        private LinearLayout _showMoreLayout;

        private MvxRecyclerView _postsRecyclerView;
        private InstagramPostPreviewAdapter _postsAdapter;

        private View _instagramCommentsWhiteBackground;
        private RelativeLayout _instagramCommentsCustomCommentsLayout;
        private AdaptiveBackgroundImageView _instagramCommentsSaveCommentView;
        private AnyFontEditText _instagramCommentsAddCommentEditText;

        private MvxRecyclerView _commentsRecyclerView;
        private CommentsAdapter _commentsAdapter;

        protected override int FragmentId => Resource.Layout.instagram_comments_fragment;

        protected override void InitComponents(View fragmentView)
        {
            //Head Control Panel
            _commentsCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_counter);
            _randomCommentsLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_random_label);
            _customCommentsLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_custom_label);
            _instagramFollowersDiscount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_discount);
            _instagramFollowersIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_increment);
            _instagramFollowersDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_decrement);

            //Show More Button
            _showMoreLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_show_more_label);
            //Instant Control Panel
            _instantStartLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_instant_instant_start_label);
            _instantDeliveryLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_instant_instant_delivery_label);
            _permanentLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_instant_permanent_label);
            _normalLookingLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_comments_instant_normal_looking_label);

            _showMoreLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.instagram_comments_show_more_layout);
            _postsRecyclerView = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.instagram_comments_posts_view);

            _instagramCommentsWhiteBackground = fragmentView.FindViewById<View>(Resource.Id.instagram_comments_white_background);
            _instagramCommentsCustomCommentsLayout = fragmentView.FindViewById<RelativeLayout>(Resource.Id.instagram_comments_custom_comments_layout);
            _instagramCommentsSaveCommentView = fragmentView.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.instagram_comments_save_comment_view);
            _instagramCommentsAddCommentEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.instagram_comments_add_comment_edit_text);

            _commentsRecyclerView = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.instagram_comments_comments_view);

            var gridLayoutManager = new GridLayoutManager(Context, SpanCount);
            _postsRecyclerView.SetLayoutManager(gridLayoutManager);

            _postsAdapter = new InstagramPostPreviewAdapter((IMvxAndroidBindingContext)BindingContext);
            _postsRecyclerView.Adapter = _postsAdapter;

            var linearLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Vertical, true);
            _commentsRecyclerView.SetLayoutManager(linearLayoutManager);

            _commentsAdapter = new CommentsAdapter((IMvxAndroidBindingContext)BindingContext);
            _commentsRecyclerView.Adapter = _commentsAdapter;

            _instagramCommentsAddCommentEditText.AfterTextChanged += InstagramCommentsAddCommentEditTextOnAfterTextChanged;
        }

       
        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<InstagramCommentsFragment, InstagramCommentsItemViewModel>();

            bindingSet.Bind(_commentsCounter).For(v => v.Text).To(vm => vm.CommentsCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_instagramFollowersDiscount).For(v => v.Text).To(vm => vm.DiscountPercent).OneWay().WithConversion("Discount");
            bindingSet.Bind(_instagramFollowersDiscount).For(v => v.Visibility).To(vm => vm.DiscountPercent).OneWay().WithConversion("Visibility");
            bindingSet.Bind(_randomCommentsLabel).For(v => v.Text).To(vm => vm.RandomCommentsLabelText);
            bindingSet.Bind(_customCommentsLabel).For(v => v.Text).To(vm => vm.CustomCommentsLabelText);
            bindingSet.Bind(_showMoreLabel).For(v => v.Text).To(vm => vm.ShowMoreLabelText);

            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();

            bindingSet.Bind(_instantStartLabel).For(v => v.Text).To(vm => vm.InstantStartLabelText);
            bindingSet.Bind(_instantDeliveryLabel).For(v => v.Text).To(vm => vm.InstantDeliveryLabelText);
            bindingSet.Bind(_permanentLabel).For(v => v.Text).To(vm => vm.PermanentLabelText);
            bindingSet.Bind(_normalLookingLabel).For(v => v.Text).To(vm => vm.NormalLookingLabelText);

            bindingSet.Bind(this).For(v => v.IsInCustomMode).To(vm => vm.IsInCustomMode);
            bindingSet.Bind(this).For(v => v.DiscountPresent).To(vm => vm.DiscountPercent).WithConversion("IntToBool");

            bindingSet.Bind(_randomCommentsLabel).For("Click").To(vm => vm.RandomModeSelectedCommand);
            bindingSet.Bind(_customCommentsLabel).For("Click").To(vm => vm.CustomModeSelectedCommand);
            bindingSet.Bind(_instagramFollowersIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_instagramFollowersDecrement).For("Click").To(vm => vm.DecrementCommand);

            bindingSet.Bind(_postsAdapter).For(v => v.ItemsSource).To(vm => vm.PostsPreviews);
            bindingSet.Bind(_postsAdapter).For(v => v.ItemClick).To(vm => vm.PostClickedCommand);

            bindingSet.Bind(_showMoreLayout).For("Click").To(vm => vm.IncreaseNumberOfVisiblePostsCommand);

            bindingSet.Bind(_instagramCommentsWhiteBackground).For(v => v.Visibility).To(vm => vm.IsInCustomMode).WithConversion("Visibility");
            bindingSet.Bind(_instagramCommentsCustomCommentsLayout).For(v => v.Visibility).To(vm => vm.IsInCustomMode).WithConversion("GoneVisibility");
            bindingSet.Bind(_instagramCommentsSaveCommentView).For("Click").To(vm => vm.AddCommentCommand);
            bindingSet.Bind(_instagramCommentsAddCommentEditText).For(v => v.Hint).To(vm => vm.AddCommentHint);
            bindingSet.Bind(_instagramCommentsAddCommentEditText).For(v => v.Text).To(vm => vm.TypedCommentText);

            bindingSet.Bind(_commentsAdapter).For(v => v.ItemsSource).To(vm => vm.CommentItems);
            bindingSet.Bind(_commentsAdapter).For(v => v.ItemClick).To(vm => vm.CommentClickedCommand);

            bindingSet.Apply();

            ViewModel.Initialized = true;
        }
        private bool _discountPresent;
        public bool DiscountPresent
        {
            get => _discountPresent;
            set
            {
                _discountPresent = value;

                SetColorToCounter(_commentsCounter, _discountPresent);
            }
        }


        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                ToggleButtonState(_instagramFollowersIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_instagramFollowersDecrement, _decrementButtonActive);
            }
        }
        private bool _isInCustomMode;
        public bool IsInCustomMode
        {
            get => _isInCustomMode;
            set
            {
                _isInCustomMode = value;

                ChangeTextColor(_randomCommentsLabel, !IsInCustomMode);
                ChangeTextColor(_customCommentsLabel, IsInCustomMode);
            }
        }

        private void InstagramCommentsAddCommentEditTextOnAfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            if (e == null || !e.Editable.Any())
            {
                return;
            }

            if (e.Editable.Last() == '\n')
            {
                ViewModel.AddCommentCommand.Execute();
            }
        }
    }
}