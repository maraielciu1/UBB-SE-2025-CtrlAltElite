using System.ComponentModel;
using MarketPlace924.Domain;

namespace MarketPlace924.ViewModel;

public sealed class BuyerAddressViewModel : INotifyPropertyChanged
    {
        private Address _address;
        public Address Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        
        public BuyerAddressViewModel(Address address)
        {
            _address = address;
            OnPropertyChanged(nameof(Address));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
