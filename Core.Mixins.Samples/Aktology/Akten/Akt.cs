using System;

namespace Remotion.Mixins.Samples.Aktology.Akten
{
  public interface IAkt
  {
    string ID { get; set; }
    string Inhalt { get; set; }
  }

  public class Akt : IAkt
  {
    private string _id;
    private string _inhalt;

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public string Inhalt
    {
      get { return _inhalt; }
      set { _inhalt = value; }
    }
  }
}
