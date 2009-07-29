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
using Castle.DynamicProxy;

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

    // TODO: Make private
    public object BuildProxy (object proxied)
    {
      Type proxyType = BuildProxyType (proxied);
      return Activator.CreateInstance (proxyType, proxied);
    }

    // TODO: Make private

    public Type BuildProxyType (object proxied)
    {
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxied.GetType(), _typeFilter, _moduleScope);
      return stableBindingProxyBuilder.BuildProxyType();
    }

    //private void SetProxied (Object proxy, Object proxied)
    //{
    //  proxy
    //  //_proxied
    //}
  }
}