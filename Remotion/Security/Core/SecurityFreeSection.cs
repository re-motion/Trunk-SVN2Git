// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
  /// <summary>
  /// Represents a scope within no security will be evaluated.
  /// </summary>
  public sealed class SecurityFreeSection : IDisposable
  {
    private class ActiveSections
    {
      public int Count;
    }

    private static readonly string s_activeSectionCountKey = SafeContextKeys.SecuritySecurityFreeSection;

    public static bool IsActive
    {
      get { return ActiveSectionCount > 0; }
    }

    private static int ActiveSectionCount
    {
      get
      {
        lock (typeof (ActiveSections))
        {
          var activeSections = (ActiveSections) SafeContext.Instance.GetData (s_activeSectionCountKey);
          if (activeSections == null)
          {
            activeSections = new ActiveSections();
            SafeContext.Instance.SetData (s_activeSectionCountKey, activeSections);
          }

          return activeSections.Count;
        }
      }
      set
      {
        lock (typeof (ActiveSections))
        {
          SafeContext.Instance.SetData (s_activeSectionCountKey, new ActiveSections { Count = value });
        }
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
