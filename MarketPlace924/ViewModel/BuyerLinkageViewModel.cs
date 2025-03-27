using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml;

namespace MarketPlace924.ViewModel;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class BuyerLinkageViewModel : INotifyPropertyChanged
{
    private BuyerLinkageStatus _status = BuyerLinkageStatus.Possible;
    private Visibility _requestSyncVsbl = Visibility.Collapsed;
    private Visibility _unsyncVsbl = Visibility.Collapsed;
    private Visibility _acceptVsbl = Visibility.Collapsed;
    private Visibility _declineVsbl = Visibility.Collapsed;

    public BuyerService Service { get; set; } = null!;

    public Buyer UserBuyer { get; set; } = null!;
    public Buyer LinkedBuyer { get; set; } = null!;
    public string DisplayName { get; private set; } = null!;

    public IOnBuyerLinkageUpdatedCallback LinkageUpdatedCallback { get; set; } = null!;

    public BuyerLinkageStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            UpdateDisplayName();
            _requestSyncVsbl = Visibility.Collapsed;
            _unsyncVsbl = Visibility.Collapsed;
            _acceptVsbl = Visibility.Collapsed;
            _declineVsbl = Visibility.Collapsed;
            if (_status == BuyerLinkageStatus.Possible)
            {
                _requestSyncVsbl = Visibility.Visible;
            }
            else if (_status == BuyerLinkageStatus.PendingSelf)
            {
                _acceptVsbl = Visibility.Visible;
                _declineVsbl = Visibility.Visible;
            }
            else if (_status == BuyerLinkageStatus.PendingOther || _status == BuyerLinkageStatus.Confirmed)
            {
                _unsyncVsbl = Visibility.Visible;
            }

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(RequestSyncVsbl));
            OnPropertyChanged(nameof(UnsyncVsbl));
            OnPropertyChanged(nameof(AcceptVsbl));
            OnPropertyChanged(nameof(DeclineVsbl));
        }
    }


    private void UpdateDisplayName()
    {
        if (_status == BuyerLinkageStatus.Possible || _status == BuyerLinkageStatus.PendingOther)
        {
            DisplayName = KeepFirstLetter(LinkedBuyer.FirstName) + " " + KeepFirstLetter(LinkedBuyer.LastName);
        }
        else
        {
            DisplayName = LinkedBuyer.FirstName + " " + LinkedBuyer.LastName;
        }

        OnPropertyChanged(nameof(DisplayName));
    }

    private static string KeepFirstLetter(string name)
    {
        return name[0].ToString().ToUpper() + new string('*', name.Length - 1);
    }


    public Visibility RequestSyncVsbl => _requestSyncVsbl;
    public Visibility UnsyncVsbl => _unsyncVsbl;
    public Visibility AcceptVsbl => _acceptVsbl;
    public Visibility DeclineVsbl => _declineVsbl;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task RequestSync()
    {
        await Service.CreateLinkageRequest(UserBuyer, LinkedBuyer);
        Status = BuyerLinkageStatus.PendingOther;
    }

    public async Task Unsync()
    {
        if (_status == BuyerLinkageStatus.Confirmed)
        {
            await Service.BreakLinkage(UserBuyer, LinkedBuyer);
        }

        if (_status == BuyerLinkageStatus.PendingOther)
        {
            await Service.CancelLinkageRequest(UserBuyer, LinkedBuyer);
            
        }
        await LinkageUpdatedCallback.OnBuyerLinkageUpdated();
        Status = BuyerLinkageStatus.Possible;
    }

    public async Task Accept()
    {
        await Service.AcceptLinkageRequest(UserBuyer, LinkedBuyer);
        Status = BuyerLinkageStatus.Confirmed;
        await LinkageUpdatedCallback.OnBuyerLinkageUpdated();
    }

    public async Task Decline()
    {
        if (_status == BuyerLinkageStatus.PendingSelf)
        {
            await Service.RefuseLinkageRequest(UserBuyer, LinkedBuyer);
            await LinkageUpdatedCallback.OnBuyerLinkageUpdated();
        }

        Status = BuyerLinkageStatus.Possible;
    }
}