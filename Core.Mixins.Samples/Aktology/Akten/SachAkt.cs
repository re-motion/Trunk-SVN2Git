using System;

namespace Remotion.Mixins.Samples.Aktology.Akten
{
  public interface ISachAkt : IAkt
  {
    string Sache { get; set; }
  }

  public class SachAkt : Akt, ISachAkt
  {
    private string _sache;

    public string Sache
    {
      get { return _sache; }
      set { _sache = value; }
    }
  }
}
