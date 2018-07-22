namespace Pac_Man_on_wheels.ViewModel
{
  using System;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Pac_Man_on_wheels.Models;
  using Pac_Man_on_wheels.NTRU;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Utils;

  using Xamarin.Forms;

  public class LoginViewModel : BaseViewModel
  {
    public Action DisplayInvalidLoginPrompt;

    private string randomSeed = Seed.Random().Value;

    private bool storeSeed;

    private User user;

    public LoginViewModel()
    {
      this.StoreSeed = true;
    }

    public bool StoreSeed
    {
      get => this.storeSeed;
      set
      {
        this.storeSeed = value;
        this.RaisePropertyChanged();
      }
    }

    public string RandomSeed
    {
      get => this.randomSeed ?? string.Empty;
      set
      {
        this.randomSeed = value;
        this.RaisePropertyChanged();
      }
    }

    public ICommand SubmitCommand => new Command(async () => { await this.Login(); });

    private async Task Login()
    {
      this.RandomSeed = this.RandomSeed.Trim();
      if (!InputValidator.IsTrytes(this.RandomSeed))
      {
        this.DisplayInvalidLoginPrompt();
      }
      else if (!this.IsBusy)
      {
        this.IsBusy = true;

        this.user = new User { Seed = new Seed(this.randomSeed) };
        var addresses = await Task.Run(() => new AddressGenerator().GetAddresses(this.user.Seed, SecurityLevel.Medium, 0, 2));
        this.user.MoneyAddress = addresses[0].Value;
        this.user.DataAddress = addresses[1].Value;

        this.user.NtruKeyPair = new NtruKex().CreateAsymmetricKeyPair(this.user.Seed.Value, this.user.MoneyAddress); // could be any other address

        // Just for testing and reading out the addresses
        Trace.WriteLine(this.user.DataAddress);

        await this.Navigation.PushAsync(new MenuPage(this.user));
      }

      this.IsBusy = false;
    }
  }
}
