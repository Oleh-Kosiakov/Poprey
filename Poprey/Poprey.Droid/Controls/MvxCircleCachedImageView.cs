using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.Annotation;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading.Cross;
using Java.Lang;
using Math = Java.Lang.Math;
using Uri = Android.Net.Uri;

namespace Poprey.Droid.Controls
{
    public class MvxCircleCachedImageView : MvxCachedImageView
    {
        private static readonly ScaleType SCALE_TYPE = ImageView.ScaleType.CenterCrop;
        private static readonly Bitmap.Config BitmapConfig = Bitmap.Config.Argb8888;
        private const int ColordrawableDimension = 2;
        private const int DefaultBorderWidth = 0;
        private const bool DefaultBorderOverlay = false;

        private static readonly Color DefaultBorderColor = Color.Black;
        private static readonly Color DefaultCircleBackgroundColor = Color.Transparent;

        private readonly RectF MDrawableRect = new RectF();
        private readonly RectF MBorderRect = new RectF();
        private readonly Matrix MShaderMatrix = new Matrix();
        private readonly Paint MBitmapPaint = new Paint();
        private readonly Paint MBorderPaint = new Paint();
        private readonly Paint MCircleBackgroundPaint = new Paint();

        private int _mBorderColor = DefaultBorderColor;

        private int _mBorderWidth = DefaultBorderWidth;

        private int _mCircleBackgroundColor = DefaultCircleBackgroundColor;

        private Bitmap _mBitmap;
        private BitmapShader _mBitmapShader;

        private int _mBitmapWidth;
        private int _mBitmapHeight;


        private float _mDrawableRadius;
        private float _mBorderRadius;

        private ColorFilter _mColorFilter;
        private bool _mReady;
        private bool _mSetupPending;
        private bool _mBorderOverlay;
        private bool _mDisableCircularTransformation;

        public MvxCircleCachedImageView(Context context):base(context)
        {
            init();
        }

        public MvxCircleCachedImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            IntiCtor(context, attrs, 0);
        }

        public MvxCircleCachedImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private void IntiCtor(Context context, IAttributeSet attrs, int defStyle)
        {
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.MvxCircleCachedImageView, defStyle, 0);

            _mBorderWidth = a.GetDimensionPixelSize(Resource.Styleable.MvxCircleCachedImageView_civ_border_width, DefaultBorderWidth);
            _mBorderColor = a.GetColor(Resource.Styleable.MvxCircleCachedImageView_civ_border_color, DefaultBorderColor);
            _mBorderOverlay = a.GetBoolean(Resource.Styleable.MvxCircleCachedImageView_civ_border_overlay, DefaultBorderOverlay);
            _mCircleBackgroundColor = a.GetColor(Resource.Styleable.MvxCircleCachedImageView_civ_circle_background_color, DefaultCircleBackgroundColor);

