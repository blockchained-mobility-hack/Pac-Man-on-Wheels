namespace Pac_Man_on_wheels
{
  using Pac_Man_on_wheels.ViewModel;

  using Xamarin.Forms;

  /// <summary>
  /// The login page.
  /// </summary>
  public partial class LoginPage : ContentPage
  {
    public LoginPage()
    {
      this.InitializeComponent();
      NavigationPage.SetHasNavigationBar(this, false);
      var vm = new LoginViewModel { Navigation = this.Navigation };
      vm.DisplayInvalidLoginPrompt += () => this.DisplayAlert("Error", "Invalid seed, try again", "OK");
      this.BindingContext = vm;
    }
  }
}
