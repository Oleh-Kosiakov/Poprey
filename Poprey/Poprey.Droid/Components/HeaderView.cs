using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.UI;
using Poprey.Core.ViewModels;
using Poprey.Droid.Adapters;
using Poprey.Droid.Util;

namespace Poprey.Droid.Components
{
    public class HeaderView : BaseView
    {
        private const int OverlapValueInDp = 15;
        private const int AdditionalOffsetPerItemInDp = 5;
        private const int WeightSumForInstagram = 2;
        private const int WeightSumForOtherServices = 4;
        private const int WeightForSelectedService = 2;
        private const int WeightForUnselectedService = 1;

        private HeaderViewModelSingleton ViewModel => (HeaderViewModelSingleton)DataContext;

        //Popup and drawer
        private View _headerWidhtRangeBackground;
        private RelativeLayout _popupRelativeLayout;
        private ImageView _drawerStateImageView;
        private ImageView _popupDrawerStateImageView;
        private TextView _internetMessageTextView;

        //Header items

        private LinearLayout _mainContainer;

        private AdaptiveBackgroundImageView _instagramIcon;
        private HeaderInstagramAdapter _headerInstagramAdapter;
        private MvxRecyclerView _instagramAccountsRecyclerView;
        private AdaptiveBackgroundImageView _tikTokIcon;
        private AdaptiveBackgroundImageView _hashtagIcon;

        private HorizontalOverlapDecoration _horizontalOverlapDecoration;

