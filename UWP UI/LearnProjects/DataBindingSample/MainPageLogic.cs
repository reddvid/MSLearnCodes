using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace DataBindingSample
{
    public class MainPageLogic : ObservableObject
    {
        private DispatcherTimer _timer;
        public string CurrentTime => DateTime.Now.ToLongTimeString();
        public bool IsSubmitAllowed => UserName?.Trim().Length > 2;
        
        public MainPageLogic()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (sender, o) => RaisePropertyChanged(nameof(CurrentTime));
            _timer.Start();
        }

        private string _userName;

        public string UserName
        {
            get { return _userName;  }
            set
            {
                _userName = value;
                RaisePropertyChanged(nameof(IsSubmitAllowed));
            }
        }

        private bool _isNameNeeded = true;

        public bool IsNameNeeded
        {
            get { return _isNameNeeded; }
            set { Set(ref _isNameNeeded, value); }
        }

        public Visibility GetGreetingVisibility()
        {
            return IsNameNeeded ? Visibility.Collapsed : Visibility.Visible;
        }

        public void Submit()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return;
            }

            IsNameNeeded = false;
            RaisePropertyChanged(nameof(GetGreetingVisibility));
        }
    }
}
