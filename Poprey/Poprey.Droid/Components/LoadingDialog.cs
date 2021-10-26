using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Com.Bumptech.Glide;

namespace Poprey.Droid.Components
{
    public class LoadingDialog : Dialog
    {
        private Activity _context;

        protected LoadingDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LoadingDialog(Context context, int themeResId) : base(context, themeResId)
        {
            _context = (Activity)context;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.loading_dialog);

            var imageView = FindViewById<ImageView>(Resource.Id.image_container);
            Glide.With(_context).AsGif().Load(Resource.Drawable.loading).Into(imageView);
        }
    }
}