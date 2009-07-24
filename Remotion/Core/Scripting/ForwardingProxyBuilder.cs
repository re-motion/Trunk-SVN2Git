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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Helper class to create a proxy object which forwards explcitely added methods/properties to its proxied instance. 
  /// </summary>
  /// <remarks>
  /// <para/> 
  /// Used by <see cref="StableBindingProxyBuilder"/>.
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
    public Type BuildProxyType ()
    {
      return _classEmitter.BuildType ();
    }

    /// <summary>
    /// Calls <see cref="BuildProxyType"/> and returns an instance of the generated proxy type proxying the passed <see cref="object"/>.
    /// </summary>
    /// <param name="proxied">The <see cref="object"/> to be proxied. Must be of the <see cref="Type"/> 
    /// the <see cref="ForwardingProxyBuilder"/> was initialized with.</param>
    public object CreateInstance (Object proxied)
    {
      ArgumentUtility.CheckNotNullAndType ("proxied", proxied, _proxiedType);
      return Activator.CreateInstance (BuildProxyType (), proxied);
    }

    public void AddForwardingExplicitInterfaceMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      var methodEmitter = _classEmitter.CreateInterfaceMethodImplementation (methodInfo);
      
      //methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, methodInfo.DeclaringType), methodInfo);
      ImplementForwardingMethod (methodInfo, methodEmitter);
    }

    public void AddForwardingImplicitInterfaceMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      var methodEmitter = _classEmitter.CreatePublicInterfaceMethodImplementation (methodInfo);

      //methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, methodInfo.DeclaringType), methodInfo);
      ImplementForwardingMethod (methodInfo, methodEmitter);
    }

    // Implement method in proxy by forwarding call to proxied instance
    private void ImplementForwardingMethod (MethodInfo methodInfo, CustomMethodEmitter methodEmitter)
    {
      //methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, methodInfo.DeclaringType), methodInfo);
      methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, _proxiedType), methodInfo);
    }


    public void AddForwardingMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      AddForwardingMethod (methodInfo, methodInfo.Name);
    }


    public void AddForwardingProperty (PropertyInfo propertyInfo)
    {
      string propertyName = propertyInfo.Name;
      var propertyEmitter = _classEmitter.CreateProperty (propertyName, PropertyKind.Instance, propertyInfo.PropertyType);
      if (propertyInfo.CanRead)
      {
        var getMethodEmitter = propertyEmitter.CreateGetMethod ();
        var proxiedGetMethodInfo = propertyInfo.GetGetMethod ();
        getMethodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, _proxiedType), proxiedGetMethodInfo);
      }

      if (propertyInfo.CanWrite)
      {
        var setMethodEmitter = propertyEmitter.CreateSetMethod ();
        var proxiedSetMethodInfo = propertyInfo.GetSetMethod ();
        setMethodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, _proxiedType), proxiedSetMethodInfo);
      }
    }


    /// <summary>
    /// Adds a forwarding method to the proxy based on the passed <see cref="MethodInfo"/>. 
    /// </summary>
    /// <remarks>
    /// Note that this works for interface methods only, if the <see cref="MethodInfo"/> comes from the interface, not the 
    /// type implementing the interface.
    /// </remarks>
    public void AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      var methodAttributes = methodInfo.Attributes;

      CustomMethodEmitter methodEmitter;


      if (methodInfo.DeclaringType.IsInterface)
      {
        methodEmitter = _classEmitter.CreateMethodOverrideOrInterfaceImplementation (
            methodInfo, true, methodAttributes & MethodAttributes.MemberAccessMask);
      }
      else
      {
        // Note: Masking the attributes with MethodAttributes.MemberAccessMask below, would remove 
        // desired attributes such as Final, Virtual and HideBySig.
        
        //methodEmitter = _classEmitter.CreateMethod (methodInfo.Name, methodAttributes); 

        methodEmitter = _classEmitter.CreateMethod (methodInfo.Name, methodAttributes & ~MethodAttributes.Virtual); 
      }

      methodEmitter.CopyParametersAndReturnType (methodInfo); 
      ImplementForwardingMethod (methodInfo, methodEmitter);
    }

    private static HashSet<MethodInfo> s_objectMethods;

    protected HashSet<MethodInfo> ObjectMethods
    {
      get
      {
        if (s_objectMethods == null)
        {
          s_objectMethods = new HashSet<MethodInfo> (typeof (Object).GetMethods (), MethodInfoEqualityComparer.Get);
        }
        return s_objectMethods;
      }
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

    private void AddForwardingMethod (MethodInfo methodInfo, string forwardingMethodName)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("forwardingMethodName", forwardingMethodName);
      var methodEmitter = _classEmitter.CreateMethod (forwardingMethodName, methodInfo.Attributes);
      methodEmitter.CopyParametersAndReturnType (methodInfo);
     
      //methodEmitter.ImplementByDelegating (new TypeReferenceWrapper (_proxied, _proxiedType), methodInfo);
      ImplementForwardingMethod (methodInfo, methodEmitter);
    }

  }
}