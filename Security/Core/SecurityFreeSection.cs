// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Context;

namespace Remotion.Security
{
  public sealed class SecurityFreeSection : IDisposable
  {
    private static readonly string s_activeSectionCountKey = typeof (SecurityFreeSection).AssemblyQualifiedName + "_ActiveSectionCount";

    public static bool IsActive
    {
      get { return ActiveSectionCount > 0; }
    }

    private static int ActiveSectionCount
    {
      get
      {
        int? count = (int?) SafeContext.Instance.GetData (s_activeSectionCountKey);
        if (!count.HasValue)
        {
          count = 0;
          SafeContext.Instance.SetData (s_activeSectionCountKey, count);
        }

        return count.Value;
      }
      set
      {
        SafeContext.Instance.SetData (s_activeSectionCountKey, value);
      }
    }

    private bool _isDisposed;

    public SecurityFreeSection ()
    {
      ActiveSectionCount++;
    }

    void IDisposable.Dispose ()
    {
      Dispose ();
    }

    private void Dispose ()
    {
      if (!_isDisposed)
      {
        ActiveSectionCount--;
        _isDisposed = true;
      }
    }

    public void Leave ()
    {
      Dispose ();
    }
  }
}
