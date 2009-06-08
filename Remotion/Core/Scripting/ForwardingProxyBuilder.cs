// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Scripting
{
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
      ArgumentUtility.CheckNotNull ("interfaces", interfaces);

      _proxiedType = proxiedType;

      _classEmitter = CreateClassEmitter (name, interfaces, moduleScope);

      _proxied = CreateProxiedField();

      CreateProxyCtor(proxiedType);
    }


    public Type BuildProxyType ()
    {
      return _classEmitter.BuildType ();
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