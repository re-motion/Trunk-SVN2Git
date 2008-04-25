using System;
using Remotion.Mixins;
using Remotion.Mixins.Samples.Aktology.Akten;

namespace Remotion.Mixins.Samples.Aktology.Mixins
{
  public interface IFahrzeugMixin
  {
    string Type { get; set; }
    string Kennzeichen { get; set; }
  }

  public class FahrzeugMixin : IFahrzeugMixin // Wir könnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber für dieses Mixin nicht nötig
  {
    private string _type;
    private string _kennzeichen;

    public string Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public string Kennzeichen
    {
      get { return _kennzeichen; }
      set { _kennzeichen = value; }
    }
  }
}
