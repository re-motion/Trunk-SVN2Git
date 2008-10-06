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
using System.Collections.Generic;
using System.Reflection.Emit;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Reflection;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.Collections;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class GeneratedMethodWrapperAttributeTest
  {
    [Test]
    public void ResolveWrappedMethod()
    {
      MethodInfo wrappedMethod = typeof (DateTime).GetMethod ("get_Now");
      var attribute = new GeneratedMethodWrapperAttribute (wrappedMethod.MetadataToken, new Type[0]);
      Type typeHoldingWrapperMethod = typeof (DateTime);
      var resolvedMethod = attribute.ResolveWrappedMethod (typeHoldingWrapperMethod.Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveWrappedMethod_FromOtherModule ()
    {
      var moduleForWrappers = new ModuleManager ();
      TypeBuilder wrapperClassBuilder = moduleForWrappers.Scope.ObtainDynamicModuleWithStrongName ().DefineType ("WrapperClass");

      wrapperClassBuilder.DefineMethod ("Wrapper", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      Type wrapperType = wrapperClassBuilder.CreateType ();

      MethodInfo wrappedMethod = typeof (DateTime).GetMethod ("get_Now");
      var attribute = new GeneratedMethodWrapperAttribute (moduleForWrappers.SignedModule.GetMethodToken (wrappedMethod).Token, new Type[0]);
      var resolvedMethod = attribute.ResolveWrappedMethod (wrapperType.Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveWrappedMethod_GenType_RefType ()
    {
      MethodInfo wrappedMethod = typeof (List<string>).GetMethod ("Add");
      var attribute = new GeneratedMethodWrapperAttribute (wrappedMethod.MetadataToken, new[] { typeof (string) });
      var resolvedMethod = attribute.ResolveWrappedMethod (typeof (List<string>).Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveWrappedMethod_GenType_ValueType ()
    {
      MethodInfo wrappedMethod = typeof (List<int>).GetMethod ("Add");
      var attribute = new GeneratedMethodWrapperAttribute (wrappedMethod.MetadataToken, new[] { typeof (int) });
      var resolvedMethod = attribute.ResolveWrappedMethod (typeof (List<int>).Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveWrappedMethod_GenMethod ()
    {
      MethodInfo wrappedMethod = typeof (ClassWithConstrainedGenericMethod).GetMethod ("GenericMethod");
      var attribute = new GeneratedMethodWrapperAttribute (wrappedMethod.MetadataToken, new Type[0]);
      Type typeHoldingWrapperMethod = typeof (ClassWithConstrainedGenericMethod);
      var resolvedMethod = attribute.ResolveWrappedMethod (typeHoldingWrapperMethod.Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveWrappedMethod_GenMethod_GenType ()
    {
      var genericType = typeof (GenericClassWithGenericMethod<IConvertible, List<string>, DateTime, object, IConvertible,  List<List<IConvertible[]>>>);
      MethodInfo wrappedMethod = genericType.GetMethod ("GenericMethod");
      var attribute = new GeneratedMethodWrapperAttribute (wrappedMethod.MetadataToken, genericType.GetGenericArguments());
      var resolvedMethod = attribute.ResolveWrappedMethod (genericType.Module);

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }
  }
}