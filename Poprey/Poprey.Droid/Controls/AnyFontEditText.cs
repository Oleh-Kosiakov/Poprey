using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Widget;
using Java.Lang;
using String = Java.Lang.String;

namespace Poprey.Droid.Controls
{
    public class AnyFontEditText : EditText
    {
        private bool _allowUppercaseAndSpace;

        protected AnyFontEditText(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnyFontEditText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetCustomFont(context, attrs);
            ProcessFilters();
        }

        private void ProcessFilters()
        {
            if (_allowUppercaseAndSpace)
            {
                return;
            }

            var editFilters = GetFilters();
            var newFilters = new List<IInputFilter>(editFilters)
            {
                new AllLowerCaseFilter(),
                new SpaceAsUnderscoreFilter()
            };
            SetFilters(newFilters.ToArray());
        }

        private void SetCustomFont(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.AnyFontTextView);
            var customFontName = typedArray.GetString(Resource.Styleable.AnyFontTextView_customFont);
            _allowUppercaseAndSpace = attrs.GetAttributeBooleanValue("http://schemas.android.com/apk/res-auto", "allowUppercaseAndSpace", false);

            if (customFontName == null)
            {
                typedArray.Recycle();
                return;
            }

            Typeface = Android.Graphics.Typeface.CreateFromAsset(context.Assets, customFontName);
            typedArray.Recycle();
        }

        private class AllLowerCaseFilter : Java.Lang.Object, IInputFilter
        {
            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                if (source.ToString() == source.ToString().ToLower())
                    return null;

                return new String(source.ToString().ToLower());
            }
        }

        private class SpaceAsUnderscoreFilter : Java.Lang.Object, IInputFilter
        {
            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                if (!source.Contains(' '))
                    return null;

                return new String(source.ToString().Replace(" ", "_"));
            }
        }
    }
}