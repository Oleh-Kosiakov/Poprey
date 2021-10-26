using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Poprey.Droid.Controls
{
    public class AnyFontTextView : TextView
    {
        protected AnyFontTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnyFontTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetCustomFont(context, attrs);
        }

        private void SetCustomFont(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.AnyFontEditText);
            var customFontName = typedArray.GetString(Resource.Styleable.AnyFontEditText_customFont);

            if (customFontName == null)
            {
                return;
            }

            Typeface = Android.Graphics.Typeface.CreateFromAsset(context.Assets, customFontName);
            typedArray.Recycle();
        }
    }
}