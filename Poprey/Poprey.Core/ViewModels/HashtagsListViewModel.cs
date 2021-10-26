using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.Clipboard;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class HashtagsListViewModel : BaseViewModel<string>
    {
        private Timer _timer;

        private readonly List<Hashtag> _allHashtags;

        public string SelectedHashtagText { get; set; }
        public MvxObservableCollection<Hashtag> VisibleHashtags { get; set; }

        public MvxObservableCollection<Hashtag> RemovedHashtags { get; set; } = new MvxObservableCollection<Hashtag>();

        public int AllHashtagsCount => _allHashtags.Count;

        private int _backgroundOverlayDesiredInNativeUnits;

        public int BackgroundOverlayWidthInNativeUnits
        {
            get => _backgroundOverlayDesiredInNativeUnits;
            set
            {
                if(_backgroundOverlayDesiredInNativeUnits == value)
                    return;

                _backgroundOverlayDesiredInNativeUnits = value;
                HeaderViewModel.BackgroundOverlayWidthInNativeUnits = value;

                RaisePropertyChanged(() => BackgroundOverlayWidthInNativeUnits);
            }
        }
        public bool IsCopied { get; set; }

        public int VisibleHashtagsCount
        {
            get => VisibleHashtags.Count;
            set
            {
                AddOrRemoveVisibleTags(value);

                RaisePropertyChanged(() => VisibleHashtagsCount);
            }
        }

        public HashtagsListViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService,
            MessageTokenHelper messenger, IPopupService popupService, IHashtagsService hashtagsService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
#if DEBUG
            SelectedHashtagText = "likes";
#endif
            MainViewModel.HashtagViewModel = this;

            _allHashtags = hashtagsService.GetLastLoadedHashtags()
                .Select(str => new Hashtag { Text = str }).ToList();

            ShowHalfOfTheHashtags();
        }

        private void ShowHalfOfTheHashtags()
        {
            VisibleHashtags = new MvxObservableCollection<Hashtag>(_allHashtags.Take(_allHashtags.Count / 2).ToList());
        }

        public override void Prepare(string selectedHashtag)
        {
            SelectedHashtagText = selectedHashtag;
        }

        public override void ViewDisappearing()
        {
            BackgroundOverlayWidthInNativeUnits = 0;

            base.ViewDisappearing();
        }

        private void AddOrRemoveVisibleTags(int value)
        {
            var newTags = _allHashtags.Take(value);
            VisibleHashtags.SwitchTo(newTags);
        }

        private void OnTimerExpired()
        {
            _timer.Dispose();
            _timer = null;

            IsCopied = false;
            RaisePropertyChanged(() => IsCopied);
        }
        
        private IMvxCommand _removeHashtagCommand;
        public IMvxCommand RemoveHashtagCommand => _removeHashtagCommand =
            _removeHashtagCommand ?? new MvxCommand<Hashtag>(
                ht =>
                {
                    _allHashtags.Remove(ht);
                    VisibleHashtags.Remove(ht);
                    RemovedHashtags.Add(ht);

                    RaisePropertyChanged(() => AllHashtagsCount);
                    RaisePropertyChanged(() => VisibleHashtagsCount);
                });

        private IMvxCommand _restoreHashtagCommand;
        public IMvxCommand RestoreHashtagCommand => _restoreHashtagCommand =
            _restoreHashtagCommand ?? new MvxCommand<Hashtag>(
                ht =>
                {
                    _allHashtags.Insert(0, ht);
                    VisibleHashtags.Insert(0, ht);
                    RemovedHashtags.Remove(ht);

                    RaisePropertyChanged(() => AllHashtagsCount);
                    RaisePropertyChanged(() => VisibleHashtagsCount);
                });

        private IMvxCommand _copyHashtagCommand;
        public IMvxCommand CopyHashtagCommand => _copyHashtagCommand =
            _copyHashtagCommand ?? new MvxCommand<Hashtag>(
                ht =>
                {
                    CrossClipboard.Current.SetText($"#{ht.Text}");

                    ht.BecomeTick();
                });

        private IMvxCommand _copyAllCommand;

        public IMvxCommand CopyAllCommand => _copyAllCommand =
            _copyAllCommand ?? new MvxCommand<Hashtag>(
                ht =>
                {
                    if(_timer != null)
                        return;

                    var builder = new StringBuilder();

                    foreach (var hashtag in VisibleHashtags)
                    {
                        builder.Append($"#{hashtag.Text} ");
                    }

                    CrossClipboard.Current.SetText(builder.ToString());

                    // UI logic

                    IsCopied = true;
                    RaisePropertyChanged(() => IsCopied);

                    _timer = new Timer(OnTimerExpired, Constants.HashtagPageTickDelay);
                });

     
    }
}