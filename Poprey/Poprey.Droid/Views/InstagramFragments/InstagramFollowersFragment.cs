using Android.Views;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.ViewModels.InstagramMenuItems;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views.InstagramFragments
{
    public class InstagramFollowersFragment : MenuItemBaseFragment<InstagramFollowersMenuItemViewModel>
    {
        private AnyFontTextView _instagramFollowersCounter;
        private AnyFontTextView _instagramFollowersDiscount;

        private AnyFontTextView _instagramFollowersIncrement;
        private AnyFontTextView _instagramFollowersDecrement;

        private AnyFontTextView _instagramFollowersGetTenFreeLabel;
        private AnyFontTextView _instagramFollowersNormalLabel;
        private AnyFontTextView _instagramFollowersPremiumLabel;

        protected override int FragmentId => Resource.Layout.instagram_followers_fragment;

        protected override void InitComponents(View fragmentView)
        {
            _instagramFollowersCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_counter);
            _instagramFollowersDiscount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_discount);
            _instagramFollowersIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_increment);
            _instagramFollowersDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_decrement);

            _instagramFollowersGetTenFreeLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_get_ten_free_label);
            _instagramFollowersNormalLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_normal_label);
            _instagramFollowersPremiumLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.instagram_followers_premium_label);
        }


        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<InstagramFollowersFragment, InstagramFollowersMenuItemViewModel>();

            bindingSet.Bind(_instagramFollowersCounter).For(v => v.Text).To(vm => vm.FollowersCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_instagramFollowersDiscount).For(v => v.Text).To(vm => vm.DiscountPercent).OneWay().WithConversion("Discount");
            bindingSet.Bind(_instagramFollowersDiscount).For(v => v.Visibility).To(vm => vm.DiscountPercent).OneWay().WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();

            bindingSet.Bind(_instagramFollowersGetTenFreeLabel).For(v => v.Text).To(vm => vm.GetTenFreeLabelText);
            bindingSet.Bind(_instagramFollowersNormalLabel).For(v => v.Text).To(vm => vm.NormalLabelText);
            bindingSet.Bind(_instagramFollowersPremiumLabel).For(v => v.Text).To(vm => vm.PremiumLabelText);

            bindingSet.Bind(this).For(v => v.IsInPremiumMode).To(vm => vm.IsInPremiumMode);
            bindingSet.Bind(this).For(v => v.TenFreeFollowersAvailable).To(vm => vm.TenFreeFollowersAvailable);
            bindingSet.Bind(this).For(v => v.DiscountPresent).To(vm => vm.DiscountPercent).WithConversion("IntToBool");

            bindingSet.Bind(_instagramFollowersNormalLabel).For("Click").To(vm => vm.NormalModeSelectedCommand);
            bindingSet.Bind(_instagramFollowersPremiumLabel).For("Click").To(vm => vm.PremiumModeSelectedCommand);
            bindingSet.Bind(_instagramFollowersGetTenFreeLabel).For("Click").To(vm => vm.GetTenFreeCommand);
            bindingSet.Bind(_instagramFollowersIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_instagramFollowersDecrement).For("Click").To(vm => vm.DecrementCommand);

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

                SetColorToCounter(_instagramFollowersCounter, _discountPresent);
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

        private bool _isInPremiumMode;
        public bool IsInPremiumMode
        {
            get => _isInPremiumMode;
            set
            {
                _isInPremiumMode = value;

                ChangeTextColor(_instagramFollowersNormalLabel, !IsInPremiumMode);
                ChangeTextColor(_instagramFollowersPremiumLabel, IsInPremiumMode);
            }
        }

        private bool _tenFreeFollowersAvailable;
        public bool TenFreeFollowersAvailable
        {
            get => _tenFreeFollowersAvailable;
            set
            {
                _tenFreeFollowersAvailable = value;

                ChangeTextColor(_instagramFollowersGetTenFreeLabel, TenFreeFollowersAvailable);
            }
        }
    }
}