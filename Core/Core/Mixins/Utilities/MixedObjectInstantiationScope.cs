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
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities.Singleton;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Allows users to specify configuration settings when a mixed type is instantiated.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Usually, the mixin types configured in the <see cref="ClassContext"/> of a target class are simply instantiated when the mixed
  /// instance is constructed. Using this scope class, a user can supply pre-instantiated mixins instead.
  /// </para>
  /// <para>
  /// This is mainly for internal purposes, users should use the <see cref="ObjectFactory"/>
  /// class to instantiate mixed types.
  /// </para>
  /// <para>
  /// This class is a singleton bound to the current <see cref="CallContext"/>.
  /// </para>
  /// </remarks>
  public class MixedObjectInstantiationScope
      : CallContextSingletonBase<MixedObjectInstantiationScope, DefaultInstanceCreator<MixedObjectInstantiationScope>>, IDisposable
  {
    /// <summary>
    /// The mixin instances to be used when a mixed class is instantiated from within the scope.
    /// </summary>
    public readonly object[] SuppliedMixinInstances;

    private MixedObjectInstantiationScope _previous;
    private bool _isDisposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="CallContextSingletonBase{TSelf,TCreator}.Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope will not contain any pre-created mixin instances.
    /// </summary>
    public MixedObjectInstantiationScope ()
    {
      StorePreviousAndSetCurrent ();
      SuppliedMixinInstances = new object[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="CallContextSingletonBase{TSelf,TCreator}.Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope contains the specified pre-created mixin instances.
    /// </summary>
    /// <param name="suppliedMixinInstances">The mixin instances to be used when a mixed type is instantiated from within the scope. The objects
    /// specified must fit the mixin types specified in the mixed type's configuration. Users can also specify instances for a subset of the mixin
    /// types, the remaining ones will be created on demand.</param>
    public MixedObjectInstantiationScope (params object[] suppliedMixinInstances)
    {
      StorePreviousAndSetCurrent ();
      SuppliedMixinInstances = suppliedMixinInstances;
    }

    public bool IsDisposed
    {
      get { return _isDisposed; }
    }

    /// <summary>
    /// When called for the first time, restores the <see cref="MixedObjectInstantiationScope"/> that was in effect when this scope was created.
    /// </summary>
    public void Dispose ()
    {
      if (!_isDisposed)
      {
        RestorePrevious ();
        _isDisposed = true;
      }
    }

    private void StorePreviousAndSetCurrent ()
    {
      if (HasCurrent)
        _previous = Current;
      else
        _previous = null;

      SetCurrent (this);
    }

    private void RestorePrevious ()
    {
      SetCurrent (_previous);
    }
  }
}
