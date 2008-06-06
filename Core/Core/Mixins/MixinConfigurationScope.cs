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
using System.Runtime.Remoting.Messaging;

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
    /// thread (<see cref="CallContext"/>) before this object was constructed.
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
