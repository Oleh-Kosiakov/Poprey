using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Poprey.Droid.Controls
{
    public class DiscountSeekBar : SeekBar
    {
        private Context _context;
        private float MiddleY => (float) Height / 2;
        private float _discountThumbRadius = 10;

        #region Constructors
        protected DiscountSeekBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DiscountSeekBar(Context context) : base(context)
        {
            Initialize(context);
        }

        public DiscountSeekBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public DiscountSeekBar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public DiscountSeekBar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            _context = context;
        }

        #endregion

        //protected override void OnDraw(Canvas canvas)
        //{
        //    base.OnDraw(canvas);

        //    DrawDiscountThumb(canvas, 600);
        //}

        //private void DrawDiscountThumb(Canvas canvas, float xPos)
        //{
        //    var paint = new Paint
        //    {
        //        Color = Color.Red
        //    };
        //    paint.SetStyle(Paint.Style.Fill);
        //    canvas.DrawCircle(xPos, MiddleY, _discountThumbRadius, paint);
        //}
    }
}