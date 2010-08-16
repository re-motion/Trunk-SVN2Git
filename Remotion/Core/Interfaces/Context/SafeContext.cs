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
using Remotion.BridgeInterfaces;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Context
{
  /// <summary>
  /// Superior alternative to the <see cref="ThreadStaticAttribute"/> and <see cref="CallContext"/> for making member variables thread safe that 
  /// also works with ASP.NET threads.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The data managed by this class is by default stored in the <see cref="CallContext"/>, but the storage provider can be replaced by application 
  /// code if needed. Replacements for the storage provider must guarantee that all data stored by the <see cref="SafeContext"/> is thread-local.
  /// </para>
  /// <para>
  /// The Remotion.Web assembly by default replaces the storage provider with one that stores all data in the <see cref="O:System.Web.HttpContext"/>. 
  /// This ensures that <see cref="SafeContext"/> works as expected in ASP.NET environments when a session migrates between threads.
  /// </para>
  /// </remarks>
  /// <threadsafety>
  /// The data managed by this class is thread-local. The class is safe to be used from multiple threads at the same time, but each thread will have 
  /// its own copy of the data.
  /// </threadsafety>
  public class SafeContext
  {
    private static readonly object s_lock = new object ();
    private static ISafeContextStorageProvider s_instance;

    public static ISafeContextStorageProvider Instance
    {
      get
      {
        lock (s_lock)
        {
          if (s_instance == null)
          {
            // set temporary context so that mixins can be used
            IBootstrapStorageProvider bootstrapStorageProvider = SafeServiceLocator.Current.GetInstance < IBootstrapStorageProvider>();
            s_instance = bootstrapStorageProvider;
            
            // then determine the actual context to be used
            s_instance = ObjectFactory.Create<SafeContext> (ParamList.Empty).GetDefaultInstance();
          }
          return s_instance;
        }
      }
    }

    public static void SetInstance (ISafeContextStorageProvider newInstance)
    {
      lock (s_lock)
      {
        s_instance = newInstance;
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
#pragma warning disable 168
      object bootstrapperInstance = Instance;
#pragma warning restore 168
      return SafeServiceLocator.Current.GetInstance <ICallContextStorageProvider>();
    }
  }
}
