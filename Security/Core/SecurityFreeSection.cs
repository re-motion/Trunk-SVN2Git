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
