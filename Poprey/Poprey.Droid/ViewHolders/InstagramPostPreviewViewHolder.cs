using Android.Views;
using FFImageLoading.Cross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.ViewHolders
{
    public class InstagramPostPreviewViewHolder : MvxRecyclerViewHolder
    {
        private const string PlaceholderPath = "post_placeholder.png";

        private MvxCachedImageView _mainImage;
        private View _shadowOverlay;
        private AnyFontTextView _numberLabel;


        public InstagramPostPreviewViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _mainImage = itemView.FindViewById<MvxCachedImageView>(Resource.Id.post_preview_main_image);
            _shadowOverlay = itemView.FindViewById<View>(Resource.Id.post_preview_shadow_overlay);
            _numberLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.post_preview_number_label);

            _mainImage.LoadingPlaceholderImagePath = PlaceholderPath;
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<InstagramPostPreviewViewHolder, InstagramPostPreview>();

            bindingSet.Bind(_mainImage).For(v => v.ImagePath).To(vm => vm.ImageUrl);
            bindingSet.Bind(_shadowOverlay).For(v => v.Visibility).To(vm => vm.IsSelected).WithConversion("Visibility");
            bindingSet.Bind(_numberLabel).For(v => v.Text).To(vm => vm.Number);
            bindingSet.Bind(_numberLabel).For(v => v.Visibility).To(vm => vm.IsSelected).WithConversion("Visibility");

            bindingSet.Apply();
        }
    }
}