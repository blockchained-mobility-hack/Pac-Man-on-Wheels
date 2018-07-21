namespace Pac_Man_on_wheels
{
  using Pac_Man_on_wheels.Models;
  using Pac_Man_on_wheels.ViewModel;

  using Xamarin.Forms;

  /// <summary>
  /// The login page.
  /// </summary>
  public partial class MenuPage : ContentPage
  {
    public MenuPage(User user)
    {
      this.InitializeComponent();
      // NavigationPage.SetHasNavigationBar(this, false);
      var vm = new MenuViewModel(user) { Navigation = this.Navigation };
      this.BindingContext = vm;
    }
  }
}
