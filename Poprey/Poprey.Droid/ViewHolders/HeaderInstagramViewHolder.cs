using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Components;

namespace Poprey.Droid.ViewHolders
{
    public class HeaderInstagramViewHolder : MvxRecyclerViewHolder
    {
        private AdaptiveBackgroundImageView _adaptiveImageView;
        private AdaptiveBackgroundImageView _additonalImage;

        public HeaderInstagramViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _adaptiveImageView = itemView.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.mainImage);
            _additonalImage = itemView.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.additonalImage);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<HeaderInstagramViewHolder, HeaderInstagramAccount>();

            bindingSet.Bind(_adaptiveImageView).For(v => v.ImageSrc).To(vm => vm.AvatarImageUrl);
            bindingSet.Bind(_adaptiveImageView).For(v => v.ShouldLoadImageFromWeb).To(vm => vm.ShouldLoadImageFromWeb);
            bindingSet.Bind(_adaptiveImageView).For(v => v.ShouldShowAddImage).To(vm => vm.ShouldShowAddCross);
            bindingSet.Bind(_adaptiveImageView).For(v => v.ShouldShowGrayBackground).To(vm => vm.ShouldShowGrayBackground);

            bindingSet.Bind(_additonalImage).For(v => v.Visibility).To(vm => vm.ShouldShowInstagramIcon).WithConversion("Visibility");

            bindingSet.Apply();
        }
    }
}