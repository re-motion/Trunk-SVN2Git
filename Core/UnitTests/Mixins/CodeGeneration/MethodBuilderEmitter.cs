/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using System.Reflection.Emit;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public class MethodBuilderEmitter : IMemberEmitter
  {
    private readonly MethodBuilder _methodBuilder;
    private readonly MethodCodeBuilder _codeBuilder;

    public MethodBuilderEmitter (MethodBuilder methodBuilder)
    {
      ArgumentUtility.CheckNotNull ("methodBuilder", methodBuilder);
      _methodBuilder = methodBuilder;
      _codeBuilder = new MethodCodeBuilder (methodBuilder.DeclaringType.BaseType, methodBuilder, methodBuilder.GetILGenerator());
    }

    public MethodBuilder MethodBuilder
    {
      get { return _methodBuilder; }
    }

    public MethodCodeBuilder CodeBuilder
    {
      get { return _codeBuilder; }
    }

    public void Generate ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_codeBuilder, "Generate", this, _codeBuilder.Generator);
    }

    public void EnsureValidCodeBlock ()
    {
      throw new NotImplementedException();
    }

    public MemberInfo Member
    {
      get { return _methodBuilder; }
    }

    public Type ReturnType
    {
      get { return _methodBuilder.ReturnType; }
    }
  }
}