using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gu.Wpf.Media;
using Microsoft.Win32;
using VideoPlayer.Infrastructure.Commands;
using VideoPlayer.Models;
using VideoPlayer.ViewModels.Base;

namespace VideoPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region title : string - Title of window

        ///<summary>Title of window</summary>
        private string _title = "Video Player";

        ///<summary>Title of window</summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        #endregion

        public VideoPlayerViewModel VideoPlayerViewModel { get; }

        #region Commands

        #region OpenVideoCommand

        public ICommand OpenVideoCommand { get; }

        //todo change _path with Video class
        private void OnOpenVideoCommandExecuted(object p)
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All Media Files|*.avi;*.mp4;";

            if (fileDialog.ShowDialog() != null && fileDialog.FileName != "")
            {
                VideoPlayerViewModel.SelectedVideo = new Video
                {
                    Path = new Uri(fileDialog.FileName),
                    Name = fileDialog.SafeFileName,
                    CurrentPosition = default
                };
            }
        }

        private bool CanOpenVideoCommandExecute(object p) => true;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            OpenVideoCommand = new ActionCommand(OnOpenVideoCommandExecuted, CanOpenVideoCommandExecute);

            VideoPlayerViewModel = new VideoPlayerViewModel(this);
        }
    }
}