using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using Poprey.Core.ViewModels;
using Poprey.Droid.Adapters;
using Poprey.Droid.Animations;
using Poprey.Droid.Controls;
using Poprey.Droid.Listeners;
using Poprey.Droid.Util;

namespace Poprey.Droid.Components
{
    public class BagView : BaseView
    {
        private int _screenHeightInPx;
        private float _collapsedBagHeight;

        private int MaxBagHeightForExpandedState => (int)(_screenHeightInPx * 0.5);
        private BagViewModelSingleton ViewModel => (BagViewModelSingleton)DataContext;

        //Collapsed bag
        private RelativeLayout _collapsedBagLayout;
        private AnyFontTextView _serviceNameLabel;
        private AnyFontTextView _sumLabel;
        private RelativeLayout _butButtonLayout;

        //Bag Footer
        private LinearLayout _footerLayout;
        private AnyFontTextView _footerTotalPriceLabel;
        private AnyFontTextView _footerSumLabel;
        private AnyFontTextView _footerDiscountLabel;
        private AnyFontTextView _footerTermsAndConditionsLabel;
        private AnyFontTextView _footerTermsAndConditionsUnderlinedLabel;

        private MvxRecyclerView _paymentMethodsList;
        private PaymentMethodsAdapter _paymentMethodsAdapter;

        //Bag List
        private MvxRecyclerView _bagItemsList;
        private BagItemsAdapter _bagItemsAdapter;

        //Bag Header
        private RelativeLayout _bagListLayout;
        private ImageView _drawerStateImage;
        private AdaptiveBackgroundImageView _crossImage;
        private ImageView _trashBinImage;

        //Partial Bag Header
        private RelativeLayout _partialBagHeaderLayout;
        private AdaptiveBackgroundImageView _partialCloseButton;

        public BagView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void Initialize(ViewGroup viewGroup, IAttributeSet attrs)
        {
            _screenHeightInPx = WindowUtil.GetFullScreenHeight(Context);

            var inflater = LayoutInflater.From(Context);
            var layout = inflater.Inflate(Resource.Layout.bag_view, this);

            var bindingContext = new MvxAndroidBindingContext(Context, (IMvxLayoutInflaterHolder)Context);

            //Collapsed bag
            _collapsedBagLayout = layout.FindViewById<RelativeLayout>(Resource.Id.bag_collapsed_layout);
            _serviceNameLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_service_name_label);
            _sumLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_sum_label);
            _butButtonLayout = layout.FindViewById<RelativeLayout>(Resource.Id.bag_buy_button_layout);

