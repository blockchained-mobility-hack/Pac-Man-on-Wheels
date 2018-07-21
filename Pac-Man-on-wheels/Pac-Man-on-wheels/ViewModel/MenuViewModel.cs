namespace Pac_Man_on_wheels.ViewModel
{
  using System.Diagnostics;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Newtonsoft.Json;

  using Pac_Man_on_wheels.IOTA;
  using Pac_Man_on_wheels.IPFS;
  using Pac_Man_on_wheels.Models;

  using Xamarin.Forms;

  public class MenuViewModel : BaseViewModel
  {
    private readonly User user;

    private readonly TangleMessenger tangle;

    private readonly IpfsHelper ipfsHelper;

    private string earnings = "0 Mi (0 €)"; // "500 Mi (433 €)"

    private string distance = "0 m"; // "500 km"

    public MenuViewModel(User user)
    {
      this.user = user;
      this.tangle = new TangleMessenger(this.user.Seed);
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

    public ICommand RefreshCommand => new Command(async () => { await this.Refresh(); });

    private async Task Refresh()
    {
      this.IsBusy = true;

      // Get Ipfs hash from Tangle
      var messageList = await this.tangle.GetMessagesAsync(this.user.DataAddress);

      if (messageList.Count > 0)
      {
        // Get data from ipfs node 
        var json = await this.ipfsHelper.CatString(messageList[0]);
        var carDataJson = JsonConvert.DeserializeObject<CarDataJson>(json);

        this.Distance = carDataJson.trip.distance + " m";
      }

      this.IsBusy = false;
    }
  }
}
