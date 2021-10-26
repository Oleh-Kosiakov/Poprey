using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading.Cross;
using Poprey.Droid.Controls;
using Poprey.Droid.Util;

namespace Poprey.Droid.Components
{
    public class AdaptiveBackgroundImageView : BaseView
    {
        private const float CollapsedWidthInDp = 50;
        private const float ExpandedWidthInDp = 100;

        private ImageView _internalImage;
        private MvxCircleCachedImageView _internalLoadableImage;
        private GradientDrawable _backgroundDrawable;

        private const string PlaceholderPath = "avatar_placeholder.png";

        public AdaptiveBackgroundImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        private bool IsExpanded
        {
            get => this.LayoutParameters.Width == DpConverter.ConvertDpToPx(ExpandedWidthInDp, Context.Resources);
            set
            {
                if (value)
                {
                    this.LayoutParameters.Width = (int)ExpandedWidthInDp;
                }
                else
                {
                    this.LayoutParameters.Width = (int)CollapsedWidthInDp;
                }

                RequestLayout();
            }
        }

        public string ImageSrc
        {
            get => _internalLoadableImage.ImagePath;
            set
            {
                if (_internalLoadableImage.ImagePath == value || (_internalLoadableImage.ImagePath != null && value == null))
                {
                    return;
                }

                _internalLoadableImage.ImagePath = value;
            }
        }

        private bool _shouldLoadFromWeb;
        public bool ShouldLoadImageFromWeb
        {
            get => _shouldLoadFromWeb;
            set
            {
                _shouldLoadFromWeb = value;

                ToggleBigImageVisibility();
            }
        }

        private bool _shouldShowGrayBackground;
        public bool ShouldShowGrayBackground
        {
            get => _shouldShowGrayBackground;
            set
            {
                _shouldShowGrayBackground = value;

                if (_shouldShowGrayBackground)
                {
                    _backgroundDrawable.SetColor(ContextCompat.GetColor(Context,Resource.Color.gray));
                }
            }
        }

        private bool _shouldShowAddImage;
        public bool ShouldShowAddImage
        {
            get => _shouldShowAddImage;
            set
            {
                _shouldShowAddImage = value;

                if (_shouldShowAddImage)
                {
                    _internalImage.SetImageResource(Resource.Drawable.cross_add);
                }
            }
        }

        protected override void Initialize(ViewGroup viewGroup, IAttributeSet attr)
        {
            var inflater = LayoutInflater.From(Context);
            var layout = inflater.Inflate(Resource.Layout.adaptive_background_image_view, this);

            _internalImage = layout.FindViewById<ImageView>(Resource.Id.internal_image);
            _internalLoadableImage = layout.FindViewById<MvxCircleCachedImageView>(Resource.Id.internal_image_loadable);

            _internalLoadableImage.LoadingPlaceholderImagePath = PlaceholderPath;

            var imageId = attr.GetAttributeResourceValue("http://schemas.android.com/apk/res-auto", "src", -1);
            var backgroundColorId = attr.GetAttributeResourceValue("http://schemas.android.com/apk/res-auto", "backgroundColorResource", -1);
            var loadFromUrl = attr.GetAttributeBooleanValue("http://schemas.android.com/apk/res-auto", "loadFromUrl", false);

            if (imageId != -1)
            {
                _internalImage.SetImageResource(imageId);
            }

            _backgroundDrawable = new GradientDrawable();
            _backgroundDrawable.SetShape(ShapeType.Rectangle);
            _backgroundDrawable.SetCornerRadius(DpConverter.ConvertDpToPx(50, Context.Resources));

            if (backgroundColorId != -1)
            {
                var color = ContextCompat.GetColor(Context, backgroundColorId);
                _backgroundDrawable.SetColor(color);
            }

            Background = _backgroundDrawable;

            if (loadFromUrl)
            {
                _shouldLoadFromWeb = true;
                ToggleBigImageVisibility();
            }
        }

        private void ToggleBigImageVisibility()
        {
            if (_shouldLoadFromWeb)
            {
                _internalImage.Visibility = ViewStates.Gone;
                _internalLoadableImage.Visibility = ViewStates.Visible;
            }
            else
            {
                _internalImage.Visibility = ViewStates.Visible;
                _internalLoadableImage.Visibility = ViewStates.Gone;
            }

            RequestLayout();
        }
    }
}