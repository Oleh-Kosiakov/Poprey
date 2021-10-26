using System;
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
    public class InstagramLikesFragment : MenuItemBaseFragment<InstagramLikeMenuItemViewModel>
    {
        #region Labels

        //Head Control Panel
        private AnyFontTextView _instagramLikesCounter;
        private AnyFontTextView _instagramLikesInstantLabel;
        private AnyFontTextView _instagramLikesGradualLabel;
        private AnyFontTextView _instagramLikesDiscount;

        private AnyFontTextView _instagramLikesIncrement;
        private AnyFontTextView _instagramLikesDecrement;

        //Gradual Controls
        private AnyFontTextView _instagramLikesSlowLabel;
        private AnyFontTextView _instagramLikesNormalLabel;
        private AnyFontTextView _instagramLikesFastLabel;
        private AnyFontTextView _instagramLikesUltraFastLabel;

        //Show More Button
        private AnyFontTextView _instagramLikesShowMoreLabel;

        //Instant Control Panel
        private AnyFontTextView _instagramLikesInstantHeader;
        private AnyFontTextView _instagramLikesInstantInstantStartLabel;
        private AnyFontTextView _instagramLikesInstantInstantDeliveryLabel;
        private AnyFontTextView _instagramLikesInstantPermanentLabel;
        private AnyFontTextView _instagramLikesInstantRealLookingLabel;

        //Gradual Control Panel
        private AnyFontTextView _instagramLikesGradualHeader;
        private AnyFontTextView _instagramLikesGradualInstantStartLabel;
        private AnyFontTextView _instagramLikesGradualInstantDeliveryLabel;
        private AnyFontTextView _instagramLikesGradualPermanentLabel;
        private AnyFontTextView _instagramLikesGradualNormalLookingLabel;

        #endregion

        private LinearLayout _instagramLikesGradualControlPanel;
        private LinearLayout _showMoreLayout;

        private MvxRecyclerView _postsRecyclerView;
        private InstagramPostPreviewAdapter _postsAdapter;

        protected override int FragmentId => Resource.Layout.instagram_like_fragment;
        protected override void InitComponents(View fragmentView)
        {
            #region Labels
            //Head Control Panel
            _instagramLikesCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_counter);
            _instagramLikesInstantLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_label);
            _instagramLikesGradualLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_label);
            _instagramLikesDiscount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_discount);
            _instagramLikesIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_increment);
            _instagramLikesDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_decrement);
            //Gradual Controls
            _instagramLikesSlowLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_less_label);
            _instagramLikesNormalLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_n_label);
            _instagramLikesFastLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_more_label);
            _instagramLikesUltraFastLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_much_more_label);
            //Show More Button
            _instagramLikesShowMoreLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_show_more_label);
            //Instant Control Panel
            _instagramLikesInstantHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_header);
            _instagramLikesInstantInstantStartLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_instant_start_label);
            _instagramLikesInstantInstantDeliveryLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_instant_delivery_label);
            _instagramLikesInstantPermanentLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_permanent_label);
            _instagramLikesInstantRealLookingLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_instant_real_looking_label);

            //Gradual Control Panel
            _instagramLikesGradualHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_header);
            _instagramLikesGradualInstantStartLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_instant_start_label);
            _instagramLikesGradualInstantDeliveryLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_instant_delivery_label);
            _instagramLikesGradualPermanentLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_permanent_label);
            _instagramLikesGradualNormalLookingLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_likes_gradual_normal_looking_label);

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
            var bindingSet = this.CreateBindingSet<InstagramLikesFragment, InstagramLikeMenuItemViewModel>();

            #region Labels

            //Head Control Panel
            bindingSet.Bind(_instagramLikesInstantLabel).For(v => v.Text).To(vm => vm.InstantLabelText);
            bindingSet.Bind(_instagramLikesGradualLabel).For(v => v.Text).To(vm => vm.GradualLabelText);

            //Show More Button
            bindingSet.Bind(_instagramLikesShowMoreLabel).For(v => v.Text).To(vm => vm.ShowMoreLabelText);

            //Instant Control Panel
            bindingSet.Bind(_instagramLikesInstantHeader).For(v => v.Text).To(vm => vm.InstantLabelText);
            bindingSet.Bind(_instagramLikesInstantInstantStartLabel).For(v => v.Text).To(vm => vm.InstantStartLabelText);
            bindingSet.Bind(_instagramLikesInstantInstantDeliveryLabel).For(v => v.Text).To(vm => vm.InstantDeliveryLabelText);
            bindingSet.Bind(_instagramLikesInstantPermanentLabel).For(v => v.Text).To(vm => vm.PermanentLabelText);
            bindingSet.Bind(_instagramLikesInstantRealLookingLabel).For(v => v.Text).To(vm => vm.RealLookingLabelText);

            //Gradual Control Panel
            bindingSet.Bind(_instagramLikesGradualHeader).For(v => v.Text).To(vm => vm.GradualLabelText);
            bindingSet.Bind(_instagramLikesGradualInstantStartLabel).For(v => v.Text).To(vm => vm.InstantStartLabelText);
            bindingSet.Bind(_instagramLikesGradualInstantDeliveryLabel).For(v => v.Text).To(vm => vm.InstantDeliveryLabelText);
            bindingSet.Bind(_instagramLikesGradualPermanentLabel).For(v => v.Text).To(vm => vm.PermanentLabelText);
            bindingSet.Bind(_instagramLikesGradualNormalLookingLabel).For(v => v.Text).To(vm => vm.NormalLookingLabelText);

            #endregion

            bindingSet.Bind(_instagramLikesCounter).For(v => v.Text).To(vm => vm.Counter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_instagramLikesDiscount).For(v => v.Text).To(vm => vm.DiscountPercent).OneWay().WithConversion("Discount");
            bindingSet.Bind(_instagramLikesDiscount).For(v => v.Visibility).To(vm => vm.DiscountPercent).OneWay().WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DiscountPresent).To(vm => vm.DiscountPercent).WithConversion("IntToBool");

            bindingSet.Bind(this).For(v => v.IsInGradualMode).To(vm => vm.IsInGradualMode);
            bindingSet.Bind(_instagramLikesInstantLabel).For("Click").To(vm => vm.InstantModeSelectedCommand);
            bindingSet.Bind(_instagramLikesGradualLabel).For("Click").To(vm => vm.GradualModeSelectedCommand);
            bindingSet.Bind(_instagramLikesGradualControlPanel).For(v => v.Visibility).To(vm => vm.IsInGradualMode).WithConversion("GoneVisibility");

            bindingSet.Bind(_instagramLikesSlowLabel).For("Click").To(vm => vm.SetSlowModeCommand);
            bindingSet.Bind(_instagramLikesNormalLabel).For("Click").To(vm => vm.SetNormalModeCommand);
            bindingSet.Bind(_instagramLikesFastLabel).For("Click").To(vm => vm.SetFastModeCommand);
            bindingSet.Bind(_instagramLikesUltraFastLabel).For("Click").To(vm => vm.SetUltraFastModeCommand);
            bindingSet.Bind(_instagramLikesIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_instagramLikesDecrement).For("Click").To(vm => vm.DecrementCommand);
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

                SetColorToCounter(_instagramLikesCounter, _discountPresent);
            }
        }


        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                ToggleButtonState(_instagramLikesIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_instagramLikesDecrement, _decrementButtonActive);
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

                ChangeTextColor(_instagramLikesInstantLabel, !IsInGradualMode);
                ChangeTextColor(_instagramLikesGradualLabel, IsInGradualMode);
            }
        }

        private void VisualizeGradualMode()
        {
            AnyFontTextView viewToSetBackground = null;

            switch (_gradualMode)
            {
                case GradualSpeed.Slow:
                    viewToSetBackground = _instagramLikesSlowLabel;
                    break;
                case GradualSpeed.Normal:
                    viewToSetBackground = _instagramLikesNormalLabel;
                    break;
                case GradualSpeed.Fast:
                    viewToSetBackground = _instagramLikesFastLabel;
                    break;
                case GradualSpeed.UltraFast:
                    viewToSetBackground = _instagramLikesUltraFastLabel;
                    break;
            }

            viewToSetBackground?.SetBackgroundResource(Resource.Drawable.selected_gradual_mode_button_background);

            if (viewToSetBackground != _instagramLikesSlowLabel)
            {
                _instagramLikesSlowLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramLikesNormalLabel)
            {
                _instagramLikesNormalLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramLikesFastLabel)
            {
                _instagramLikesFastLabel.SetBackgroundResource(0);
            }

            if (viewToSetBackground != _instagramLikesUltraFastLabel)
            {
                _instagramLikesUltraFastLabel.SetBackgroundResource(0);
            }
        }
    }
}