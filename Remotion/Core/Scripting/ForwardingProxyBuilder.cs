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
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Builds a proxy object which exposes only selected methods/properties, as decided by its <see cref="ITypeArbiter"/>. 
  /// </summary>
  /// <remarks>
  /// What methods/properties are to be exposed is dependent on whether the method/property comes from a type which is
  /// classified as "valid" by the <see cref="ITypeArbiter"/> of the class.
  /// <para/> 
  /// Used by <see cref="StableBindingProxyProvider"/>.
  /// </remarks>
  public class ForwardingProxyBuilder
  {
    private readonly CustomClassEmitter _classEmitter;
    private readonly FieldReference _proxied;
    private readonly Type _proxiedType;

    public ForwardingProxyBuilder (string name, ModuleScope moduleScope, Type proxiedType, Type[] interfaces)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("moduleScope", moduleScope);
      ArgumentUtility.CheckNotNull ("proxiedType", proxiedType);
      ArgumentUtility.CheckNotNullOrItemsNull ("interfaces", interfaces);

      _proxiedType = proxiedType;
      _classEmitter = CreateClassEmitter (name, interfaces, moduleScope);
      _proxied = CreateProxiedField();
      CreateProxyCtor(proxiedType);
    }

    /// <summary>
    /// Builds the proxy <see cref="Type"/> with the members added through <see cref="AddForwardingExplicitInterfaceMethod"/> etc.
    /// </summary>
    /// <returns></returns>
    public Type BuildProxyType ()
    {
      return _classEmitter.BuildType ();
    }

    /// <summary>
    /// Calls <see cref="BuildProxyType"/> and returns an instance of the generated proxy type proxying the passed <see cref="object"/>.
    /// </summary>
    /// <param name="proxied">The <see cref="object"/> to be proxied. Must be of the <see cref="Type"/> 
    /// the <see cref="ForwardingProxyBuilder"/> was initialized with.</param>
    /// <returns></returns>
    public object CreateInstance (Object proxied)
    {
      ArgumentUtility.CheckNotNullAndType ("proxied", proxied, _proxiedType);
      return Activator.CreateInstance (BuildProxyType (), proxied);
    }

    public void AddForwardingExplicitInterfaceMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      var methodEmitter = _classEmitter.CreateInterfaceMethodImplementation (methodInfo);
      methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, methodInfo.DeclaringType), methodInfo);
    }

    public void AddForwardingImplicitInterfaceMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      var methodEmitter = _classEmitter.CreatePublicInterfaceMethodImplementation (methodInfo);
      methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, methodInfo.DeclaringType), methodInfo);
    }

    // Create proxy ctor which takes proxied instance and stores it in field in class
    private void CreateProxyCtor (Type proxiedType)
    {
      var arg = new ArgumentReference (proxiedType);
      var ctor = _classEmitter.CreateConstructor (new[] { arg });
      ctor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (typeof (object).GetConstructor (Type.EmptyTypes)));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_proxied, arg.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    // Create the field which holds the proxied instance
    private FieldReference CreateProxiedField ()
    {
      return _classEmitter.CreateField ("_proxied", _proxiedType, FieldAttributes.Private | FieldAttributes.InitOnly);
    }

    private CustomClassEmitter CreateClassEmitter (string name, Type[] interfaces, ModuleScope moduleScope)
    {
      return new CustomClassEmitter (
          moduleScope,
          name,
          typeof (Object),
          interfaces,
          TypeAttributes.Public | TypeAttributes.Class,
          true);
    }
  }
}