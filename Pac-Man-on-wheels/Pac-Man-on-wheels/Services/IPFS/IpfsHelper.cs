namespace Pac_Man_on_wheels.IPFS
{
  using System.Threading.Tasks;

  using Ipfs.Api;
  using Ipfs.CoreApi;

  public class IpfsHelper
  {
    private readonly IpfsClient ipfs;

    public IpfsHelper(string host = "https://ipfs.infura.io:5001")
    {
      this.ipfs = new IpfsClient(host);
    }

    public async Task<string> CatString(string qmName)
    {
      return await this.ipfs.FileSystem.ReadAllTextAsync(qmName);
    }

    public async Task<string> PinFile(string path)
    {
      var addFileOptions = new AddFileOptions { Pin = true };
      var test = await this.ipfs.FileSystem.AddFileAsync(path, addFileOptions);
      return test.Id.Hash.ToString();
    }
  }
}
