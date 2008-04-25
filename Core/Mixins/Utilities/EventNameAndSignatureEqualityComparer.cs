using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class EventNameAndSignatureEqualityComparer : IEqualityComparer<EventInfo>
  {
    private EventSignatureEqualityComparer _signatureComparer = new EventSignatureEqualityComparer();

    public bool Equals (EventInfo x, EventInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else
        return x.Name == y.Name && _signatureComparer.Equals (x, y);
    }

    public int GetHashCode (EventInfo Event)
    {
      ArgumentUtility.CheckNotNull ("Event", Event);

      return Event.Name.GetHashCode() ^ _signatureComparer.GetHashCode (Event);
    }
  }
}
