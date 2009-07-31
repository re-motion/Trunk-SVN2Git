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
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.Collections;
using Remotion.Diagnostics.ToText;

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
  public class StableBindingProxyProviderCachedUsingDictionary
  {
    protected static void SetProxiedFieldValue (object proxy, object value)
    {
      var proxiedField = GetProxiedField (proxy);
      proxiedField.SetValue (proxy, value);
    }

    protected static FieldInfo GetProxiedField (object proxy)
    {
      Type proxyType = GetActualType (proxy);
      return proxyType.GetField ("_proxied", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    protected static Type GetActualType (object proxy)
    {
      var objectGetType = typeof (object).GetMethod ("GetType");
      return (Type) objectGetType.Invoke (proxy, new object[0]);
    }

    protected readonly ITypeFilter _typeFilter;
    protected readonly ModuleScope _moduleScope;
    protected readonly Dictionary<Type, Type> _proxiedTypeToProxyTypeCache = new Dictionary<Type, Type> ();
    protected readonly Dictionary<Type, object> _proxiedTypeToProxyCache = new Dictionary<Type, object> ();
    protected readonly Dictionary<Tuple<Type, string>, object> _proxiedTypeToAttributeProxyCache = new Dictionary<Tuple<Type, string>, object> ();

    public StableBindingProxyProviderCachedUsingDictionary (ITypeFilter typeFilter, ModuleScope moduleScope)
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
      get
      {
        return _moduleScope;
      }
    }

    //public object GetAttributeProxy (Object proxied, string attributeName)
    //{
    //  object proxy = BuildProxy(proxied);
    //  var typeMemberProxy = ScriptingHost.GetScriptEngine(ScriptLanguageType.Python).Operations.GetMember (proxy, attributeName);
    //  return typeMemberProxy;
    //}

    public object GetAttributeProxy (Object proxied, string attributeName)
    {
      object proxy = GetProxy (proxied);

      //var attributeProxy = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python).Operations.GetMember (proxy, attributeName);

      var key = new Tuple<Type, string> (proxied.GetType(), attributeName);
      Object attributeProxy;
      if(!_proxiedTypeToAttributeProxyCache.TryGetValue (key, out attributeProxy))
      {
        attributeProxy = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python).Operations.GetMember (proxy, attributeName);
        _proxiedTypeToAttributeProxyCache[key] = attributeProxy;
      }

      return attributeProxy;
    }

    protected Type BuildProxyType (Type proxiedType)
    {
      //To.ConsoleLine.s ("BuildProxyType: ").e (proxiedType);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, _typeFilter, _moduleScope);
      return stableBindingProxyBuilder.BuildProxyType ();
    }

    protected Type GetProxyType (Type proxiedType)
    {
      //Type proxyType = _proxiedTypeToProxyTypeCache.GetOrCreateValue (proxiedType, BuildProxyType);

      var key = proxiedType;
      Type proxyType;
      if (!_proxiedTypeToProxyTypeCache.TryGetValue (key, out proxyType))
      {
        proxyType = BuildProxyType (proxiedType);
        _proxiedTypeToProxyTypeCache[key] = proxyType;
      }

      return proxyType;
    }


    //protected object BuildProxy (object proxied)
    //{
    //  Type proxyType = GetProxyType (proxied.GetType());
    //  return Activator.CreateInstance (proxyType, proxied);
    //}

    protected object BuildProxy (object proxied)
    {
      //To.ConsoleLine.s ("BuildProxy: ").e (proxied);
      Type proxyType = GetProxyType (proxied.GetType ());
      var proxy = Activator.CreateInstance (proxyType, proxied);
      // Set proxied member in proxy to null, so it will not keep the proxied object alive.
      SetProxiedFieldValue (proxy, null);
      return proxy;
    }


    protected object GetProxy (object proxied)
    {
      //object proxy = _proxiedTypeToProxyCache.GetOrCreateValue (proxied.GetType (), dummyKey => BuildProxy (proxied));

      var key = proxied.GetType ();
      object proxy;
      if (!_proxiedTypeToProxyCache.TryGetValue (key, out proxy))
      {
        proxy = BuildProxy (proxied);
        _proxiedTypeToProxyCache[key] = proxy;
      }


      SetProxiedFieldValue (proxy, proxied);
      return proxy;
    }


  }
}