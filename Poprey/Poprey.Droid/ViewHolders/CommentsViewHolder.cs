using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.ViewHolders
{
    public class CommentsViewHolder : MvxRecyclerViewHolder
    {
        private AnyFontTextView _textView;

        public CommentsViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _textView = itemView.FindViewById<AnyFontTextView>(Resource.Id.comment_text);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<CommentsViewHolder, CommentItem>();

            bindingSet.Bind(_textView).For(v => v.Text).To(vm => vm.Text);

            bindingSet.Apply();
        }
    }
}