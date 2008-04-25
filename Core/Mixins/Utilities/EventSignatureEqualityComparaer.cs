using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class EventSignatureEqualityComparer : IEqualityComparer<EventInfo>
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public bool Equals (EventInfo x, EventInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else 
        return s_signatureChecker.EventSignaturesMatch (x, y);
    }

    public int GetHashCode (EventInfo eventInfo)
    {
      ArgumentUtility.CheckNotNull ("eventInfo", eventInfo);

      return MethodSignatureEqualityComparer.GetSafeType (eventInfo.EventHandlerType).GetHashCode();
    }
  }
}
