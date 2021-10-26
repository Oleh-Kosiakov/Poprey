using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.Models;
using Poprey.Core.ViewModels.InstagramMenuItems;
using Poprey.Droid.Adapters;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views.InstagramFragments
{
    public class InstagramViewsFragment : MenuItemBaseFragment<InstagramViewsMenuItemViewModel>
    {
        #region Labels

        //Head Control Panel
        private AnyFontTextView _instagramViewsCounter;
        private AnyFontTextView _instagramViewsInstantLabel;
        private AnyFontTextView _instagramViewsGradualLabel;
        private AnyFontTextView _instagramViewsDiscount;

        private AnyFontTextView _instagramViewsIncrement;
        private AnyFontTextView _instagramViewsDecrement;
        //Gradual Controls
        private AnyFontTextView _instagramViewsSlowLabel;
        private AnyFontTextView _instagramViewsNormalLabel;
        private AnyFontTextView _instagramViewsFastLabel;
        private AnyFontTextView _instagramViewsUltraFastLabel;

        //Show More Button
        private AnyFontTextView _instagramViewsShowMoreLabel;

        //Instant Control Panel
        private AnyFontTextView _instagramViewsInstantHeader;
        private AnyFontTextView _instagramViewsInstantInstantStartLabel;
        private AnyFontTextView _instagramViewsInstantInstantDeliveryLabel;
        private AnyFontTextView _instagramViewsInstantPermanentLabel;
        private AnyFontTextView _instagramViewsInstantRealLookingLabel;

        //Gradual Control Panel
        private AnyFontTextView _instagramViewsGradualHeader;
        private AnyFontTextView _instagramViewsGradualInstantStartLabel;
        private AnyFontTextView _instagramViewsGradualInstantDeliveryLabel;
        private AnyFontTextView _instagramViewsGradualPermanentLabel;
        private AnyFontTextView _instagramViewsGradualNormalLookingLabel;

        #endregion

        private LinearLayout _instagramLikesGradualControlPanel;
        private LinearLayout _showMoreLayout;

        private MvxRecyclerView _postsRecyclerView;
        private InstagramPostPreviewAdapter _postsAdapter;

        protected override int FragmentId => Resource.Layout.instagram_views_fragment;
        protected override void InitComponents(View fragmentView)
        {
            #region Labels
            //Head Control Panel
            _instagramViewsCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_counter);
            _instagramViewsInstantLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_label);
            _instagramViewsGradualLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_label);
            _instagramViewsDiscount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_discount);
            _instagramViewsIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_increment);
            _instagramViewsDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_decrement);

            //Gradual Controls
            _instagramViewsSlowLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_less_label);
            _instagramViewsNormalLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_n_label);
            _instagramViewsFastLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_more_label);
            _instagramViewsUltraFastLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_much_more_label);
            //Show More Button
            _instagramViewsShowMoreLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_show_more_label);
            //Instant Control Panel
            _instagramViewsInstantHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_header);
            _instagramViewsInstantInstantStartLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_instant_start_label);
            _instagramViewsInstantInstantDeliveryLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_instant_delivery_label);
            _instagramViewsInstantPermanentLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_permanent_label);
            _instagramViewsInstantRealLookingLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_real_looking_label);

            //Gradual Control Panel
            _instagramViewsGradualHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_header);
            _instagramViewsGradualInstantStartLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_instant_start_label);
            _instagramViewsGradualInstantDeliveryLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_instant_delivery_label);
            _instagramViewsGradualPermanentLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_permanent_label);
            _instagramViewsGradualNormalLookingLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_normal_looking_label);

            #endregion

            _instagramLikesGradualControlPanel = fragmentView.FindViewById<LinearLayout>(Resource.Id.instagram_likes_gradual_control_panel);
            _showMoreLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.instagram_likes_show_more_layout);
            _postsRecyclerView = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.instagram_likes_posts_view);

            var gridLayoutManager = new GridLayoutManager(Context, SpanCount);
            _postsRecyclerView.SetLayoutManager(gridLayoutManager);

            _postsAdapter = new InstagramPostPreviewAdapter((IMvxAndroidBindingContext)BindingContext);
            _postsRecyclerView.Adapter = _postsAdapter;
        }


        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<InstagramViewsFragment, InstagramViewsMenuItemViewModel>();

            #region Labels

            //Head Control Panel
            bindingSet.Bind(_instagramViewsInstantLabel).For(v => v.Text).To(vm => vm.InstantLabelText);
            bindingSet.Bind(_instagramViewsGradualLabel).For(v => v.Text).To(vm => vm.GradualLabelText);

            //Show More Button
            bindingSet.Bind(_instagramViewsShowMoreLabel).For(v => v.Text).To(vm => vm.ShowMoreLabelText);

            //Instant Control Panel
            bindingSet.Bind(_instagramViewsInstantHeader).For(v => v.Text).To(vm => vm.InstantLabelText);
            bindingSet.Bind(_instagramViewsInstantInstantStartLabel).For(v => v.Text).To(vm => vm.InstantStartLabelText);
            bindingSet.Bind(_instagramViewsInstantInstantDeliveryLabel).For(v => v.Text).To(vm => vm.InstantDeliveryLabelText);
            bindingSet.Bind(_instagramViewsInstantPermanentLabel).For(v => v.Text).To(vm => vm.PermanentLabelText);
            bindingSet.Bind(_instagramViewsInstantRealLookingLabel).For(v => v.Text).To(vm => vm.RealLookingLabelText);

            //Gradual Control Panel
            bindingSet.Bind(_instagramViewsGradualHeader).For(v => v.Text).To(vm => vm.GradualLabelText);
            bindingSet.Bind(_instagramViewsGradualInstantStartLabel).For(v => v.Text).To(vm => vm.InstantStartLabelText);
            bindingSet.Bind(_instagramViewsGradualInstantDeliveryLabel).For(v => v.Text).To(vm => vm.InstantDeliveryLabelText);
            bindingSet.Bind(_instagramViewsGradualPermanentLabel).For(v => v.Text).To(vm => vm.PermanentLabelText);
            bindingSet.Bind(_instagramViewsGradualNormalLookingLabel).For(v => v.Text).To(vm => vm.NormalLookingLabelText);

            #endregion

            bindingSet.Bind(_instagramViewsCounter).For(v => v.Text).To(vm => vm.Counter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_instagramViewsDiscount).For(v => v.Text).To(vm => vm.DiscountPercent).OneWay().WithConversion("Discount");
            bindingSet.Bind(_instagramViewsDiscount).For(v => v.Visibility).To(vm => vm.DiscountPercent).OneWay().WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DiscountPresent).To(vm => vm.DiscountPercent).WithConversion("IntToBool");

            bindingSet.Bind(this).For(v => v.IsInGradualMode).To(vm => vm.IsInGradualMode);
            bindingSet.Bind(_instagramViewsInstantLabel).For("Click").To(vm => vm.InstantModeSelectedCommand);
            bindingSet.Bind(_instagramViewsGradualLabel).For("Click").To(vm => vm.GradualModeSelectedCommand);
            bindingSet.Bind(_instagramLikesGradualControlPanel).For(v => v.Visibility).To(vm => vm.IsInGradualMode).WithConversion("GoneVisibility");

            bindingSet.Bind(_instagramViewsSlowLabel).For("Click").To(vm => vm.SetSlowModeCommand);
            bindingSet.Bind(_instagramViewsNormalLabel).For("Click").To(vm => vm.SetNormalModeCommand);
            bindingSet.Bind(_instagramViewsFastLabel).For("Click").To(vm => vm.SetFastModeCommand);
            bindingSet.Bind(_instagramViewsUltraFastLabel).For("Click").To(vm => vm.SetUltraFastModeCommand);
            bindingSet.Bind(_instagramViewsIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_instagramViewsDecrement).For("Click").To(vm => vm.DecrementCommand);
            bindingSet.Bind(this).For(v => v.GradualMode).To(vm => vm.GradualMode);

            bindingSet.Bind(_postsAdapter).For(v => v.ItemsSource).To(vm => vm.PostsPreviews);
            bindingSet.Bind(_postsAdapter).For(v => v.ItemClick).To(vm => vm.PostClickedCommand);

            bindingSet.Bind(_showMoreLayout).For("Click").To(vm => vm.IncreaseNumberOfVisiblePostsCommand);

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

                SetColorToCounter(_instagramViewsCounter, _discountPresent);
            }
        }


        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                ToggleButtonState(_instagramViewsIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_instagramViewsDecrement, _decrementButtonActive);
            }
        }


        private GradualSpeed _gradualMode;
        public GradualSpeed GradualMode
        {
            get => _gradualMode;
            set
            {
                _gradualMode = value;

                VisualizeGradualMode();
            }
        }

        private bool _isInGradualMode;

        public bool IsInGradualMode
        {
            get => _isInGradualMode;
            set
            {
                _isInGradualMode = value;

                ChangeTextColor(_instagramViewsInstantLabel, !IsInGradualMode);
                ChangeTextColor(_instagramViewsGradualLabel, IsInGradualMode);
            }
        }

        private void VisualizeGradualMode()
        {
            AnyFontTextView viewToSetBackground = null;

            switch (_gradualMode)
            {
                case GradualSpeed.Slow:
                    viewToSetBackground = _instagramViewsSlowLabel;
                    break;
                case GradualSpeed.Normal:
                    viewToSetBackground = _instagramViewsNormalLabel;
                    break;
                case GradualSpeed.Fast:
                    viewToSetBackground = _instagramViewsFastLabel;
                    break;
                case GradualSpeed.UltraFast:
                    viewToSetBackground = _instagramViewsUltraFastLabel;
                    break;
            }

            viewToSetBackground?.SetBackgroundResource(Resource.Drawable.selected_gradual_mode_button_background);

            if (viewToSetBackground != _instagramViewsSlowLabel)
            {
                _instagramViewsSlowLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramViewsNormalLabel)
            {
                _instagramViewsNormalLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramViewsFastLabel)
            {
                _instagramViewsFastLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramViewsUltraFastLabel)
            {
                _instagramViewsUltraFastLabel.SetBackgroundResource(0);
            }
        }
    }
}