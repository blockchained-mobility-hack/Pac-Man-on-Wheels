﻿namespace Pac_Man_on_wheels.ViewModel
{
  using System.Diagnostics;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Newtonsoft.Json;

  using Pac_Man_on_wheels.IOTA;
  using Pac_Man_on_wheels.IPFS;
  using Pac_Man_on_wheels.Models;
  using Pac_Man_on_wheels.NTRU;
  using Pac_Man_on_wheels.Services;

  using Tangle.Net.Entity;

  using Xamarin.Forms;

  public class MenuViewModel : BaseViewModel
  {
    private readonly User user;

    private readonly TangleMessenger tangle;

    private readonly IpfsHelper ipfsHelper;

    private readonly NtruKex ntru;

    private string earnings = "0 Mi (0 €)"; // "500 Mi (433 €)"

    private string distance = "0 m"; // "500 km"

    private string loadingText = "PoW in progress...";

    public MenuViewModel(User user)
    {
      this.user = user;
      this.tangle = new TangleMessenger(this.user.Seed);
      this.ntru = new NtruKex();
      this.ipfsHelper = new IpfsHelper();
    }

    public string Earnings
    {
      get => this.earnings ?? string.Empty;
      set
      {
        this.earnings = value;
        this.RaisePropertyChanged();
      }
    }

    public string Distance
    {
      get => this.distance ?? string.Empty;
      set
      {
        this.distance = value;
        this.RaisePropertyChanged();
      }
    }

    public string LoadingText
    {
      get => this.loadingText ?? string.Empty;
      set
      {
        this.loadingText = value;
        this.RaisePropertyChanged();
      }
    }

    public ICommand RefreshCommand => new Command(async () => { await this.Refresh(); });

    public ICommand SimulateCommand => new Command(async () => { await this.Simulate(); });

    public ICommand OfferCommand => new Command(async () => { await this.Offer(); });

    private async Task Refresh()
    {
      this.LoadingText = "Loading data...";
      this.IsBusy = true;

      // 1. Get Ipfs hash from Tangle
      var messageList = await this.tangle.GetMessagesAsync(this.user.DataAddress);
      
      if (messageList.Count > 0)
      {
        // 2. decrypt
        Trace.WriteLine(messageList[0]);
        var decryptedMessage = Encoding.UTF8.GetString(new NtruKex().Decrypt(this.user.NtruKeyPair, messageList[0].DecodeBytesFromTryteString()));
        Trace.WriteLine(decryptedMessage);

        // 3. Get data from ipfs node 
        var json = await this.ipfsHelper.CatString(decryptedMessage);
        var carDataJson = JsonConvert.DeserializeObject<CarDataJson>(json);

        this.Distance = carDataJson.trip.distance + " m";
      }

      this.IsBusy = false;
    }

    private async Task Simulate()
    {
      this.IsBusy = true;
      var test = this.ntru.Encrypt(this.user.NtruKeyPair.PublicKey, Encoding.UTF8.GetBytes("QmRySnruZvL3LYVw1D329tAvyE2dQbMabrvwKFjM7Pctba"));
      var tryteString = test.EncodeBytesAsTryteString();
      await this.tangle.SendMessageAsync(new TryteString(tryteString + MarkerConstants.End), this.user.DataAddress);
      this.IsBusy = false;
    }

    private async Task Offer()
    {
      this.Earnings = "0,07 Mi (0,06 €)";
    }
  }
}
