using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;
using Poprey.Droid.Util;

namespace Poprey.Droid.Components
{
    public class ExtendedSeekBar : BaseView
    {
        private const int SeekBarStandardLeftMarginInDp = 7;
        private const int ScrollBarPadding = 8;

        private bool _needsInitialEvent;
        private DiscountSeekBar _seekBar;
        private AnyFontTextView _counterTextView;

        public event EventHandler<ExtendedSeekBarProgressChangedEventArgs> ProgressChanged;
        public event EventHandler TouchUpOnSeekBar;

        public int Max
        {
            get => _seekBar.Max;
            set => _seekBar.Max = value;
        }

        public int Progress
        {
            get => _seekBar.Progress;
            set => _seekBar.Progress = value;
        }

        public string CounterText
        {
            get => _counterTextView.Text;
            set
            {
                _counterTextView.Text = value;

                var seekBarLayoutParameters = (LinearLayout.LayoutParams) _seekBar.LayoutParameters;

                if (_counterTextView.Text == string.Empty)
                {
                    if (_counterTextView.Visibility != ViewStates.Gone)
                    {
                        _counterTextView.Visibility = ViewStates.Gone;
                        seekBarLayoutParameters.LeftMargin = 0;
                    }
                }
                else if(_counterTextView.Visibility != ViewStates.Visible)
                {
                    _counterTextView.Visibility = ViewStates.Visible;
                    seekBarLayoutParameters.LeftMargin = DpConverter.ConvertDpToPx(SeekBarStandardLeftMarginInDp, Resources);
                }

                RequestLayout();
            }
        }

        public ExtendedSeekBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void Initialize(ViewGroup viewGroup, IAttributeSet attrs)
        {
            var inflater = LayoutInflater.From(Context);
            var layout = inflater.Inflate(Resource.Layout.extended_seek_bar, this);

            _seekBar = layout.FindViewById<DiscountSeekBar>(Resource.Id.seek_bar);
            _counterTextView = layout.FindViewById<AnyFontTextView>(Resource.Id.counter);

            _seekBar.SetPadding(DpConverter.ConvertDpToPx(ScrollBarPadding * 3, Resources),
                                _seekBar.PaddingTop,
                                DpConverter.ConvertDpToPx(ScrollBarPadding,Resources), 
                                _seekBar.PaddingBottom);

            _seekBar.ProgressChanged += SeekBarOnProgressChanged;
            _seekBar.Touch += SeekBarOnTouch;
            ViewTreeObserver.PreDraw += ViewTreeObserverOnPreDraw;

            _needsInitialEvent = true;
        }

        private void SeekBarOnTouch(object sender, TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                TouchUpOnSeekBar?.Invoke(this, null);
            }

            e.Handled = false;
        }

        private void ViewTreeObserverOnPreDraw(object sender, System.EventArgs e)
        {
            if (_needsInitialEvent)
            {
                FireProgressBarPositionChanged(this);
            }

            _needsInitialEvent = false;
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs progressChangedEventArgs)
        {
            FireProgressBarPositionChanged(sender);
        }

        private void FireProgressBarPositionChanged(object sender)
        {
            var seekBarOffset = new int[2];
            _seekBar.GetLocationInWindow(seekBarOffset);

            var seekBarWidth = _seekBar.Width - _seekBar.PaddingLeft - _seekBar.PaddingRight;
            var thumbOffset = seekBarOffset[0] + _seekBar.PaddingLeft + seekBarWidth * _seekBar.Progress / _seekBar.Max;

            var eventArgs = new ExtendedSeekBarProgressChangedEventArgs
            {
                ProgressLeftThumbnailOffsetOnWindow = thumbOffset,
                Progress = _seekBar.Progress
            };

            ProgressChanged?.Invoke(sender, eventArgs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _seekBar.ProgressChanged -= SeekBarOnProgressChanged;
                _seekBar.Touch -= SeekBarOnTouch;
                ViewTreeObserver.PreDraw -= ViewTreeObserverOnPreDraw;
            }

            base.Dispose(disposing);
        }
    }
}