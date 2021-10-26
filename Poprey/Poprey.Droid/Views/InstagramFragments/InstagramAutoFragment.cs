using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.ViewModels.InstagramMenuItems;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;

namespace Poprey.Droid.Views.InstagramFragments
{
    public class InstagramAutoFragment : MenuItemBaseFragment<InstagramAutoMenuItemViewModel>
    {
        private AnyFontTextView _instagramAutoPerpostLabel;
        private AnyFontTextView _instagramAutoSubscriptionLabel;
        private AnyFontTextView _instagramAutoOverallCounter;
        private AnyFontTextView _instagramAutoOverallDiscount;

        private AnyFontTextView _instagramAutoOverallIncrementCount;
        private AnyFontTextView _instagramAutoOverallIncrement;
        private AnyFontTextView _instagramAutoOverallsDecrement;

        private RelativeLayout _instagramAutoForNewPostsLayout;
        private AnyFontTextView _instagramAutoNewpostsCounter;
        private AnyFontTextView _instagramAutoNewpostsLabel;
        private ExtendedSeekBar _newpostsSeekBar;

        protected override int FragmentId => Resource.Layout.instagram_auto_fragment;
        protected override void InitComponents(View fragmentView)
        {
            _instagramAutoPerpostLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_perpost_label);
            _instagramAutoSubscriptionLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_subscription_label);
            _instagramAutoOverallCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_overall_counter);
            _instagramAutoOverallDiscount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_overall_discount);
            _instagramAutoOverallIncrementCount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_overall_increment_count);
            _instagramAutoOverallIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_overall_increment);
            _instagramAutoOverallsDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_overall_decrement);

            _instagramAutoForNewPostsLayout = fragmentView.FindViewById<RelativeLayout>(Resource.Id.instagram_auto_for_new_posts_layout);
            _instagramAutoNewpostsCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_newposts_counter);
            _instagramAutoNewpostsLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_auto_newposts_label);
            _newpostsSeekBar = fragmentView.FindViewById<ExtendedSeekBar>(Resource.Id.instagram_auto_newposts_seek_bar);

            _newpostsSeekBar.ProgressChanged += OnNewPostsCountChanged;
        }


        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<InstagramAutoFragment, InstagramAutoMenuItemViewModel>();

            bindingSet.Bind(_instagramAutoPerpostLabel).For(v => v.Text).To(vm => vm.PerpostLabelText);
            bindingSet.Bind(_instagramAutoSubscriptionLabel).For(v => v.Text).To(vm => vm.SubscriptionLabelText);
            bindingSet.Bind(_instagramAutoOverallCounter).For(v => v.Text).To(vm => vm.OverallCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_instagramAutoOverallDiscount).For(v => v.Text).To(vm => vm.DiscountPercent).OneWay().WithConversion("Discount");
            bindingSet.Bind(_instagramAutoOverallDiscount).For(v => v.Visibility).To(vm => vm.DiscountPercent).OneWay().WithConversion("Visibility");

            bindingSet.Bind(_instagramAutoOverallIncrementCount).For(v => v.Text).To(vm => vm.IncrementCount).OneWay().WithConversion("ServiceIncrement");
            bindingSet.Bind(_instagramAutoOverallIncrementCount).For(v => v.Visibility).To(vm => vm.IncrementCount).OneWay().WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DiscountPresent).To(vm => vm.DiscountPercent).WithConversion("IntToBool");

            bindingSet.Bind(this).For(v => v.IsInSubscriptionMode).To(vm => vm.IsInSubscriptionMode);
            bindingSet.Bind(_instagramAutoPerpostLabel).For("Click").To(vm => vm.PerpostModeSelectedCommand);
            bindingSet.Bind(_instagramAutoSubscriptionLabel).For("Click").To(vm => vm.SubscriptionModeSelectedCommand);
            bindingSet.Bind(_instagramAutoOverallIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_instagramAutoOverallsDecrement).For("Click").To(vm => vm.DecrementCommand);
            bindingSet.Bind(_instagramAutoOverallIncrementCount).For("Click").To(vm => vm.IncrementCommand);

            bindingSet.Bind(_instagramAutoNewpostsLabel).For(v => v.Text).To(vm => vm.NewPostsLabelText);
            bindingSet.Bind(_instagramAutoForNewPostsLayout).For(v => v.Visibility).To(vm => vm.IsInPerPostMode).WithConversion("Visibility");
            bindingSet.Bind(_instagramAutoNewpostsCounter).For(v => v.Text).To(vm => vm.NewPostsCount).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_newpostsSeekBar).For(v => v.Max).To(vm => vm.NewPostsLimit);
            bindingSet.Bind(_newpostsSeekBar).For(v => v.Progress).To(vm => vm.NewPostsCount);

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

                SetColorToCounter(_instagramAutoOverallCounter, _discountPresent);
            }
        }


        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                ToggleButtonState(_instagramAutoOverallIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_instagramAutoOverallsDecrement, _decrementButtonActive);
            }
        }

        private bool _isInSubscriptionMode;
        public bool IsInSubscriptionMode
        {
            get => _isInSubscriptionMode;
            set
            {
                _isInSubscriptionMode = value;

                ChangeTextColor(_instagramAutoPerpostLabel, !IsInSubscriptionMode);
                ChangeTextColor(_instagramAutoSubscriptionLabel, IsInSubscriptionMode);
            }
        }
        
        private void OnNewPostsCountChanged(object sender, ExtendedSeekBarProgressChangedEventArgs e)
        {
            ViewModel.NewPostsCount = e.Progress;
        }
    }
}