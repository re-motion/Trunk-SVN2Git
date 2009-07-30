// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Castle.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Represents a re-motion script context, which is used to isolate different re-motion modules from one another.
  /// Static members give access to the currently active script context.
  /// </summary>
  /// <remarks>
  /// <seealso cref="ScriptBase"/>
  /// </remarks>
  public class ScriptContext
  {
    [ThreadStatic]
    private static ScriptContext s_currentScriptContext;

    private static readonly Dictionary<string ,ScriptContext> s_scriptContexts = new Dictionary<string, ScriptContext>();
    private static readonly Object s_scriptContextLock = new object();


    public static ScriptContext Current
    {
      get { return s_currentScriptContext; }
    }

    public static object GetAttributeProxy (object proxied, string attributeName)
    {
      return ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);
    }



    // Note: Currently switching to the same ScriptContext twice is not supported. 
    // (Would need to use a stack of ScriptContext|s to support interleaving of ScriptContext|s).
    public static void SwitchAndHoldScriptContext(ScriptContext newScriptContex)
    {
      ArgumentUtility.CheckNotNull ("newScriptContex", newScriptContex);
      Assertion.IsNull (s_currentScriptContext, s_currentScriptContext == null ? "" : String.Format ("ReleaseScriptContext: There is already an active script context ('{0}') on this thread.", s_currentScriptContext.Name));
      s_currentScriptContext = newScriptContex;
    }

    public static void ReleaseScriptContext (ScriptContext scriptContexToRelease)
    {
      ArgumentUtility.CheckNotNull ("scriptContexToRelease", scriptContexToRelease);
      if (!Object.ReferenceEquals (scriptContexToRelease, s_currentScriptContext))
      {
        throw new InvalidOperationException (String.Format("Tried to release script context '{0}' while active script context is '{1}'.", scriptContexToRelease.Name, s_currentScriptContext));
      }
      s_currentScriptContext = null;
    }

    private static Dictionary<string, ScriptContext> ScriptContexts
    {
      get { return s_scriptContexts; }
    }

    public static ScriptContext Create (string name, ITypeFilter typeFilter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);
      lock (s_scriptContextLock)
      {
        return CreateScriptContextUnsafe(name, typeFilter);
      }
    }

    private static ScriptContext CreateScriptContextUnsafe (string name, ITypeFilter typeFilter)
    {
      if (GetScriptContext (name) != null)
      {
        throw new ArgumentException (String.Format ("ScriptContext named \"{0}\" already exists.", name));
      }
      var scriptContext = new ScriptContext (name, typeFilter);
      ScriptContexts[name] = scriptContext;
      return scriptContext;
    }

    public static ScriptContext GetScriptContext (string name)
    {
      // Note: Null-or-empty for name OK
      lock (s_scriptContextLock)
      {
        ScriptContext scriptContext;
        ScriptContexts.TryGetValue (name, out scriptContext);
        return scriptContext;
      }
    }

    // Test-only method
    private static void ClearScriptContexts ()
    {
      lock (s_scriptContextLock)
      {
        s_currentScriptContext = null;
        ScriptContexts.Clear ();
      }
    }

    // Test-only method
    private static void ReleaseAllScriptContexts ()
    {
      s_currentScriptContext = null;
    }

 
    private readonly string _name;
    //private readonly ITypeFilter _typeFilter;
    private readonly StableBindingProxyProvider _proxyProvider;

    private ScriptContext (string name, ITypeFilter typeFilter)
    {
      _name = name;
      //_typeFilter = typeFilter;
      _proxyProvider = new StableBindingProxyProvider (typeFilter, ReflectionHelper.CreateModuleScope ("Scripting.ScriptContext." + name,false));
    }

    /// <summary>
    /// The name through which this <see cref="ScriptContext"/> is uniquely identified.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// The <see cref="ITypeFilter"/> used in this <see cref="ScriptContext"/> to discern which methods/properties 
    /// shall be exposed to the DLR (see <see cref="ForwardingProxyBuilder"/>).
    /// </summary>
    public ITypeFilter TypeFilter
    {
      get { return _proxyProvider.TypeFilter; }
    }

    /// <summary>
    /// The <see cref="StableBindingProxyProvider"/> used in this <see cref="ScriptContext"/> to create stable binding proxies 
    /// (see <see cref="ForwardingProxyBuilder"/>).
    /// </summary>
    public StableBindingProxyProvider StableBindingProxyProvider
    {
      get { return _proxyProvider; }
    }

  }
}