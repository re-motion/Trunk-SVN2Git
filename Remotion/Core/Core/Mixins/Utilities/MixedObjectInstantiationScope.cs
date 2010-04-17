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
using System.Runtime.Remoting.Messaging;
using Remotion.Context;
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
  /// This class is a singleton bound to the current <see cref="SafeContext"/>.
  /// </para>
  /// </remarks>
  public class MixedObjectInstantiationScope : IDisposable
  {
    private static readonly SafeContextSingleton<MixedObjectInstantiationScope> s_instance =
        new SafeContextSingleton<MixedObjectInstantiationScope> (
            SafeContextKeys.MixinsObjectInstantiationScope, () => new MixedObjectInstantiationScope());

    public static MixedObjectInstantiationScope Current
    {
      get { return s_instance.Current; }
    }

    public static bool HasCurrent
    {
      get { return s_instance.HasCurrent; }
    }

    public static void SetCurrent (MixedObjectInstantiationScope value)
    {
      s_instance.SetCurrent (value);
    }

    private readonly object[] _suppliedMixinInstances;

    private MixedObjectInstantiationScope _previous;
    private bool _isDisposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope will not contain any pre-created mixin instances.
    /// </summary>
    public MixedObjectInstantiationScope ()
    {
      StorePreviousAndSetCurrent ();
      _suppliedMixinInstances = new object[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixedObjectInstantiationScope"/> class, setting it as the
    /// <see cref="Current"/> scope object. The previous scope is restored when this scope's <see cref="Dispose"/>
    /// method is called, e.g. from a <c>using</c> statement. The new scope contains the specified pre-created mixin instances.
    /// </summary>
    /// <param name="suppliedMixinInstances">The mixin instances to be used when a mixed type is instantiated from within the scope. The objects
    /// specified must fit the mixin types specified in the mixed type's configuration. Users can also specify instances for a subset of the mixin
    /// types, the remaining ones will be created on demand.</param>
    public MixedObjectInstantiationScope (params object[] suppliedMixinInstances)
    {
      StorePreviousAndSetCurrent ();
      _suppliedMixinInstances = suppliedMixinInstances;
    }

    public bool IsDisposed
    {
      get { return _isDisposed; }
    }

    /// <summary>
    /// The mixin instances to be used when a mixed class is instantiated from within the scope.
    /// </summary>
    public object[] SuppliedMixinInstances
    {
      get { return _suppliedMixinInstances; }
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
