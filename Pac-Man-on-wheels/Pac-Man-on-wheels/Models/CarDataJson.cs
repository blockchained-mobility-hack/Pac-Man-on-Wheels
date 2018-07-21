namespace Pac_Man_on_wheels.Models
{
  using System.Collections.Generic;

  public class CarDataJson
  {
    public Trip trip { get; set; }
  }


  public class Trip
  {
    public long begin { get; set; }
    public long end { get; set; }
    public int distance { get; set; }
    public List<Trajectory> trajectory { get; set; }
  }

  public class Trajectory
  {
    public object timestamp { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }
  }

  public class RootObject
  {
    public Trip trip { get; set; }
  }
}