            a.Recycle();
            init();
        }


        private void init()
        {
            base.SetScaleType(SCALE_TYPE);
            _mReady = true;
        
            base.OutlineProvider = new CustomOutlineProvider(MBorderRect);

            if (_mSetupPending)
            {
                Setup();
                _mSetupPending = false;
            }
        }

        public override ScaleType GetScaleType()
        {
            return SCALE_TYPE;
        }


        public override void SetScaleType(ScaleType scaleType)
        {
            if (scaleType != SCALE_TYPE)
            {
                throw new IllegalArgumentException(string.Format("ScaleType %s not supported.", scaleType));
            }
        }

        public override void SetAdjustViewBounds(bool adjustViewBounds)
        {
            if (adjustViewBounds)
            {
                throw new IllegalArgumentException("adjustViewBounds not supported.");
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (_mDisableCircularTransformation)
            {
                base.OnDraw(canvas);
                return;
            }

            if (_mBitmap == null)
            {
                return;
            }

            if (_mCircleBackgroundColor != Color.Transparent)
            {
                canvas.DrawCircle(MDrawableRect.CenterX(), MDrawableRect.CenterY(), _mDrawableRadius, MCircleBackgroundPaint);
            }

            canvas.DrawCircle(MDrawableRect.CenterX(), MDrawableRect.CenterY(), _mDrawableRadius, MBitmapPaint);

            if (_mBorderWidth > 0)
            {
                canvas.DrawCircle(MBorderRect.CenterX(), MBorderRect.CenterY(), _mBorderRadius, MBorderPaint);
            }
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Setup();
        }

        public override void SetPadding(int left, int top, int right, int bottom)
        {
            base.SetPadding(left, top, right, bottom);
            Setup();
        }

        public override void SetPaddingRelative(int start, int top, int end, int bottom)
        {
            base.SetPaddingRelative(start, top, end, bottom);
            Setup();
        }

        public int GetBorderColor()
        {
            return _mBorderColor;
        }

        public void SetBorderColor([ColorInt] int borderColor)
        {

            if (borderColor == _mBorderColor)
            {
                return;
            }

            _mBorderColor = borderColor;
            MBorderPaint.Color = new Color(borderColor);

            Invalidate();
        }

        public int GetCircleBackgroundColor()
        {
            return _mCircleBackgroundColor;
        }

        public void SetCircleBackgroundColor([ColorInt] int circleBackgroundColor)
        {
            if (circleBackgroundColor == _mCircleBackgroundColor)
            {
                return;
            }

            _mCircleBackgroundColor = circleBackgroundColor;
            MCircleBackgroundPaint.Color = new Color(circleBackgroundColor);
            Invalidate();
        }

        public void SetCircleBackgroundColorResource([ColorRes]int circleBackgroundRes)
        {
            SetCircleBackgroundColor(Context.Resources.GetColor(circleBackgroundRes));
        }

        public int GetBorderWidth()
        {
            return _mBorderWidth;
        }

        public void SetBorderWidth(int borderWidth)
        {
            if (borderWidth == _mBorderWidth)
            {
                return;
            }

            _mBorderWidth = borderWidth;
            Setup();
        }

        public bool IsBorderOverlay()
        {
            return _mBorderOverlay;
        }

        public void SetBorderOverlay(bool borderOverlay)
        {
            if (borderOverlay == _mBorderOverlay)
            {
                return;
            }

            _mBorderOverlay = borderOverlay;
            Setup();
        }

        public bool IsDisableCircularTransformation()
        {
            return _mDisableCircularTransformation;
        }

        public void SetDisableCircularTransformation(bool disableCircularTransformation)
        {
            if (_mDisableCircularTransformation == disableCircularTransformation)
            {
                return;
            }

            _mDisableCircularTransformation = disableCircularTransformation;
            InitializeBitmap();
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            InitializeBitmap();
        }

        public override void SetImageDrawable(Drawable drawable)
        {
            base.SetImageDrawable(drawable);
            InitializeBitmap();
        }
        public override void SetImageResource([DrawableRes] int resId)
        {
            base.SetImageResource(resId);
            InitializeBitmap();
        }

        public override void SetImageURI(Uri uri)
        {
            base.SetImageURI(uri);

            InitializeBitmap();
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            if (cf == _mColorFilter)
            {
                return;
            }

            _mColorFilter = cf;

            applyColorFilter();
            Invalidate();
        }

        public override ColorFilter ColorFilter => _mColorFilter;

        private void applyColorFilter()
        {
            MBitmapPaint.SetColorFilter(_mColorFilter);
        }

        private Bitmap getBitmapFromDrawable(Drawable drawable)
        {
            if (drawable == null)
            {
                return null;
            }

            if (drawable is BitmapDrawable) {

                return ((BitmapDrawable)drawable).Bitmap;
            }

            try
            {
                Bitmap bitmap;

                if (drawable is ColorDrawable) {
                    bitmap = Bitmap.CreateBitmap(ColordrawableDimension, ColordrawableDimension, BitmapConfig);
                } else {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, BitmapConfig);
                }

                var canvas = new Canvas(bitmap);

                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);

                drawable.Draw(canvas);

                return bitmap;

            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        private void InitializeBitmap()
        {
            if (_mDisableCircularTransformation)
            {
                _mBitmap = null;
            }
            else
            {
                _mBitmap = getBitmapFromDrawable(Drawable);
            }

            Setup();
        }


        private void Setup()
        {

            if (!_mReady)
            {
                _mSetupPending = true;

                return;
            }

            if (Width == 0 && Height == 0)
            {
                return;
            }

            if (_mBitmap == null)
            {
                Invalidate();

                return;
            }

            _mBitmapShader = new BitmapShader(_mBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

            MBitmapPaint.AntiAlias = true;
            MBitmapPaint.SetShader(_mBitmapShader);

            MBorderPaint.SetStyle(Paint.Style.Stroke);
            MBorderPaint.AntiAlias = true;

            MBorderPaint.Color = new Color(_mCircleBackgroundColor);
            MBorderPaint.StrokeWidth = _mBorderWidth;

            MCircleBackgroundPaint.SetStyle(Paint.Style.Fill);
            MCircleBackgroundPaint.AntiAlias = true;

            MCircleBackgroundPaint.Color = new Color(_mCircleBackgroundColor);
            _mBitmapHeight = _mBitmap.Height;
            _mBitmapWidth = _mBitmap.Width;

            MBorderRect.Set(calculateBounds());

            _mBorderRadius = Math.Min((MBorderRect.Height() - _mBorderWidth) / 2.0f, (MBorderRect.Width() - _mBorderWidth) / 2.0f);

            MDrawableRect.Set(MBorderRect);

            if (!_mBorderOverlay && _mBorderWidth > 0)
            {
                MDrawableRect.Inset(_mBorderWidth - 1.0f, _mBorderWidth - 1.0f);
            }

            _mDrawableRadius = Math.Min(MDrawableRect.Height() / 2.0f, MDrawableRect.Width() / 2.0f);

            applyColorFilter();

            UpdateShaderMatrix();

            Invalidate();
        }



        private RectF calculateBounds()
        {
            var availableWidth = Width - PaddingLeft - PaddingRight;
            var availableHeight = Height - PaddingTop - PaddingBottom;

            var sideLength = Math.Min(availableWidth, availableHeight);
            var left = PaddingLeft + (availableWidth - sideLength) / 2f;
            var top = PaddingTop + (availableHeight - sideLength) / 2f;

            return new RectF(left, top, left + sideLength, top + sideLength);
        }



        private void UpdateShaderMatrix()
        {
            float scale;
            float dx = 0;
            float dy = 0;

            MShaderMatrix.Set(null);

            if (_mBitmapWidth * MDrawableRect.Height() > MDrawableRect.Width() * _mBitmapHeight)
            {
                scale = MDrawableRect.Height() / _mBitmapHeight;
                dx = (MDrawableRect.Width() - _mBitmapWidth * scale) * 0.5f;
            }
            else
            {
                scale = MDrawableRect.Width() / (float)_mBitmapWidth;
                dy = (MDrawableRect.Height() - _mBitmapHeight * scale) * 0.5f;
            }

            MShaderMatrix.SetScale(scale, scale);
            MShaderMatrix.PostTranslate((int)(dx + 0.5f) + MDrawableRect.Left, (int)(dy + 0.5f) + MDrawableRect.Top);
            _mBitmapShader.SetLocalMatrix(MShaderMatrix);
        }

        public override bool OnTouchEvent(MotionEvent evnt)
        {
            return InTouchableArea(evnt.GetX(), evnt.GetY()) && base.OnTouchEvent(evnt);
        }

        private bool InTouchableArea(float x, float y)
        {
            return Math.Pow(x - MBorderRect.CenterX(), 2) + Math.Pow(y - MBorderRect.CenterY(), 2) <= Math.Pow(_mBorderRadius, 2);
        }

        private class CustomOutlineProvider : ViewOutlineProvider
        {
            private RectF _mBorderRectF;

            public CustomOutlineProvider(RectF mBorderRect)
            {
                _mBorderRectF = mBorderRect;
            }

            public override void GetOutline(View view, Outline outline)
            {
                var bounds = new Rect();

                _mBorderRectF.RoundOut(bounds);

                outline.SetRoundRect(bounds, bounds.Width() / 2.0f);
            }
        }
    }
}
