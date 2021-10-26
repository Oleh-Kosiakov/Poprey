using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Controls;
using Poprey.Droid.Views;

namespace Poprey.Droid.ViewHolders
{
    public class MenuHeaderFragment : BaseFragment<MenuHeaderItem>
    {
        private AnyFontTextView _title;
        private View _underlineView;

        protected override int FragmentId => Resource.Layout.menu_header_template;

        protected override void InitComponents(View fragmentView)
        {
            _title = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.menuHeaderTitle);
            _underlineView = fragmentView.FindViewById<View>(Resource.Id.menu_header_item_underline);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<MenuHeaderFragment, MenuHeaderItem>();

            bindingSet.Bind(_title).For(v => v.Text).To(vm => vm.Title);

            bindingSet.Bind(this).For(v => v.IsActive).To(vm => vm.IsActive);
            bindingSet.Bind(_underlineView).For(v => v.Visibility).To(vm => vm.IsActive).WithConversion("Visibility");

            bindingSet.Apply();
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;

                ChangeTextColor(_title, _isActive);
            }
        }

        protected void ChangeTextColor(TextView label, bool isBlack)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                label.SetTextColor(
                    Resources.GetColor(isBlack ? Resource.Color.app_black : Resource.Color.colorPrimaryDark,
                        Context.Theme));
            }
            else
            {
                label.SetTextColor(
                    Resources.GetColor(isBlack ? Resource.Color.app_black : Resource.Color.colorPrimaryDark));
            }
        }

    }
}