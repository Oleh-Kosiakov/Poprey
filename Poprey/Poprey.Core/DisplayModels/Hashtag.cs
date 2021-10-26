using System;
using MvvmCross.ViewModels;
using Poprey.Core.Util;

namespace Poprey.Core.DisplayModels
{
    public class Hashtag : MvxViewModel, IDisposable
    {
        private Timer _timer;

        public string Text { get; set; }

        public bool IsTickNow { get; set; }

        public void BecomeTick()
        {
            if (_timer != null)
                return;

            IsTickNow = true;
            RaisePropertyChanged(() => IsTickNow);

            _timer = new Timer(OnTimerEllapsed, Constants.HashtagPageTickDelay);
        }

        private void OnTimerEllapsed()
        {
            _timer.Dispose();
            _timer = null;

            IsTickNow = false;
            RaisePropertyChanged(() => IsTickNow);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}