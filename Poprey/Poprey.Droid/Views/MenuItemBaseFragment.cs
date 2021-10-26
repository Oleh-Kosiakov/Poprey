using Android.OS;
using Android.Widget;
using MvvmCross.ViewModels;

namespace Poprey.Droid.Views
{
    public abstract class MenuItemBaseFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : MvxViewModel
    {
        protected const int SpanCount = 3;

        protected void ChangeTextColor(TextView label, bool isBlack)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                label.SetTextColor(Resources.GetColor(isBlack ? Resource.Color.app_black : Resource.Color.colorPrimaryDark, Context.Theme));
            }
            else
            {
                label.SetTextColor(Resources.GetColor(isBlack ? Resource.Color.app_black : Resource.Color.colorPrimaryDark));
            }
        }

        protected void ToggleButtonState(TextView textView, bool isActive)
        {
            textView.SetBackgroundResource(isActive ? Resource.Drawable.increment_decrement_background : Resource.Drawable.inactive_increment_decrement_background);
        }

        protected void SetColorToCounter(TextView textView, bool discountPresent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                textView.SetTextColor(Resources.GetColor(discountPresent ? Resource.Color.instagram_discount_color : Resource.Color.app_black, Context.Theme));
            }
            else
            {
                textView.SetTextColor(Resources.GetColor(discountPresent ? Resource.Color.instagram_discount_color : Resource.Color.app_black));
            }
        }
    }
}