using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;

namespace Poprey.Droid.ViewHolders
{
    public class HashtagViewHolder : MvxRecyclerViewHolder
    {
        private TextView _hashtagText;
        private RelativeLayout _tickLayout;

        public HashtagViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _hashtagText = itemView.FindViewById<TextView>(Resource.Id.Hashtag_text);
            _tickLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.tick_layout);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<HashtagViewHolder, Hashtag>();

            bindingSet.Bind(_hashtagText).For(v => v.Text).To(vm => vm.Text);
            bindingSet.Bind(_tickLayout).For(v => v.Visibility).To(vm => vm.IsTickNow).WithConversion("Visibility");

            bindingSet.Apply();
        }
    }
}