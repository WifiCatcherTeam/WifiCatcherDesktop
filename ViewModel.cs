using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WifiCatcher.Commands;
using WifiMap;

namespace WifiCatcher
{
    class ViewModel : Notifier
    {
        private ICommand _refreshCommand;
        private ICommand _orientateCommand;

        private ObservableCollection<WifiNetworks.Network> _networks;
        private WifiNetworks _model;

        public ObservableCollection<WifiNetworks.Network> Networks
        {
            get { return _networks; }
            set
            {
                _networks = value;
                NotifyPropertyChanged("Networks");
            }
        }

        public ICommand RefreshCmd { get { return _refreshCommand; } }
        public ICommand OrientateCmd { get { return _orientateCommand; } }

        public ViewModel()
        {
            _refreshCommand = new RefreshCommand(this);
            _orientateCommand = new OrientateCommand(this);
            _model = new WifiNetworks();
            _model.Search();
            _networks = new ObservableCollection<WifiNetworks.Network>(_model.Networks);
        }

        public void OrientateCatcher()
        {
            
        }

        public void RefreshNetworks()
        {
            
        }
    }
}
