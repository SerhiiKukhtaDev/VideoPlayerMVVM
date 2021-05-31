using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Gu.Wpf.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using VideoPlayer.Infrastructure.Commands;
using VideoPlayer.Models;
using VideoPlayer.Models.Interfaces;
using VideoPlayer.Services;
using VideoPlayer.ViewModels.Base;
using Application = System.Windows.Application;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using FileDialog = Microsoft.Win32.FileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VideoPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly VideoDurationXmlSaver _videoDurationXmlSaver = new VideoDurationXmlSaver(@"Test.xml");

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

        public bool IsVideoElementHovered { get; set; }
        private bool _isHidden;

        private int _timerTime = 0;

        private DispatcherTimer _timer;

        #region cursor : Cursor - Curso

        ///<summary>Cursor</summary>
        private Cursor _cursor = Cursors.Arrow;

        ///<summary>Cursor</summary>
        public Cursor Cursor
        {
            get => _cursor;
            set => Set(ref _cursor, value);
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

        #region elVisability : Visibility - DESCRIPTION

        ///<summary>DESCRIPTION</summary>
        private Visibility _elVisibility = Visibility.Visible;

        ///<summary>DESCRIPTION</summary>
        public Visibility ElVisibility
        {
            get => _elVisibility;
            set => Set(ref _elVisibility, value);
        }

        #endregion

        #endregion

        #region selectedFolder : Folder - SelectedFoler

        ///<summary>SelectedFolder</summary>
        private SimpleFolder _selectedFolder;

        ///<summary>Selected Folder</summary>
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

        #region isMin : bool - Is min  

        ///<summary>Is min  </summary>
        private bool _isMin = false;

        ///<summary>Is min  </summary>
        public bool IsMin
        {
            get => _isMin;
            set => Set(ref _isMin, value);
        }

        #endregion

        #region selectedIndex : int - Selected video index

        ///<summary>Selected video index</summary>
        private int _selectedIndex;

        ///<summary>Selected video index</summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }

        #endregion

        #region Commands

        #region OpenVideoCommand

        public ICommand OpenVideoCommand { get; }

        
        private void OnOpenVideoCommandExecuted(object p)
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All Media Files|*.avi;*.mp4;*.mkv;";

            if (fileDialog.ShowDialog() == false) return;
            if (CurrentFolder != null) _videoDurationXmlSaver.SaveToXml(CurrentFolder);

            string folderName = Path.GetDirectoryName(fileDialog.FileName);

            CurrentFolder = new Folder
            {
                Name = folderName,
                Path = fileDialog.FileName,
                Videos = VideoService.GetVideos(folderName)
            };

            CurrentPlayedVideo = SelectedVideo = CurrentFolder.Videos.Find(v => v.Path.OriginalString == fileDialog.FileName);
            SetStoppedTime();
        }

        private bool CanOpenVideoCommandExecute(object p) => true;

        #endregion

        #region SetMouseStateCommand

        public ICommand SetMouseStateCommand { get; }

        private void OnSetMouseStateCommandExecuted(object p)
        {
            IsVideoElementHovered = bool.Parse(p.ToString());
        }

        private bool CanSetMouseStateCommandExecute(object p) => true;

        #endregion

        #region ChangeCurrentVideo

        public ICommand ChangeCurrentVideo { get; }

        private void OnChangeCurrentVideoExecuted(object p)
        {
            if(SelectedVideo.Equals(CurrentPlayedVideo)) return;
            CurrentPlayedVideo = SelectedVideo;
            
            SetStoppedTime();
        }

        private bool CanChangeCurrentVideoExecute(object p) => true;

        #endregion

        #region ChangeCurrentFolder 

        public ICommand ChangeCurrentFolder  { get; }

        private void OnChangeCurrentFolderExecuted(object p)
        {
            LastFolder = new SimpleFolder {Name = CurrentFolder.Name, Path = CurrentFolder.Path};
            _lastFolders.Push(LastFolder);

            _videoDurationXmlSaver.SaveToXml(CurrentFolder);

            ChangeCurrentFolderWith(SelectedFolder);
        }

        private bool CanChangeCurrentFolderExecute(object p) => true;

        #endregion

        #region OpenFolderCommand

        public ICommand OpenFolderCommand { get; }

        private void OnOpenFolderCommandExecuted(object p)
        {
            var dlg = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            
            if (dlg.ShowDialog() == CommonFileDialogResult.Cancel) return;

            Folder folder1 = new Folder
            {
                Folders = FolderService.GetFolders(dlg.FileName),
                Videos = VideoService.GetVideos(dlg.FileName),
                Name = dlg.FileName,
                Path = dlg.FileName
            };

            CurrentFolder = folder1;
        }

        private bool CanOpenFolderCommandExecute(object p) => true;

        #endregion

        #region ChangeVisibilityCommand

        public ICommand ChangeVisibilityCommand { get; }

        private void OnChangeVisibilityCommandExecuted(object p)
        {
            if (!_isHidden) return;

            Cursor = Cursors.Arrow;
            ElVisibility = Visibility.Visible;
            _timerTime = 0;
        }

        private bool CanChangeVisibilityCommandExecute(object p) => true;

        #endregion

        #region BackToLastFolderCommand

        public ICommand BackToLastFolderCommand { get; }

        private void OnBackToLastFolderCommandExecuted(object p)
        {
            LastFolder = _lastFolders.Pop();

            _videoDurationXmlSaver.SaveToXml(CurrentFolder);

            ChangeCurrentFolderWith(LastFolder);

            if (_lastFolders.Count > 0) LastFolder = _lastFolders.Peek();
            
        }

        private bool CanBackToLastFolderCommandExecute(object p) => _lastFolders.Count > 0;

        #endregion

        #region PlayPrevNextVideoCommand

        public ICommand PlayPrevNextVideoCommand { get; }

        private void OnPlayPrevNextVideoCommandExecuted(object p)
        {
            int index = CurrentFolder.Videos.FindIndex(el => el.Equals(CurrentPlayedVideo));
            
            CurrentPlayedVideo = CurrentFolder.Videos[SelectedIndex = index + Convert.ToInt32(p)];
            SetStoppedTime();
        }

        private bool CanPlayPrevNextVideoCommandExecute(object p)
        {
            if (CurrentFolder?.Videos == null) return false;

            bool correctIndex = int.TryParse(p.ToString(), out int index);
            int curIndex = CurrentFolder.Videos.FindIndex(el => el.Equals(CurrentPlayedVideo));

            return correctIndex && index + curIndex >= 0 && index + curIndex < CurrentFolder.Videos.Count;
        }

        #endregion

        #region ToggleMinCommand

        public ICommand ToggleMinCommand { get; }

        private void OnToggleMinCommandExecuted(object p)
        {
            IsMin = !IsMin;
        }

        private bool CanToggleMinCommandExecute(object p) => true;

        #region OnVideosSourceUpdateCommand

        public ICommand VideosSourceUpdateCommand { get; }

        private void OnVideosSourceUpdateCommandExecuted(object p)
        {
            Title = new Random().Next(0, 100).ToString();
        }

        private bool CanVideosSourceUpdateCommandExecute(object p) => CurrentFolder != null;

        #endregion

        #endregion

        #region ChangeCurrentPositionWithStoppedTimeCommand

        public ICommand ChangeCurrentPositionWithStoppedTimeCommand { get; }

        private void OnChangeCurrentPositionWithStoppedTimeCommandExecuted(object p)
        {
            CurrentPlayedVideo.CurrentPosition = _videoDurationXmlSaver.GetDurationFromXml(CurrentPlayedVideo.Path.OriginalString,
                CurrentPlayedVideo.Name);

            CurrentPlayedVideo.CanPositionBeChangedToStoppedTime = false;
            CurrentPlayedVideo.IsPositionChanged = true;

            OnPropertyChanged("CurrentPlayedVideo");
        }

        private bool CanChangeCurrentPositionWithStoppedTimeCommandExecute(object p)
        {
            return CurrentPlayedVideo != null && 
                   _videoDurationXmlSaver.GetDurationFromXml(CurrentPlayedVideo.Path.OriginalString,
                CurrentPlayedVideo.Name) != null;
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            OpenVideoCommand = new ActionCommand(OnOpenVideoCommandExecuted, CanOpenVideoCommandExecute);
            ChangeCurrentVideo = new ActionCommand(OnChangeCurrentVideoExecuted, CanChangeCurrentVideoExecute);
            OpenFolderCommand = new ActionCommand(OnOpenFolderCommandExecuted, CanOpenFolderCommandExecute);
            ChangeCurrentFolder = new ActionCommand(OnChangeCurrentFolderExecuted, CanChangeCurrentFolderExecute);
            BackToLastFolderCommand =
                new ActionCommand(OnBackToLastFolderCommandExecuted, CanBackToLastFolderCommandExecute);
            _lastFolders = new Stack<SimpleFolder>();
            ToggleMinCommand = new ActionCommand(OnToggleMinCommandExecuted, CanToggleMinCommandExecute);
            PlayPrevNextVideoCommand =
                new ActionCommand(OnPlayPrevNextVideoCommandExecuted, CanPlayPrevNextVideoCommandExecute);
            SetMouseStateCommand = new ActionCommand(OnSetMouseStateCommandExecuted, CanSetMouseStateCommandExecute);
            ChangeVisibilityCommand =
                new ActionCommand(OnChangeVisibilityCommandExecuted, CanChangeVisibilityCommandExecute);
            VideosSourceUpdateCommand =
                new ActionCommand(OnVideosSourceUpdateCommandExecuted, CanVideosSourceUpdateCommandExecute);

            ChangeCurrentPositionWithStoppedTimeCommand = new ActionCommand(
                OnChangeCurrentPositionWithStoppedTimeCommandExecuted,
                CanChangeCurrentPositionWithStoppedTimeCommandExecute);

            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)};

            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void SetStoppedTime()
        {
            CurrentPlayedVideo.StoppedPosition =
                _videoDurationXmlSaver.GetDurationFromXml(CurrentPlayedVideo.Path.LocalPath, CurrentPlayedVideo.Name);

            if (CurrentPlayedVideo.StoppedPosition != null && !CurrentPlayedVideo.IsPositionChanged)
                CurrentPlayedVideo.CanPositionBeChangedToStoppedTime = true;
        }

        private void ChangeCurrentFolderWith(IFolder folder)
        {
            CurrentPlayedVideo = null;

            Folder folder1 = new Folder
            {
                Folders = FolderService.GetFolders(folder.Path),
                Videos = VideoService.GetVideos(folder.Path),
                Name = folder.Name,
                Path = folder.Path
            };

            CurrentFolder = folder1;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if(CurrentPlayedVideo == null) return;
            
            if(!IsVideoElementHovered)
            {
                _timerTime = 0;
                return;
            }
            
            _timerTime++;

            if (_timerTime > 3)
            {
                ElVisibility = Visibility.Hidden;
                _isHidden = true;
                Cursor = Cursors.None;
            }
        }
    }
}
