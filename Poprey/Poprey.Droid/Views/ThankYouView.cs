using Android.App;
using Android.Content.PM;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Label = "Thank you",
        Theme = "@style/ThankYouTheme",
        Name = "poprey.droid.views.ThankYouView",
        ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class ThankYouView : BaseAppCompatActivity<ThankYouViewModel>
    {
        private ImageView _backArrow;

        private AnyFontTextView _header;
        private AnyFontTextView _subtitle;

        private AnyFontTextView _price;
        private AnyFontTextView _subprice;

        private AnyFontTextView _comeBack;
        private RelativeLayout _comeBackLayout;

        protected override int ContentViewId => Resource.Layout.ThankYouView;

        protected override void InitComponents()
        {
            _backArrow = FindViewById<ImageView>(Resource.Id.thank_you_back_arrow);
            _header = FindViewById<AnyFontTextView>(Resource.Id.thank_you_header);
            _subtitle = FindViewById<AnyFontTextView>(Resource.Id.thank_you_subtitle);

            _price = FindViewById<AnyFontTextView>(Resource.Id.thank_you_price);
            _subprice = FindViewById<AnyFontTextView>(Resource.Id.thank_you_subprice);

            _comeBack = FindViewById<AnyFontTextView>(Resource.Id.thank_you_come_back);
            _comeBackLayout = FindViewById<RelativeLayout>(Resource.Id.thank_you_come_back_layout);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<ThankYouView, ThankYouViewModel>();

            bindingSet.Bind(_header).For(v => v.Text).To(vm => vm.HeaderText);
            bindingSet.Bind(_subtitle).For(v => v.Text).To(vm => vm.SubheaderText);

            bindingSet.Bind(_price).For(v => v.Text).To(vm => vm.Price).WithConversion("BagSum");
            bindingSet.Bind(_subprice).For(v => v.Text).To(vm => vm.SubpriceText);

            bindingSet.Bind(_comeBack).For(v => v.Text).To(vm => vm.ComeBackText);

            bindingSet.Bind(_price).For(v => v.Visibility).To(vm => vm.PriceVisible).WithConversion("Visibility");
            bindingSet.Bind(_subprice).For(v => v.Visibility).To(vm => vm.SubpriceVisible).WithConversion("Visibility");

            bindingSet.Bind(_backArrow).For("Click").To(vm => vm.GoBackCommand);
            bindingSet.Bind(_comeBackLayout).For("Click").To(vm => vm.GoBackCommand);

            bindingSet.Apply();
        }
    }
}