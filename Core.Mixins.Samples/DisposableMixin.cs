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
using Remotion.Mixins;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples
{
  public abstract class DisposableMixin : Mixin<object>, IDisposable
  {
    private bool _disposed = false;

    protected virtual void Dispose (bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
          CleanupManagedResources ();
        CleanupUnmanagedResources ();
        _disposed = true;
      }
    }

    protected abstract void CleanupManagedResources ();
    protected abstract void CleanupUnmanagedResources ();

    ~DisposableMixin ()
    {
      Dispose (false);
    }

    public void Dispose()
    {
      Dispose (true);
      GC.SuppressFinalize (this);
    }
  }
}
