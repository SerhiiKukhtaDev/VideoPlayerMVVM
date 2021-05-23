using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Gu.Wpf.Media;
using VideoPlayer.Infrastructure.Commands;
using VideoPlayer.Models;
using VideoPlayer.Services;
using VideoPlayer.Utils;
using VideoPlayer.ViewModels.Base;
using Application = System.Windows.Application;
using FileDialog = Microsoft.Win32.FileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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

        #region currentPlayedVideo : Video - Current Played Video

        ///<summary>Current Played Video</summary>
        private Video _currentPlayedVideo;

        ///<summary>Current Played Video</summary>
        public Video CurrentPlayedVideo
        {
            get => _currentPlayedVideo;
            set => Set(ref _currentPlayedVideo, value);
        }

        #endregion

        #region selectedFolder : Folder - SelectedFoler

        ///<summary>SelectedFoler</summary>
        private SimpleFolder _selectedFolder;

        ///<summary>Selected Foler</summary>
        public SimpleFolder SelectedFolder
        {
            get => _selectedFolder;
            set => Set(ref _selectedFolder, value);
        }

        #region currentFolder : Folder - Current Folder

        ///<summary>Current Folder</summary>
        private Folder _currentFolder;

        ///<summary>Current Folder</summary>
        public Folder CurrentFolder
        {
            get => _currentFolder;
            set => Set(ref _currentFolder, value);
        }

        #endregion

        #endregion

        #region lastFolder : Stack<SimpleFolder> - Last opened folder

        private readonly Stack<SimpleFolder> _lastFolders;

        #region SimpleFolder : SimpleFolder - Last Folder

        ///<summary>Last Folder</summary>
        private SimpleFolder _lastFolder;

        ///<summary>Last Folder</summary>
        public SimpleFolder LastFolder
        {
            get => _lastFolder;
            set => Set(ref _lastFolder, value);
        }

        #endregion

        #endregion

        #region Commands

        #region OpenVideoCommand

        public ICommand OpenVideoCommand { get; }

        
        private void OnOpenVideoCommandExecuted(object p)
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All Media Files|*.avi;*.mp4;*.mkv;";

            if (fileDialog.ShowDialog() == null || fileDialog.FileName == "") return;

            string folderName = Path.GetDirectoryName(fileDialog.FileName);

            CurrentFolder = new Folder
            {
                Name = folderName,
                Path = fileDialog.FileName,
                Videos = VideoService.GetVideos(folderName, "mkv")
            };
        }

        private bool CanOpenVideoCommandExecute(object p) => true;

        #endregion

        #region ToggleFullScreenCommand

        public ICommand ToggleFullScreenCommand { get; }

        private void OnToggleFullScreenCommandExecuted(object p)
        {
            if (Application.Current.MainWindow != null && p is MediaElementWrapper videoPlayer) 
            {
                Window window = Application.Current.MainWindow;
                
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                    window.SizeToContent = SizeToContent.WidthAndHeight;
                    window.ResizeMode = ResizeMode.NoResize;
            }
                
        }

        private bool CanToggleFullScreenCommandExecute(object p) => true;

        #endregion

        #region ChangeCurrentVideo

        public ICommand ChangeCurrentVideo { get; }

        private void OnChangeCurrentVideoExecuted(object p)
        {
            if(SelectedVideo.Equals(CurrentPlayedVideo)) return;
            CurrentPlayedVideo = SelectedVideo;
        }

        private bool CanChangeCurrentVideoExecute(object p) => true;

        #endregion

        #region ChangeCurrentFolder 

        public ICommand ChangeCurrentFolder  { get; }

        private void OnChangeCurrentFolderExecuted(object p)
        {
            LastFolder = new SimpleFolder {Name = CurrentFolder.Name, Path = CurrentFolder.Path};
            _lastFolders.Push(LastFolder);

            CurrentFolder = new Folder
            {
                Folders = FolderService.GetFolders(SelectedFolder.Path),
                Videos = VideoService.GetVideos(SelectedFolder.Path, "mkv"),
                Name = SelectedFolder.Name,
                Path = SelectedFolder.Path
            };
        }

        private bool CanChangeCurrentFolderExecute(object p) => true;

        #endregion

        #region OpenFolderCommand

        public ICommand OpenFolderCommand { get; }

        private void OnOpenFolderCommandExecuted(object p)
        {
            var dlg = new FolderPicker {InputPath = @"c:\"};
            dlg.ShowDialog();

            CurrentFolder = new Folder
            {
                Videos = VideoService.GetVideos(dlg.ResultPath, "mkv"),
                Folders = FolderService.GetFolders(dlg.ResultPath),
                Name = dlg.ResultName,
                Path = dlg.ResultPath
            };
        }

        private bool CanOpenFolderCommandExecute(object p) => true;

        #endregion

        #region BackToLastFolderCommand

        public ICommand BackToLastFolderCommand { get; }

        private void OnBackToLastFolderCommandExecuted(object p)
        {
            LastFolder = _lastFolders.Pop();

            CurrentFolder = new Folder
            {
                Folders = FolderService.GetFolders(LastFolder.Path),
                Videos = VideoService.GetVideos(LastFolder.Path, "mkv"),
                Name = LastFolder.Name,
                Path = LastFolder.Path
            };

            if(_lastFolders.Count > 0) LastFolder = _lastFolders.Peek();
        }

        private bool CanBackToLastFolderCommandExecute(object p) => _lastFolders.Count > 0;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            OpenVideoCommand = new ActionCommand(OnOpenVideoCommandExecuted, CanOpenVideoCommandExecute);
            ToggleFullScreenCommand =
                new ActionCommand(OnToggleFullScreenCommandExecuted, CanToggleFullScreenCommandExecute);
            ChangeCurrentVideo = new ActionCommand(OnChangeCurrentVideoExecuted, CanChangeCurrentVideoExecute);
            OpenFolderCommand = new ActionCommand(OnOpenFolderCommandExecuted, CanOpenFolderCommandExecute);
            ChangeCurrentFolder = new ActionCommand(OnChangeCurrentFolderExecuted, CanChangeCurrentFolderExecute);
            BackToLastFolderCommand =
                new ActionCommand(OnBackToLastFolderCommandExecuted, CanBackToLastFolderCommandExecute);
            _lastFolders = new Stack<SimpleFolder>();
        }
    }
}