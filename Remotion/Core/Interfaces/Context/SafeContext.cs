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
using Remotion.BridgeInterfaces;
using Remotion.Implementation;
using Remotion.Mixins;

namespace Remotion.Context
{
  public class SafeContext
  {
    private static readonly object s_lock = new object ();
    private static ISafeContextStorageProvider _instance;

    public static ISafeContextStorageProvider Instance
    {
      get
      {
        lock (s_lock)
        {
          if (_instance == null)
          {
            // set temporary context so that mixins can be used
            IBootstrapStorageProvider bootstrapStorageProvider = VersionDependentImplementationBridge<IBootstrapStorageProvider>.Implementation;
            _instance = bootstrapStorageProvider;
            
            // then determine the actual context to be used
            _instance = ObjectFactory.Create<SafeContext>().With().GetDefaultInstance();
          }
          return _instance;
        }
      }
    }

    public static void SetInstance (ISafeContextStorageProvider newInstance)
    {
      lock (s_lock)
      {
        _instance = newInstance;
      }
    }

    /// <summary>
    /// Gets or creates the default instance to be used when the <see cref="SafeContext"/> is initialized.
    /// </summary>
    /// <returns>The default storage instance for this <see cref="SafeContext"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method can be overridden by a mixin in order to change the default <see cref="Instance"/>. While this method is executed,
    /// a temporary default <see cref="CallContext"/>-based storage provider is set active. Therefore, code executed from within
    /// <see cref="GetDefaultInstance"/> can safely access <see cref="Instance"/> without causing a stack overflow.
    /// </para>
    /// <para>
    /// However, the fact that it is temporary means that the data written into the context will not be available after <see cref="Instance"/>
    /// has been initialized (unless the new instance is also based on the <see cref="CallContext"/>). This also means that it is not possible to
    /// imperatively prepare a certain mixin configuration before the <see cref="SafeContext"/> is initialized; only the mixins present in the
    /// default mixin configuration will be considered for overriding this method.
    /// </para>
    /// </remarks>
    public virtual ISafeContextStorageProvider GetDefaultInstance ()
    {
      // assert that access to bootstrapper Instance is possible while actual Instance is initialized:
      object bootstrapperInstance = Instance;
      return VersionDependentImplementationBridge<ICallContextStorageProvider>.Implementation;
    }
  }
}
