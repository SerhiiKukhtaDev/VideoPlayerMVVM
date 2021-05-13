using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoPlayer.Infrastructure.Commands;
using VideoPlayer.Models;
using VideoPlayer.ViewModels.Base;
using MediaState = Gu.Wpf.Media.MediaState;

namespace VideoPlayer.ViewModels
{
    public class VideoPlayerViewModel : ViewModel
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        #region selectedVideo : Video - Selected Video

        ///<summary>Selected Video</summary>
        private Video _selectedVideo;

        ///<summary>Selected Video</summary>
        public Video SelectedVideo
        {
            get => _selectedVideo;
            set => Set(ref _selectedVideo, value);
        }

        #endregion

        #region VideoPlayCommand

        public ICommand VideoPlayCommand { get; }

        private void OnVideoPlayCommandExecuted(object p)
        {
            if (!(p is Gu.Wpf.Media.MediaElementWrapper videoMediaElement)) return;

            videoMediaElement.TogglePlayPause();
            
            //videoMediaElement.Play();
        }

        private bool CanVideoPlayCommandExecute(object p)
        {
            return SelectedVideo != null;
        }

        #endregion

        public VideoPlayerViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            VideoPlayCommand = new ActionCommand(OnVideoPlayCommandExecuted, CanVideoPlayCommandExecute);
        }
    }
}