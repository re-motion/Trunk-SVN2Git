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
