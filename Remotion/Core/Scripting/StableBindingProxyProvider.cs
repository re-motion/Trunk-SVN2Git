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
using Remotion.Collections;

namespace Remotion.Scripting
{
  /// <summary>
  /// Creates and caches forwarding proxy objects which expose only the members known in the current <see cref="ScriptContext"/>.
  /// </summary>
  /// <remarks>
  /// Used by the re-motion mixin engine to present only the members of a class known in the current <see cref="ScriptContext"/>
  /// to the Dynamic Language Runtime, thereby guaranteeing that mixins coming from different re-motion modules do not 
  /// interfere with the mixins and scripts coming from a specific module.
  /// </remarks>
  public class StableBindingProxyProvider
  {
    private readonly ITypeFilter _typeFilter;
    private readonly ModuleScope _moduleScope;
    private readonly Cache<Type, Type> _proxiedTypeToProxyTypeMap = new Cache<Type, Type> ();

    public StableBindingProxyProvider (ITypeFilter typeFilter, ModuleScope moduleScope)
    {
      _typeFilter = typeFilter;
      _moduleScope = moduleScope;
    }

    public ITypeFilter TypeFilter
    {
      get { return _typeFilter; }
    }

    public ModuleScope ModuleScope
    {
      get {
        return _moduleScope;
      }
    }

    // TODO: Store proxyType in map (and maybe result of pythonScriptEngine.Operations.GetMember wrapping operation in cache).
    public object GetMemberProxy (Object proxied, string attributeName)
    {
      object proxy = BuildProxy(proxied);
      var typeMemberProxy = ScriptingHost.GetScriptEngine(ScriptLanguageType.Python).Operations.GetMember (proxy, attributeName);
      return typeMemberProxy;
    }

    private object BuildProxy (object proxied)
    {
      Type proxyType = GetProxyType (proxied.GetType());
      return Activator.CreateInstance (proxyType, proxied);
    }

    private Type BuildProxyType (Type proxiedType)
    {
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, _typeFilter, _moduleScope);
      return stableBindingProxyBuilder.BuildProxyType();
    }

    private Type GetProxyType (Type proxiedType)
    {
      Type proxyType = _proxiedTypeToProxyTypeMap.GetOrCreateValue (proxiedType, BuildProxyType);
      return proxyType;
    }


    //private void SetProxied (Object proxy, Object proxied)
    //{
    //  proxy
    //  //_proxied
    //}
  }
}