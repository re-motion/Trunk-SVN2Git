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

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryPriorityComparer : IComparer<AccessControlEntry>
  {
    public int Compare (AccessControlEntry x, AccessControlEntry y)
    {
      if (x == null && y == null)
        return 0;

      if (x == null)
        return -1;

      if (y == null)
        return 1;

      return x.ActualPriority.CompareTo (y.ActualPriority);
    }
  }
}
