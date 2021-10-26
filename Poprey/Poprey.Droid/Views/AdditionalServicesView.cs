using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    [Register("poprey.droid.views.AdditionalServicesView")]
    public class AdditionalServicesView : BaseFragment<AdditionalServicesViewModel>
    {
        private LinearLayout _youtubeLayout;
        private AnyFontTextView _youtubeLabel;
        private AnyFontTextView _youtubeServicesLabel;

        private LinearLayout _tiktokLayout;
        private AnyFontTextView _tiktokLabel;
        private AnyFontTextView _tiktokServicesLabel;

        protected override int FragmentId => Resource.Layout.AdditionalServicesView;
        protected override void InitComponents(View fragmentView)
        {
            _youtubeLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.adds_youtube_layout);
            _youtubeLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.adds_youtube_label);
            _youtubeServicesLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.adds_youtube_services_label);

            _tiktokLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.adds_tiktok_layout);
            _tiktokLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.adds_tiktok_label);
            _tiktokServicesLabel = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.adds_tiktok_services_label);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<AdditionalServicesView, AdditionalServicesViewModel>();

            bindingSet.Bind(_youtubeLabel).For(v => v.Text).To(vm => vm.YoutubeLabelText);
            bindingSet.Bind(_youtubeServicesLabel).For(v => v.Text).To(vm => vm.YoutubeServicesLabelText);
            bindingSet.Bind(_tiktokLabel).For(v => v.Text).To(vm => vm.TikTokLabelText);
            bindingSet.Bind(_tiktokServicesLabel).For(v => v.Text).To(vm => vm.TikTokServicesLabelText);

            bindingSet.Bind(_youtubeLayout).For("Click").To(vm => vm.YoutubeSelectedCommand);
            bindingSet.Bind(_tiktokLayout).For("Click").To(vm => vm.TikTokSelectedCommand);

            bindingSet.Apply();
        }
    }
}