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
using System.Collections.Generic;
using Remotion.Context;
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
    private const string c_scriptContextCurrentSafeContextTag = "Remotion.Scripting.ScriptContext.Current";
    
    private static readonly Dictionary<string ,ScriptContext> s_scriptContexts = new Dictionary<string, ScriptContext>();
    private static readonly Object s_scriptContextLock = new object();

    /// <summary>
    /// The currently active <see cref="ScriptContext"/>. Thread safe through <see cref="SafeContext"/>.
    /// </summary>
    public static ScriptContext Current
    {
      get { return CurrentScriptContext; }
    }

    /// <summary>
    /// Returns the DLR proxy object for the proxied objects member with the passed name.
    /// </summary>
    public static object GetAttributeProxy (object proxied, string attributeName)
    {
      return ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);
    }

   
    /// <summary>
    /// Switches to the passed <see cref="ScriptContext"/>. All re-motion script classes (e.g. <see cref="ExpressionScript{TResult}"/>, 
    /// <see cref="ScriptFunction{TFixedArg1,TResult}"/>, etc) do this automatically before executing their DLR script.
    /// </summary>
    public static void SwitchAndHoldScriptContext(ScriptContext newScriptContex)
    {
      // Note: Currently switching to the same ScriptContext twice is not supported. 
      // (Would need to use a stack of ScriptContext|s to support interleaving of ScriptContext|s).
      ArgumentUtility.CheckNotNull ("newScriptContex", newScriptContex);
      Assertion.IsNull (CurrentScriptContext, CurrentScriptContext == null ? "" : String.Format ("ReleaseScriptContext: There is already an active script context ('{0}') on this thread.", CurrentScriptContext.Name));
      CurrentScriptContext = newScriptContex;
    }

    /// <summary>
    /// Releases the the passed <see cref="ScriptContext"/>. All re-motion script classes (e.g. <see cref="ExpressionScript{TResult}"/>, 
    /// <see cref="ScriptFunction{TFixedArg1,TResult}"/>, etc) do this automatically after having executed their DLR script.
    /// </summary>
    public static void ReleaseScriptContext (ScriptContext scriptContexToRelease)
    {
      ArgumentUtility.CheckNotNull ("scriptContexToRelease", scriptContexToRelease);
      if (!Object.ReferenceEquals (scriptContexToRelease, CurrentScriptContext))
      {
        throw new InvalidOperationException (String.Format("Tried to release script context '{0}' while active script context was '{1}'.", scriptContexToRelease.Name, CurrentScriptContext));
      }
      CurrentScriptContext = null;
    }

    /// <summary>
    /// Creates a new <see cref="ScriptContext"/>.
    /// </summary>
    /// <param name="name">The tag name of the <see cref="ScriptContext"/>. Must be unique. Suggested naming scheme: 
    /// company domain name + namespace of module (e.g. "rubicon.eu.Remotion.Data.DomainObjects.Scripting", "microsoft.com.Word.Scripting").
    /// </param>
    /// <param name="typeFilter">The <see cref="ITypeFilter"/> which decides which <see cref="Type"/>|s are known in the <see cref="ScriptContext"/>.</param>
    public static ScriptContext Create (string name, ITypeFilter typeFilter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);
      lock (s_scriptContextLock)
      {
        return CreateScriptContextUnsafe(name, typeFilter);
      }
    }

    /// <summary>
    /// Retrieves the <see cref="ScriptContext"/> corresponding the passed <paramref name="name"/>. 
    /// </summary>
    /// <returns>The <see cref="ScriptContext"/> or null if none with the passed name has been defined using <see cref="Create"/>.</returns>
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


    private static ScriptContext CurrentScriptContext
    {
      get
      {
        return (ScriptContext) SafeContext.Instance.GetData (c_scriptContextCurrentSafeContextTag);
      }

      set
      {
        SafeContext.Instance.SetData (c_scriptContextCurrentSafeContextTag, value);
      }
    }

    private static Dictionary<string, ScriptContext> ScriptContexts
    {
      get { return s_scriptContexts; }
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

    // Test-only method
    private static void ClearScriptContexts ()
    {
      lock (s_scriptContextLock)
      {
        CurrentScriptContext = null;
        ScriptContexts.Clear ();
      }
    }

    // Test-only method
    private static void ReleaseAllScriptContexts ()
    {
      CurrentScriptContext = null;
    }

 
    private readonly string _name;
    private readonly StableBindingProxyProvider _proxyProvider;

    private ScriptContext (string name, ITypeFilter typeFilter)
    {
      _name = name;
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