        public HeaderView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void Initialize(ViewGroup viewGroup, IAttributeSet attrs)
        {
            var inflater = LayoutInflater.From(Context);
            var layout = inflater.Inflate(Resource.Layout.custom_header, this);

            var context = new MvxAndroidBindingContext(Context, (IMvxLayoutInflaterHolder)Context);

            //Popup and drawer
            _headerWidhtRangeBackground = layout.FindViewById<View>(Resource.Id.header_width_range_background);
            _popupRelativeLayout = layout.FindViewById<RelativeLayout>(Resource.Id.popup_relative_layout);
            _drawerStateImageView = layout.FindViewById<ImageView>(Resource.Id.drawer_state_image);
            _popupDrawerStateImageView = layout.FindViewById<ImageView>(Resource.Id.popup_drawer_state_image);
            _internetMessageTextView = layout.FindViewById<TextView>(Resource.Id.popup_header_text);

            //Header items
            _mainContainer = layout.FindViewById<LinearLayout>(Resource.Id.header_main_container);
            _instagramIcon = layout.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.instagram_header_item);
            _instagramAccountsRecyclerView = layout.FindViewById<MvxRecyclerView>(Resource.Id.instagram_accounts_list);
            _tikTokIcon = layout.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.tiktok_header_icon);
            _hashtagIcon = layout.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.hashtag_header_icon);

            _horizontalOverlapDecoration = new HorizontalOverlapDecoration
            {
                BasicHorizontalOverlap = DpConverter.ConvertDpToPx(OverlapValueInDp, Context.Resources)
            };

            var layoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);

            _headerInstagramAdapter = new HeaderInstagramAdapter(context);

            _instagramAccountsRecyclerView.AddItemDecoration(_horizontalOverlapDecoration);
            _instagramAccountsRecyclerView.SetLayoutManager(layoutManager);
            _instagramAccountsRecyclerView.Adapter = _headerInstagramAdapter;

            _instagramAccountsRecyclerView.Touch += InstagramAccountsRecyclerViewOnClick;
        }

        private void InstagramAccountsRecyclerViewOnClick(object sender, TouchEventArgs eventArgs)
        {
            if (eventArgs.Event.Action == MotionEventActions.Up)
            {
                ViewModel.ShowAuthenticationCommand.Execute();
            }
        }


        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<HeaderView, HeaderViewModelSingleton>();

            bindingSet.Bind(this).For(v => v.BackgroundWidth).To(vm => vm.BackgroundOverlayWidthInNativeUnits);
            bindingSet.Bind(this).For(v => v.IsCollapsed).To(vm => vm.IsCollapsed);
            //DrawerIcon
            bindingSet.Bind(this).For(v => v.IsDrawerMenuExpanded).To(vm => vm.IsDrawerMenuExpanded);
            bindingSet.Bind(_popupDrawerStateImageView).For("Click").To(vm => vm.ToggleDrawerCommand);
            bindingSet.Bind(_drawerStateImageView).For(v => v.Visibility).To(vm => vm.IsPopupVisible).WithConversion("InvertedGoneVisibility");
            bindingSet.Bind(_drawerStateImageView).For("Click").To(vm => vm.ToggleDrawerCommand);

            //No Internet Popup
            bindingSet.Bind(_popupRelativeLayout).For(v => v.Visibility).To(vm => vm.IsPopupVisible).WithConversion("GoneVisibility");
            bindingSet.Bind(this).For(v => v.PopupBackgroundColor).To(vm => vm.PopupBackgroundColor).OneWay();
            bindingSet.Bind(_internetMessageTextView).For(v => v.Text).To(vm => vm.PopupText).OneWay();
            bindingSet.Bind(this).For(v => v.IsErrorImage).To(vm => vm.IsErrorImage).OneWay();

            //Services Headers
            bindingSet.Bind(this).For(v => v.InstagramSelected).To(vm => vm.SelectedInstagramPage);
            bindingSet.Bind(this).For(v => v.TikTokSelected).To(vm => vm.SelectedTikTokPage);
            bindingSet.Bind(this).For(v => v.HashtagSelected).To(vm => vm.SelectedHashtagPage);

            bindingSet.Bind(_instagramIcon).For("Click").To(vm => vm.ShowInstagramCommand);
            bindingSet.Bind(_tikTokIcon).For("Click").To(vm => vm.ShowTikTokCommand);
            bindingSet.Bind(_hashtagIcon).For("Click").To(vm => vm.ShowHastagCommand);

            bindingSet.Bind(_headerInstagramAdapter).For(v => v.ItemsSource).To(vm => vm.InstagramAccounts);
            bindingSet.Bind(_horizontalOverlapDecoration).For(v => v.ItemCount).To(vm => vm.InstagramAccountsCount);

            bindingSet.Apply();
        }

        private bool _isCollapsed;
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set
            {
                if(_isCollapsed == value)
                    return;

                _isCollapsed = value;

                if (_isCollapsed)
                {
                    this.LayoutParameters.Height = 0;
                }
                else
                {
                    this.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
                }

                RequestLayout();
            }
        }


        private bool _hashtagSelected;

        public bool HashtagSelected
        {
            get => _hashtagSelected;
            set
            {
                _hashtagSelected = value;

                ChangeIconState(_hashtagIcon, value);
            }
        }

        public int BackgroundWidth
        {
            get => _headerWidhtRangeBackground.LayoutParameters.Width;
            set
            {
                _headerWidhtRangeBackground.LayoutParameters.Width = value;

                _headerWidhtRangeBackground. RequestLayout();
            }
        }

        private bool _tikTokSelected;

        public bool TikTokSelected
        {
            get => _tikTokSelected;
            set
            {
                _tikTokSelected = value;

                ChangeIconState(_tikTokIcon, value);
            }
        }

        private bool _instagramSelected;

        public bool InstagramSelected
        {
            get => _instagramSelected;
            set
            {
                _instagramSelected = value;

                if (_instagramSelected)
                {
                    _instagramIcon.Visibility = ViewStates.Gone;
                    _instagramAccountsRecyclerView.Visibility = ViewStates.Visible;

                    _mainContainer.WeightSum = WeightSumForInstagram;
                    ((LinearLayout.LayoutParams) _tikTokIcon.LayoutParameters).Weight = WeightForUnselectedService;
                    ((LinearLayout.LayoutParams)_hashtagIcon.LayoutParameters).Weight = WeightForUnselectedService;
                }
                else
                {
                    _instagramAccountsRecyclerView.Visibility = ViewStates.Gone;
                    _instagramIcon.Visibility = ViewStates.Visible;

                    _mainContainer.WeightSum = WeightSumForOtherServices;
                    ((LinearLayout.LayoutParams)_tikTokIcon.LayoutParameters).Weight = WeightForUnselectedService;
                    ((LinearLayout.LayoutParams)_hashtagIcon.LayoutParameters).Weight = WeightForUnselectedService;
                }
                RequestLayout();
            }
        }

        private void ChangeIconState(AdaptiveBackgroundImageView icon, bool selected)
        {
            var linearLayoutParams = icon.LayoutParameters as LinearLayout.LayoutParams;
            linearLayoutParams.Weight = selected ? WeightForSelectedService : WeightForUnselectedService;

            icon.RequestLayout();
        }


        #region DrawerIcon

        private bool _isDrawerMenuExpanded;
        public bool IsDrawerMenuExpanded
        {
            get => _isDrawerMenuExpanded;
            set
            {
                _isDrawerMenuExpanded = value;
                if (_isDrawerMenuExpanded)
                {
                    _drawerStateImageView.SetImageResource(Resource.Drawable.cross_dark);
                    _popupDrawerStateImageView.SetImageResource(Resource.Drawable.cross);
                }
                else
                {
                    _drawerStateImageView.SetImageResource(Resource.Drawable.drawer_collapsed);
                    _popupDrawerStateImageView.SetImageResource(Resource.Drawable.drawer_collapsed_white);
                }
            }
        }


        #endregion

        #region NoInternetPopup
        public MvxColor PopupBackgroundColor
        {
            get => default(MvxColor);
            set
            {
                if (value != null)
                {
                    _popupRelativeLayout.SetBackgroundColor(new Color(value.R, value.G, value.B));
                }
            }
        }

        private bool _isErrorImage;
        public bool IsErrorImage
        {
            get => _isErrorImage;
            set
            {
                _isErrorImage = value;

                if (_isErrorImage)
                {
                    _internetMessageTextView.SetCompoundDrawablesWithIntrinsicBounds(Context.GetDrawable(Resource.Drawable.cross_white_small), null, null, null);
                }
                else
                {
                    _internetMessageTextView.SetCompoundDrawablesWithIntrinsicBounds(Context.GetDrawable(Resource.Drawable.tick), null, null, null);
                }
            }
        }

        #endregion

        private class HorizontalOverlapDecoration : RecyclerView.ItemDecoration
        {
            public int BasicHorizontalOverlap { get; set; }

            public int ItemCount { get; set; }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                var itemPosition = parent.GetChildAdapterPosition(view);

                if (itemPosition == 0)
                    return;

                var additionalOffcet = ItemCount * DpConverter.ConvertDpToPx(AdditionalOffsetPerItemInDp, parent.Context.Resources);

                outRect.Set((BasicHorizontalOverlap + additionalOffcet) * -1, 0, 0, 0);
            }
        }
    }
}