using System;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Components;

namespace Poprey.Droid.ViewHolders
{
    public class AddInstagramAccountViewHolder : MvxRecyclerViewHolder
    {
        private AdaptiveBackgroundImageView _adaptiveImageView;
        private TextView _editText;
        private ImageView _removeCrossImageView;
        private View _separatorView;
        private View _itemView;

        public AddInstagramAccountViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _itemView = itemView;
            _adaptiveImageView = itemView.FindViewById<AdaptiveBackgroundImageView>(Resource.Id.image);
            _editText = itemView.FindViewById<TextView>(Resource.Id.account_name_text_view);
            _removeCrossImageView = itemView.FindViewById<ImageView>(Resource.Id.cross_remove_image);
            _separatorView = itemView.FindViewById<View>(Resource.Id.separator);

            _adaptiveImageView.ShouldLoadImageFromWeb = true;
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<AddInstagramAccountViewHolder, AddInstagramAccount>();

            bindingSet.Bind(_adaptiveImageView).For(v => v.ImageSrc).To(vm => vm.ImageUrl);
            bindingSet.Bind(_editText).For(v => v.Text).To(vm => vm.AccountName);
            bindingSet.Bind(_removeCrossImageView).For("Click").To(vm => vm.RemoveCommand);
            bindingSet.Bind(_itemView).For("Click").To(vm => vm.SwitchCommand);
            bindingSet.Bind(_separatorView).For(v => v.Visibility).To(vm => vm.ShowSeparator).WithConversion("Visibility");

            bindingSet.Apply();
        }
    }
}