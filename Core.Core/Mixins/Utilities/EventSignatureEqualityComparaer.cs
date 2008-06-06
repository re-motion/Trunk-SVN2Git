/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
