using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Droid.ViewHolders;

namespace Poprey.Droid.Adapters
{
    public class InstagramPostPreviewAdapter : MvxRecyclerAdapter
    {
        public bool RequireCompactLayout { get; set; }

        public InstagramPostPreviewAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

            var view = itemBindingContext.BindingInflate(RequireCompactLayout 
                ? Resource.Layout.compact_instagram_post_preview_template 
                : Resource.Layout.instagram_post_preview_template, parent, false);

            return new InstagramPostPreviewViewHolder(view, itemBindingContext)
            {
                Click = ItemClick,
                LongClick = ItemLongClick
            };
        }
    }
}