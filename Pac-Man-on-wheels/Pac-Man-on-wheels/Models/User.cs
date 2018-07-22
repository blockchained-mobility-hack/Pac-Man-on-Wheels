namespace Pac_Man_on_wheels.Models
{
  using Tangle.Net.Entity;

  using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;

  public class User
  {
    public string MoneyAddress { get; set; }

    public string DataAddress { get; set; }

    /// <summary>
    /// Gets or sets Seed, never upload or store!
    /// </summary>
    public Seed Seed { get; set; }

    /// <summary>
    /// Gets or sets Chat Key Pair
    /// </summary>
    public IAsymmetricKeyPair NtruKeyPair { get; set; }
  }
}
