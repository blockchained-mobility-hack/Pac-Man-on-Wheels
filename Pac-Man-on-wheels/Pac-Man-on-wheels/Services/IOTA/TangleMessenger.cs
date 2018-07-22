namespace Pac_Man_on_wheels.IOTA
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading.Tasks;

  using Pac_Man_on_wheels.Models;

  using RestSharp;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;
  using Tangle.Net.Utils;

  public class TangleMessenger
  {
    private const int Depth = 8;

    private readonly Seed seed;

    private IIotaRepository repository;

    public TangleMessenger(Seed seed, int minWeightMagnitude = 9)
    {
      this.seed = seed;
      this.MinWeight = minWeightMagnitude;
      var iotaClient = new RestIotaClient(new RestClient("https://nodes.testnet.iota.org:443"));
      this.repository = new RestIotaRepository(iotaClient, new PoWService(new CpuPearlDiver()));
    }

    private int MinWeight { get; }

    public async Task<List<TryteString>> GetMessagesAsync(string addresse)
    {
      var messagesList = new List<TryteString>();
      try
      {
        var addresses = new List<Address> { new Address(addresse) };
        var transactions = await this.repository.FindTransactionsByAddressesAsync(addresses);

        foreach (var transactionsHash in transactions.Hashes)
        {
          var bundle = await this.repository.GetBundleAsync(transactionsHash);
          var message = ExtractMessage(bundle);
          messagesList.Add(message);
        }
      }
      catch
      {
        // ignored
      }

      return messagesList;
    }

    public async Task<bool> SendMessageAsync(TryteString message, string address, int retryNumber = 3)
    {
      var bundle = new Bundle();
      bundle.AddTransfer(CreateTransfer(message, address));

      try
      {
        await this.repository.SendTransferAsync(this.seed, bundle, SecurityLevel.Medium, Depth, this.MinWeight);
        return true;
      }
      catch (Exception e)
      {
        Trace.WriteLine(e);
      }


      return false;
    }

    private static Transfer CreateTransfer(TryteString message, string address)
    {
      return new Transfer
      {
        Address = new Address(address),
        Message = message,
        Tag = new Tag("PACMANFOREVER"),
        Timestamp = Timestamp.UnixSecondsTimestamp
      };
    }

    private static TryteString ExtractMessage(Bundle bundle)
    {
      var messageTrytes = string.Empty;

      // multiple message per bundle?
      foreach (var transaction in bundle.Transactions)
      {
        if (transaction.Value < 0)
        {
          continue;
        }

        if (!transaction.Fragment.IsEmpty)
        {
          messageTrytes += transaction.Fragment.Value;
        }
      }

      if (!messageTrytes.Contains(MarkerConstants.End))
      {
        return null;
      }

      var index = messageTrytes.IndexOf(MarkerConstants.End, StringComparison.Ordinal);
      return new TryteString(messageTrytes.Substring(0, index));
    }
  }
}
