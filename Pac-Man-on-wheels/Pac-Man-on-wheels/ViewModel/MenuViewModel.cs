namespace Pac_Man_on_wheels.ViewModel
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

    private string firstButtonColor = "#fdde54";

    private string secondButtonColor = "#fdde54";

    private string thirdButtonColor = "#fdde54";

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

    public string FirstButtonColor
    {
      get => this.firstButtonColor ?? string.Empty;
      set
      {
        this.firstButtonColor = value;
        this.RaisePropertyChanged();
      }
    }

    public string SecondButtonColor
    {
      get => this.secondButtonColor ?? string.Empty;
      set
      {
        this.secondButtonColor = value;
        this.RaisePropertyChanged();
      }
    }

    public string ThirdButtonColor
    {
      get => this.thirdButtonColor ?? string.Empty;
      set
      {
        this.thirdButtonColor = value;
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

      this.SecondButtonColor = "#9ccc65";
      this.IsBusy = false;
    }

    private async Task Simulate()
    {
      this.IsBusy = true;

      // 1. Upload ipfs
      // var hash = await this.ipfsHelper.PinFile("path to file");

      // 2. Encrypt
      var encrypted = this.ntru.Encrypt(this.user.NtruKeyPair.PublicKey, Encoding.UTF8.GetBytes("QmRySnruZvL3LYVw1D329tAvyE2dQbMabrvwKFjM7Pctba"));
      var tryteString = encrypted.EncodeBytesAsTryteString();
      Trace.WriteLine("IPFS: QmRySnruZvL3LYVw1D329tAvyE2dQbMabrvwKFjM7Pctba");

      // 3. Send to Tangle
      await this.tangle.SendMessageAsync(new TryteString(tryteString + MarkerConstants.End), this.user.DataAddress);
      Trace.WriteLine("Data Address: " + this.user.DataAddress);

      this.FirstButtonColor = "#9ccc65";
      this.IsBusy = false;
    }

    private async Task Offer()
    {
      this.Earnings = "0,07 Mi (0,06 €)";
      this.ThirdButtonColor = "#9ccc65";
    }
  }
}
