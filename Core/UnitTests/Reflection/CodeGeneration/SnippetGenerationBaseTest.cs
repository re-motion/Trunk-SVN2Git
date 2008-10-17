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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class SnippetGenerationBaseTest : CodeGenerationBaseTest
  {
    private static int s_typeCount;

    private CustomClassEmitter _classEmitter;
    private CustomMethodEmitter _methodEmitter;
    private Type _builtType;
    private object _builtInstance;
    private Type _unsavedBuiltType;
    private CustomClassEmitter _unsavedClassEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = null;
      _unsavedClassEmitter = null;

      _methodEmitter = null;
      _builtType = null;
      _builtInstance = null;
      _unsavedBuiltType = null;
    }

    public CustomClassEmitter ClassEmitter
    {
      get
      {
        if (_classEmitter == null)
          _classEmitter = new CustomClassEmitter (Scope, this.GetType ().Name + s_typeCount++, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
        return _classEmitter;
      }
    }

    public CustomClassEmitter UnsavedClassEmitter
    {
      get { 
        if (_unsavedClassEmitter == null)
          _unsavedClassEmitter = new CustomClassEmitter (UnsavedScope, GetType ().Name + "Unsaved" + s_typeCount++, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
        return _unsavedClassEmitter; }
    }

    public CustomMethodEmitter GetMethodEmitter (bool isStatic)
    {
      if (_methodEmitter == null)
      {
        MethodAttributes flags = MethodAttributes.Public;
        if (isStatic)
          flags |= MethodAttributes.Static;
        _methodEmitter = ClassEmitter.CreateMethod ("TestMethod", flags);
      }
      return _methodEmitter;
    }

    public CustomMethodEmitter GetUnsavedMethodEmitter (bool isStatic)
    {
      MethodAttributes flags = MethodAttributes.Public;
      if (isStatic)
        flags |= MethodAttributes.Static;
      var methodEmitter = UnsavedClassEmitter.CreateMethod ("TestMethod", flags);
      return methodEmitter;
    }

    public Type GetBuiltType ()
    {
      if (_builtType == null)
        _builtType = ClassEmitter.BuildType ();
      return _builtType;
    }

    public Type GetUnsavedBuiltType ()
    {
      if (_unsavedBuiltType == null)
        _unsavedBuiltType = UnsavedClassEmitter.BuildType ();
      return _unsavedBuiltType;
    }

    public object GetBuiltInstance ()
    {
      if (_builtInstance == null)
        _builtInstance = Activator.CreateInstance (GetBuiltType ());
      return _builtInstance;
    }

    public object InvokeMethod (params object[] args)
    {
      if (_methodEmitter == null)
        throw new InvalidOperationException ("No method created.");
      else
      {
        if (_methodEmitter.MethodBuilder.IsStatic)
          return PrivateInvoke.InvokePublicStaticMethod (GetBuiltType (), _methodEmitter.Name, args);
        else
          return PrivateInvoke.InvokePublicMethod (GetBuiltInstance (), _methodEmitter.Name, args);
      }
    }
  }
}
