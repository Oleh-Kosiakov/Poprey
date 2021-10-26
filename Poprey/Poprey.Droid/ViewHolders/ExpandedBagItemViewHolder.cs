using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Adapters;
using Poprey.Droid.Controls;

namespace Poprey.Droid.ViewHolders
{
    public class ExpandedBagItemViewHolder : MvxRecyclerViewHolder
    {
        public const int Identifier = 112;
        private Activity CurrentTopActivity => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

        private AnyFontTextView _serviceNameLabel;
        private AnyFontTextView _serviceCountLabel;
        private AnyFontTextView _servicePriceLabel;
        private ImageView _removeImageView;
        private View _separatorView;

        private MvxRecyclerView _postsListView;
        private InstagramPostPreviewAdapter _postsAdapter;

        private RelativeLayout _impressionsLayout;
        private AnyFontTextView _impressionsCountLabel;
        private AnyFontTextView _impressionsPriceLabel;

        public ExpandedBagItemViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }
        private void InitComponents(View itemView)
        {
            _serviceNameLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_service_name);
            _serviceCountLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_service_count);
            _servicePriceLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_price);
            _separatorView = itemView.FindViewById<View>(Resource.Id.bag_item_separator_view);
            _removeImageView = itemView.FindViewById<ImageView>(Resource.Id.bag_item_remove_image);

            _postsListView = itemView.FindViewById<MvxRecyclerView>(Resource.Id.bag_item_previews_list);

            _impressionsLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.bag_item_impressions_layout);
            _impressionsCountLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_impressions_count);
            _impressionsPriceLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.bag_item_impressions_price);

            _postsAdapter = new InstagramPostPreviewAdapter((IMvxAndroidBindingContext) BindingContext)
            {
                RequireCompactLayout = true
            };

            _postsListView.Adapter = _postsAdapter;
            var layoutManager = new LinearLayoutManager(CurrentTopActivity, LinearLayoutManager.Horizontal, false);
            _postsListView.SetLayoutManager(layoutManager);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<ExpandedBagItemViewHolder, BagItem>();

            bindingSet.Bind(_serviceNameLabel).For(v => v.Text).To(vm => vm.ServiceName);
            bindingSet.Bind(_serviceCountLabel).For(v => v.Text).To(vm => vm.TariffPlan).WithConversion("BagServiceCount");
            bindingSet.Bind(_servicePriceLabel).For(v => v.Text).To(vm => vm.ServicePrice).WithConversion("BagSum");

            bindingSet.Bind(_postsListView).For(v => v.ItemsSource).To(vm => vm.PostPreviews);
            bindingSet.Bind(_removeImageView).For("Click").To(vm => vm.RemoveCommand);

            bindingSet.Bind(_separatorView).For(v => v.Visibility).To(vm => vm.IsSeparatorVisible).WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IsImpressionsAdded).To(vm => vm.IsImpressionsAdded);
            bindingSet.Bind(_impressionsLayout).For("Click").To(vm => vm.ToggleAddedImpressionsLayout);
            bindingSet.Bind(_impressionsCountLabel).For(v => v.Text).To(vm => vm.ImpressionsCount).WithConversion("ImpressionsCount");
            bindingSet.Bind(_impressionsPriceLabel).For(v => v.Text).To(vm => vm.ImpressionPrice).WithConversion("ImpressionsPrice");

            bindingSet.Apply();
        }

        private bool _isImpressionsAdded;
        public bool IsImpressionsAdded
        {
            get => _isImpressionsAdded;
            set
            {
                _isImpressionsAdded = value;

                _impressionsLayout.SetBackgroundResource(_isImpressionsAdded ? Resource.Drawable.impression_added_background : Resource.Drawable.impression_not_added_background);
            }
        }
    }
}