            //Bag Footer
            _footerLayout = layout.FindViewById<LinearLayout>(Resource.Id.bag_footer_layout);
            _footerTotalPriceLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_total_price_label);
            _footerSumLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_footer_sum_label);
            _footerDiscountLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_footer_discount_label);
            _footerTermsAndConditionsLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_footer_terms_and_conditions);
            _footerTermsAndConditionsUnderlinedLabel = layout.FindViewById<AnyFontTextView>(Resource.Id.bag_footer_terms_and_conditions_underlined);
            _paymentMethodsList = layout.FindViewById<MvxRecyclerView>(Resource.Id.bag_payment_methods_list);

            _footerTermsAndConditionsUnderlinedLabel.PaintFlags = _footerTermsAndConditionsUnderlinedLabel.PaintFlags | PaintFlags.UnderlineText;

            //Bag Header
            _bagListLayout = layout.FindViewById<RelativeLayout>(Resource.Id.bag_list_layout);
            _drawerStateImage = layout.FindViewById<ImageView>(Resource.Id.bag_drawer_state_image);
            _crossImage = layout.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.bag_cross_image);
            _trashBinImage = layout.FindViewById<ImageView>(Resource.Id.bag_trash_bin_image);

            //Partial Bag Header
            _partialBagHeaderLayout = layout.FindViewById<RelativeLayout>(Resource.Id.partial_bag_header_layout);
            _partialCloseButton = layout.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.bag_partial_header_cross_image);

            //Bag List
            _bagItemsList = layout.FindViewById<MvxRecyclerView>(Resource.Id.bag_bought_items_list);

            _paymentMethodsAdapter = new PaymentMethodsAdapter(bindingContext);
            _paymentMethodsList.Adapter = _paymentMethodsAdapter;
            var paymentsLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
            _paymentMethodsList.SetLayoutManager(paymentsLayoutManager);

            _bagItemsAdapter = new BagItemsAdapter(bindingContext);
            _bagItemsList.Adapter = _bagItemsAdapter;
            var bagItemsLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false);
            _bagItemsList.SetLayoutManager(bagItemsLayoutManager);
            var onSizeChangedListener = new RecyclerViewSizeListener(OnHeightChangedAction);
            _bagItemsList.AddOnLayoutChangeListener(onSizeChangedListener);

            _collapsedBagHeight = Context.Resources.GetDimension(Resource.Dimension.bag_collapsed_height);

            _collapsedBagLayout.SetOnTouchListener(new SwipeTouchListener(onSwipeTop: OnSwipeTop));
            _footerLayout.SetOnTouchListener(new SwipeTouchListener(onSwipeBottom: OnSwipeBottom));
            _partialBagHeaderLayout.SetOnTouchListener(new SwipeTouchListener(onSwipeBottom: OnSwipeBottom));
            _bagListLayout.SetOnTouchListener(new SwipeTouchListener(onSwipeBottom: OnSwipeBottom));
        }

        private void OnSwipeBottom()
        {
            ViewModel.CloseCommand.Execute();
        }

        private void OnSwipeTop()
        {
            ViewModel.BagFooterPressedCommand.Execute();
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<BagView, BagViewModelSingleton>();

            bindingSet.Bind(this).For(v => v.Visibility).To(vm => vm.IsVisible).WithConversion("GoneVisibility");
            bindingSet.Bind(this).For(v => v.BagState).To(vm => vm.CurrentBagState);

            //Collapsed bag
            bindingSet.Bind(_serviceNameLabel).For(v => v.Text).To(vm => vm.ServiceName);
            bindingSet.Bind(_sumLabel).For(v => v.Text).To(vm => vm.BagSum).WithConversion("BagSum");
            bindingSet.Bind(_butButtonLayout).For("Click").To(vm => vm.BagPressedCommand);
            bindingSet.Bind(_collapsedBagLayout).For("Click").To(vm => vm.BagFooterPressedCommand);

            //Bag Footer
            bindingSet.Bind(_footerTotalPriceLabel).For(v => v.Text).To(vm => vm.TotalPriceLabelText);
            bindingSet.Bind(_footerSumLabel).For(v => v.Text).To(vm => vm.BagSum).WithConversion("BagSum");
            bindingSet.Bind(_footerDiscountLabel).For(v => v.Text).To(vm => vm.DiscountPercents).WithConversion("Discount");
            bindingSet.Bind(_footerTermsAndConditionsLabel).For(v => v.Text).To(vm => vm.FooterTermsAndConditionsText);
            bindingSet.Bind(_footerTermsAndConditionsLabel).For("Click").To(vm => vm.ShowLicenseCommand);
            bindingSet.Bind(_footerTermsAndConditionsUnderlinedLabel).For(v => v.Text).To(vm => vm.FooterTermsAndConditionsUnderlinedText);
            bindingSet.Bind(_footerTermsAndConditionsUnderlinedLabel).For("Click").To(vm => vm.ShowLicenseCommand);
            bindingSet.Bind(_paymentMethodsAdapter).For(v => v.ItemsSource).To(vm => vm.PaymentMethods);
            bindingSet.Bind(_paymentMethodsAdapter).For(v => v.ItemClick).To(vm => vm.PaymentMethodSelectedCommand);

            //Bag List Headers
            bindingSet.Bind(_drawerStateImage).For("Click").To(vm => vm.ToggleDrawerCommand);
            bindingSet.Bind(_crossImage).For("Click").To(vm => vm.CloseCommand);
            bindingSet.Bind(_trashBinImage).For("Click").To(vm => vm.EmptyBagCommand);
            bindingSet.Bind(_partialCloseButton).For("Click").To(vm => vm.CloseCommand);

            //Bag list
            bindingSet.Bind(_bagItemsAdapter).For(v => v.ItemsSource).To(vm => vm.BagItems);

            bindingSet.Apply();
        }

        private BagViewModelSingleton.BagState _bagState;

        public BagViewModelSingleton.BagState BagState
        {
            get => _bagState;
            set
            {
                _bagState = value;

                Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity.RunOnUiThread(async () =>
                {
                    switch (_bagState)
                    {
                        case BagViewModelSingleton.BagState.Collapsed:
                            _collapsedBagLayout.Visibility = ViewStates.Visible;
                            await BagExpandCollapseAnimator.Collapse(this);
                            await Task.Delay(100);
                            _footerLayout.Visibility = ViewStates.Gone;
                            _bagListLayout.Visibility = ViewStates.Gone;
                            _partialBagHeaderLayout.Visibility = ViewStates.Gone;
                            _bagItemsList.Visibility = ViewStates.Gone;
                            this.LayoutParameters.Height = (int) _collapsedBagHeight;
                            break;
                        case BagViewModelSingleton.BagState.Expanded:
                            _bagListLayout.Visibility = ViewStates.Gone;
                            _partialBagHeaderLayout.Visibility = ViewStates.Visible;
                            _footerLayout.Visibility = ViewStates.Visible;
                            _bagItemsList.Visibility = ViewStates.Visible;
                            await BagExpandCollapseAnimator.Expand(this);

                            break;
                        case BagViewModelSingleton.BagState.Fullscreen:
                            _collapsedBagLayout.Visibility = ViewStates.Gone;
                            _partialBagHeaderLayout.Visibility = ViewStates.Gone;
                            _footerLayout.Visibility = ViewStates.Visible;
                            _bagListLayout.Visibility = ViewStates.Visible;
                            _bagItemsList.Visibility = ViewStates.Visible;
                            this.LayoutParameters.Height = _screenHeightInPx - 50;
                            break;
                    }
                });
            }
        }

        private async void OnHeightChangedAction(int newListHeight)
        {
            if (newListHeight == 0)
            {
                await Task.Delay(50);
                ViewModel.CloseCommand.Execute();
                return;
            }

            if (newListHeight > MaxBagHeightForExpandedState)
            {
                ViewModel.RecommendedBigBagState = BagViewModelSingleton.BagState.Fullscreen;
            }
            else
            {
                ViewModel.RecommendedBigBagState = BagViewModelSingleton.BagState.Expanded;
            }

            ViewModel.InvalidateBigState();
        }
    }
}