using Android.App;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.ViewHolders
{
    public class CompactBagItemViewHolder : MvxRecyclerViewHolder
    {
        public const int Identifier = 111;
        private Activity CurrentTopActivity => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

        private RelativeLayout _tariffDetailsLayout;
        private AnyFontTextView _serviceNameLabel;
        private AnyFontTextView _serviceCountLabel;
        private AnyFontTextView _servicePriceLabel;
        private AnyFontTextView _normalLabel;
        private AnyFontTextView _alternativeLabel;
        private ImageView _topRemoveImageView;
        private ImageView _removeImageView;
        private View _separatorView;

        public CompactBagItemViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _serviceNameLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_service_name);
            _serviceCountLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_service_count);
            _servicePriceLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_price);
            _normalLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_normal_label);
            _alternativeLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_alternative_label);
            _topRemoveImageView = itemView.FindViewById<ImageView>(Resource.Id.bag_item_top_remove_image);
            _removeImageView = itemView.FindViewById<ImageView>(Resource.Id.bag_item_remove_image);
            _separatorView = itemView.FindViewById<View>(Resource.Id.bag_item_separator_view);
            _tariffDetailsLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.bag_item_tariff_details_layout);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<CompactBagItemViewHolder, BagItem>();

            bindingSet.Bind(_serviceNameLabel).For(v => v.Text).To(vm => vm.ServiceName);
            bindingSet.Bind(_serviceCountLabel).For(v => v.Text).To(vm => vm.TariffPlan).WithConversion("BagServiceCount");
            bindingSet.Bind(_servicePriceLabel).For(v => v.Text).To(vm => vm.ServicePrice).WithConversion("BagSum");

            bindingSet.Bind(_normalLabel).For(v => v.Text).To(vm => vm.NormalLabelText);
            bindingSet.Bind(_alternativeLabel).For(v => v.Text).To(vm => vm.AlternativeLabelText);
            bindingSet.Bind(_normalLabel).For("Click").To(vm => vm.NormalModeSelectedCommand);
            bindingSet.Bind(_alternativeLabel).For("Click").To(vm => vm.AlternativeModeSelectedCommand);

            bindingSet.Bind(_topRemoveImageView).For(v => v.Visibility).To(vm => vm.OptionsVisibleInTheCompactMode).WithConversion("InvertedGoneVisibility");
            bindingSet.Bind(_tariffDetailsLayout).For(v=>v.Visibility).To(vm => vm.OptionsVisibleInTheCompactMode).WithConversion("Visibility");
            bindingSet.Bind(_separatorView).For(v => v.Visibility).To(vm => vm.IsSeparatorVisible).WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IsInAlternativeMode).To(vm => vm.IsInAlternativeMode);

            bindingSet.Bind(_removeImageView).For("Click").To(vm => vm.RemoveCommand);
            bindingSet.Bind(_topRemoveImageView).For("Click").To(vm => vm.RemoveCommand);

            bindingSet.Apply();
        }

        private bool _isInAlternativeMode;
        public bool IsInAlternativeMode
        {
            get => _isInAlternativeMode;
            set
            {
                _isInAlternativeMode = value;

                _normalLabel.SetTextColor(CurrentTopActivity.Resources.GetColor(_isInAlternativeMode
                            ? Resource.Color.bag_discount_label_color
                            : Resource.Color.colorPrimaryDark, CurrentTopActivity.Theme));
                _alternativeLabel.SetTextColor(CurrentTopActivity.Resources.GetColor(_isInAlternativeMode 
                            ? Resource.Color.colorPrimaryDark
                            : Resource.Color.bag_discount_label_color, CurrentTopActivity.Theme));
            }
        }
    }
}