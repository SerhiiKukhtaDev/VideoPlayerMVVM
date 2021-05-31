using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoPlayer.Models
{
    public class Video : INotifyPropertyChanged
    {
        public bool IsPositionChanged { get; set; }

        public string Name { get; set; }

        private TimeSpan? _currentPosition;

        public TimeSpan? CurrentPosition 
        { 
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                OnPropertyChanged();
            }
        }

        public Uri Path { get; set; }

        private TimeSpan? _stoppedPosition;
        
        public TimeSpan? StoppedPosition
        {
            get => _stoppedPosition;
            set
            {
                _stoppedPosition = value;
                OnPropertyChanged();
            }
        }

        private bool _canPositionBeChangedToStoppedTime;

        public bool CanPositionBeChangedToStoppedTime
        {
            get => _canPositionBeChangedToStoppedTime;
            set
            {
                _canPositionBeChangedToStoppedTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
