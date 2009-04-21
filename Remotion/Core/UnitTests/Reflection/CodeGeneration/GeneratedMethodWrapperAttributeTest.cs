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
    public void ResolveWrappedMethod_GenType_RefType_Object ()
    {
      var module = new ModuleManager ().Scope.ObtainDynamicModuleWithStrongName ();
      MethodInfo wrappedMethod = typeof (List<object>).GetMethod ("Add");
      int token = module.GetMethodToken (wrappedMethod).Token;
      var attribute = new GeneratedMethodWrapperAttribute (token, new[] { typeof (object) });
      var resolvedMethod = attribute.ResolveWrappedMethod (module);

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
