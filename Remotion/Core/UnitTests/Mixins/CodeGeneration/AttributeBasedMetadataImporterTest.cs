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
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Rhino.Mocks;
using System.Linq;
using Remotion.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class AttributeBasedMetadataImporterTest
  {
    [Test]
    public void GetMetadataForMixedType_Wrapper()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedResult = new ClassContext (typeof (object));
      importerMock.Expect (mock => mock.GetMetadataForMixedType ((_Type) typeof (object))).Return (expectedResult);

      importerMock.Replay ();
      var result = importerMock.GetMetadataForMixedType (typeof (object));

      Assert.That (result, Is.SameAs (expectedResult));
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_Identifier ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (expectedIdentifier);
      importerMock.Expect (mock => mock.GetMethodWrappersForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (new Tuple<MethodInfo, MethodInfo>[0]);

      importerMock.Replay ();
      
      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.Identifier, Is.SameAs (expectedIdentifier));
      
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedType ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (expectedIdentifier);
      importerMock.Expect (mock => mock.GetMethodWrappersForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (new Tuple<MethodInfo, MethodInfo>[0]);

      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.GeneratedType, Is.SameAs (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedOverrideInterface ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (expectedIdentifier);
      importerMock.Expect (mock => mock.GetMethodWrappersForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (new Tuple<MethodInfo, MethodInfo>[0]);

      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.GeneratedOverrideInterface, Is.SameAs (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods)));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (TypeImportException), ExpectedMessage = "The given type 'System.Object' has a concrete mixin type identifier, but no "
        + "IOverriddenMethods interface.")]
    public void GetMetadataForMixinType_NoOverrideInterface ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (object))).Return (expectedIdentifier);
      importerMock.Replay ();

      importerMock.GetMetadataForMixinType (typeof (object));
    }

    [Test]
    public void GetMetadataForMixinType_Wrappers ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (expectedIdentifier);

      var method1 = typeof (DateTime).GetMethod ("get_Now");
      var method2 = typeof (DateTime).GetMethod ("get_Day");
      var wrapper1 = typeof (DateTime).GetMethod ("get_Month");
      var wrapper2 = typeof (DateTime).GetMethod ("get_Year");

      importerMock
          .Expect (mock => mock.GetMethodWrappersForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Return (new[] { Tuple.NewTuple (method1, wrapper1), Tuple.NewTuple (method2, wrapper2) });

      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That (result.GetMethodWrapper (method1), Is.EqualTo (wrapper1));
      Assert.That (result.GetMethodWrapper (method2), Is.EqualTo (wrapper2));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_NoAttribute ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (null);
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That (result, Is.Null);

      importerMock.VerifyAllExpectations ();
    }


    [Test]
    public void GetMetadataForMixedType ()
    {
      var classContext1 = new ClassContext (typeof (object));

      var attribute1 = ConcreteMixedTypeAttribute.FromClassContext (classContext1, new Type[0]);

      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Return (new[] { attribute1 });
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetMetadataForMixedType (typeMock);
      Assert.That (result, Is.EqualTo (classContext1));

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixedType_NoAttribute ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Return (new ConcreteMixedTypeAttribute[0]);
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetMetadataForMixedType (typeMock);
      Assert.That (result, Is.Null);

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIdentifierForMixinType ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());
      var attribute = ConcreteMixinTypeAttribute.Create (identifier);

      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Return (new[] { attribute });
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetIdentifierForMixinType (typeMock);
      Assert.That (result, Is.EqualTo (identifier));

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIdentifierForMixinType_NoAttribute ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Return (new ConcreteMixinTypeAttribute[0]);
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetIdentifierForMixinType (typeMock);
      Assert.That (result, Is.Null);

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMethodWrappersForMixinType()
    {
      var moduleForWrappers = new ModuleManager ();
      Type builtType = CreateTypeWithFakeWrappers(moduleForWrappers);

      // fake wrapper methods
      var wrapperMethod1 = builtType.GetMethod ("Wrapper1");
      var wrapperMethod2 = builtType.GetMethod ("Wrapper2");
      var nonWrapperMethod1 = builtType.GetMethod ("NonWrapper1");
      var nonWrapperMethod2 = builtType.GetMethod ("NonWrapper2");
      
      // fake wrapped methods
      var wrappedMethod1 = typeof (DateTime).GetMethod ("get_Now");
      var wrappedMethod2 = typeof (DateTime).GetMethod ("get_Day");

      // fake attributes simulating the relationship between wrapper methods and wrapped methods
      var attribute1 = new GeneratedMethodWrapperAttribute (moduleForWrappers.SignedModule.GetMethodToken (wrappedMethod1).Token, new Type[0]);
      var attribute2 = new GeneratedMethodWrapperAttribute (moduleForWrappers.SignedModule.GetMethodToken (wrappedMethod2).Token, new Type[0]);

      // prepare importerMock.GetWrapperAttribute to return attribute1 and attribute2 for wrapperMethod1 and wrapperMethod2
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", nonWrapperMethod1)).Return (null);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", nonWrapperMethod2)).Return (null);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", wrapperMethod1)).Return (attribute1);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", wrapperMethod2)).Return (attribute2);
      importerMock.Replay ();
      
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetMethods (BindingFlags.Instance | BindingFlags.Public))
          .Return (new[] { nonWrapperMethod1, nonWrapperMethod2, wrapperMethod1, wrapperMethod2 });

      var result = importerMock.GetMethodWrappersForMixinType (typeMock).ToArray();
      Assert.That (result, Is.EqualTo (new[] { new Tuple<MethodInfo, MethodInfo> (wrappedMethod1, wrapperMethod1), new Tuple<MethodInfo, MethodInfo> (wrappedMethod2, wrapperMethod2) }));
    }

    [Test]
    public void GetWrapperAttribute_WithResult ()
    {
      var importer = new AttributeBasedMetadataImporter();
      var method = GetType ().GetMethod ("FakeWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute) PrivateInvoke.InvokeNonPublicMethod (importer, "GetWrapperAttribute", method);

      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute.WrappedMethodRefToken, Is.EqualTo (0xfeefee));
    }

    [Test]
    public void GetWrapperAttribute_WithoutResult ()
    {
      var importer = new AttributeBasedMetadataImporter ();
      var method = GetType ().GetMethod ("FakeNonWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute) PrivateInvoke.InvokeNonPublicMethod (importer, "GetWrapperAttribute", method);

      Assert.That (attribute, Is.Null);
    }

    [GeneratedMethodWrapper (0xfeefee, new Type[0])]
    public void FakeWrapperMethod ()
    {
    }

    public void FakeNonWrapperMethod ()
    {
    }

    private Type CreateTypeWithFakeWrappers (ModuleManager moduleForWrappers)
    {
      TypeBuilder wrapperClassBuilder = moduleForWrappers.Scope.ObtainDynamicModuleWithStrongName ().DefineType ("WrapperClass");

      wrapperClassBuilder.DefineMethod ("Wrapper1", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("Wrapper2", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("NonWrapper1", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("NonWrapper2", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);

      return wrapperClassBuilder.CreateType ();
    }
  }
}
