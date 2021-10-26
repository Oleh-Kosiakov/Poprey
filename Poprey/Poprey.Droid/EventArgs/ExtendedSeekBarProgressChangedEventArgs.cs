namespace Poprey.Droid.EventArgs
{
    public class ExtendedSeekBarProgressChangedEventArgs : System.EventArgs
    {
        public int ProgressLeftThumbnailOffsetOnWindow { get; set; }

        public int Progress { get; set; }
    }
}