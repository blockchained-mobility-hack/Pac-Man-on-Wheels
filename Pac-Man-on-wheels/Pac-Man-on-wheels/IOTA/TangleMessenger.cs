namespace Pac_Man_on_wheels.IOTA
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using RestSharp;

  using Tangle.Net.Entity;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class TangleMessenger
  {
    private const int Depth = 8;

    private readonly Seed seed;

    private IIotaRepository repository;

    public TangleMessenger(Seed seed, int minWeightMagnitude = 14)
    {
      this.seed = seed;
      this.MinWeight = minWeightMagnitude;
      var iotaClient = new RestIotaClient(new RestClient("https://nodes.testnet.iota.org:443"));
      this.repository = new RestIotaRepository(iotaClient, new PoWService(new CpuPearlDiver()));
    }

    private int MinWeight { get; }

    public async Task<List<string>> GetMessagesAsync(string addresse)
    {
      var messagesList = new List<string>();
      try
      {
        var addresses = new List<Address> { new Address(addresse) };
        var transactions = await this.repository.FindTransactionsByAddressesAsync(addresses);

        foreach (var transactionsHash in transactions.Hashes)
        {
          var bundle = await this.repository.GetBundleAsync(transactionsHash);
          var message = bundle.GetMessages();
          messagesList.Add(message[0]);
        }
      }
      catch
      {
        // ignored
      }

      return messagesList;
    }
  }
}
