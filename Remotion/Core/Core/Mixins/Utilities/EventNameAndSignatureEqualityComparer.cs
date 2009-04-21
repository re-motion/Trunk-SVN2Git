// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
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
