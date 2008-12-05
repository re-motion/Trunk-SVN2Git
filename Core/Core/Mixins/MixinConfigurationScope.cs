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
using System.Runtime.Remoting.Messaging;
using Remotion.Context;

namespace Remotion.Mixins
{
  internal class MixinConfigurationScope : IDisposable
  {
    private readonly MixinConfiguration _previousContext = null;
    private bool _disposed;

    public MixinConfigurationScope (MixinConfiguration previousContext)
    {
      _previousContext = previousContext;
    }

    /// <summary>
    /// When called for the first time, restores the <see cref="MixinConfiguration"/> that was the <see cref="MixinConfiguration.ActiveConfiguration"/> for the current
    /// thread (<see cref="SafeContext"/>) before this object was constructed.
    /// </summary>
    /// <remarks>
    /// After this method has been called for the first time, further calls have no effects. If the <see cref="IDisposable.Dispose"/> method is not called, the
    /// original configuration will not be restored by this object.
    /// </remarks>
    void IDisposable.Dispose ()
    {
      if (!_disposed)
      {
        MixinConfiguration.SetActiveConfiguration (_previousContext);
        _disposed = true;
      }
    }
  }
}
