// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeTest
  {
    private UnderlyingTypeDescriptor _descriptor;
    private IMemberSelector _memberSelectorMock;
    private IRelatedMethodFinder _relatedMethodFinderMock;

    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _descriptor = UnderlyingTypeDescriptorObjectMother.Create (originalType: typeof (DomainType));
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      
      // Use a dynamic mock because constructor passes on _relatedMethodFinderMock to UnderlyingTypeDescriptor, which calls methods on the mock.
      // If this changes and the UnderlyingTypeDescriptor logic becomes a problem, consider injecting an ExistingMutableMemberInfoFactory instead and 
      // stubbing that.
      _relatedMethodFinderMock = MockRepository.GenerateMock<IRelatedMethodFinder>();

      _mutableType = new MutableType (_descriptor, _memberSelectorMock, _relatedMethodFinderMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mutableType.UnderlyingSystemType, Is.EqualTo (_descriptor.UnderlyingSystemInfo));
      Assert.That (_mutableType.DeclaringType, Is.EqualTo (_descriptor.DeclaringType));
      Assert.That (_mutableType.BaseType, Is.EqualTo (_descriptor.BaseType));
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
      Assert.That (_mutableType.Name, Is.EqualTo (_descriptor.Name));
      Assert.That (_mutableType.Namespace, Is.EqualTo (_descriptor.Namespace));
      Assert.That (_mutableType.FullName, Is.EqualTo (_descriptor.FullName));

      Assert.That (_mutableType.AddedInterfaces, Is.Empty);
      Assert.That (_mutableType.AddedFields, Is.Empty);
      Assert.That (_mutableType.AddedConstructors, Is.Empty);
      Assert.That (_mutableType.AddedMethods, Is.Empty);
    }

    [Test]
    public void Initialization_Interfaces ()
    {
      Assert.That (_descriptor.Interfaces, Is.Not.Empty);

      Assert.That (_mutableType.ExistingInterfaces, Is.EqualTo (_descriptor.Interfaces));
      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (_descriptor.Interfaces));
    }

    [Test]
    public void Initialization_Fields ()
    {
      var fields = _descriptor.Fields;
      Assert.That (fields, Is.Not.Empty); // base field, declared field
      var expectedField = fields.Single (m => m.Name == "ProtectedField");

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var mutableField = _mutableType.ExistingMutableFields.Single();

      Assert.That (mutableField.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (mutableField.UnderlyingSystemFieldInfo, Is.EqualTo (expectedField));
    }

    [Test]
    public void Initialization_Constructors ()
    {
      var ctors = _descriptor.Constructors;
      Assert.That (ctors, Has.Count.EqualTo (1));
      var expectedCtor = ctors.Single();

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var mutableCtor = _mutableType.ExistingMutableConstructors.Single();

      Assert.That (mutableCtor.UnderlyingSystemConstructorInfo, Is.EqualTo (expectedCtor));
      Assert.That (mutableCtor.DeclaringType, Is.SameAs (_mutableType));
    }

    [Test]
    public void Initialization_Methods ()
    {
      var methods = _descriptor.Methods;
      Assert.That (methods, Is.Not.Empty); // ToString(), Equals(), ...
      var expectedMethod = methods.Single (m => m.Name == "VirtualMethod");

      Assert.That (_mutableType.ExistingMutableMethods.Count, Is.EqualTo (2));
      var mutableMethod = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");

      Assert.That (mutableMethod.UnderlyingSystemMethodInfo, Is.EqualTo (expectedMethod));
      Assert.That (mutableMethod.DeclaringType, Is.SameAs (_mutableType));

      // Test that the _relatedMethodFinderMock was passed to the underlying descriptor.
      _relatedMethodFinderMock.AssertWasCalled (mock => mock.GetBaseMethod (expectedMethod));
    }

    [Test]
    [Ignore("TODO 5099")]
    public void Initialization_AbstractType ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (AbstractTypeWithOneMethod));
      Assert.That (mutableType.IsAbstract, Is.True);

      var mutableMethod = mutableType.AllMutableMethods.Single (x => x.Name == "Method");

      mutableMethod.SetBody (ctx => Expression.Default (mutableMethod.ReturnType));
      Assert.That (mutableType.IsAbstract, Is.False);
    }

    [Test]
    public void IsFullyImplemented ()
    {
      Assert.That (CreateForExisting (typeof (ConcreteType)).IsFullyImplemented, Is.True);
      Assert.That (CreateForExisting (typeof (AbstractTypeWithoutMethods)).IsFullyImplemented, Is.True);
      Assert.That (CreateForExisting (typeof (AbstractTypeWithOneMethod)).IsFullyImplemented, Is.False);
      Assert.That (CreateForExisting (typeof (DerivedAbstractTypeLeavesAbstractBaseMethod)).IsFullyImplemented, Is.False);
      Assert.That (CreateForExisting (typeof (DerivedAbstractTypeOverridesAbstractBaseMethod)).IsFullyImplemented, Is.True);
    }

    [Test]
    public void AllMutableFields ()
    {
      Assert.That (GetAllFields (_mutableType).ExistingBaseMembers, Is.Not.Empty);
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var existingField = _mutableType.ExistingMutableFields.Single();
      var addedField = _mutableType.AddField (ReflectionObjectMother.GetSomeType(), "_addedField");

      var allFields = _mutableType.AllMutableFields.ToArray();

      Assert.That (allFields, Has.Length.EqualTo (2));
      Assert.That (allFields[0], Is.SameAs (existingField));
      Assert.That (allFields[1], Is.SameAs (addedField));
    }

    [Test]
    public void AllMutableConstructors ()
    {
      Assert.That (_descriptor.Constructors, Has.Count.EqualTo (1));
      var existingCtor = _descriptor.Constructors.Single();
      var addedCtor = AddConstructor (_mutableType, new ArgumentTestHelper (7).ParameterDeclarations); // Need different signature

      var allConstructors = _mutableType.AllMutableConstructors.ToArray();

      Assert.That (allConstructors, Has.Length.EqualTo (2));
      Assert.That (allConstructors[0].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allConstructors[0].UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (allConstructors[1], Is.SameAs (addedCtor));
    }

    [Test]
    public void AllMutableMethods ()
    {
      Assert.That (_descriptor.Methods, Is.Not.Empty);
      var existingMethods = _descriptor.Methods;
      var addedMethod = AddMethod (_mutableType, "NewMethod");

      var allMethods = _mutableType.AllMutableMethods.ToArray();

      Assert.That (allMethods, Has.Length.EqualTo (3));
      Assert.That (allMethods[0].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allMethods[0].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[0]));
      Assert.That (allMethods[1].DeclaringType, Is.SameAs (_mutableType));
      Assert.That (allMethods[1].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[1]));
      Assert.That (allMethods[2], Is.SameAs (addedMethod));
    }

    [Test]
    public new void ToString ()
    {
      // Note: ToString() is implemented in CustomType base class.
      Assert.That (_mutableType.ToString(), Is.EqualTo ("DomainType"));
    }

    [Test]
    public void ToDebugString ()
    {
      // Note: ToDebugString() is implemented in CustomType base class.
      Assert.That (_mutableType.ToDebugString(), Is.EqualTo ("MutableType = \"DomainType\""));
    }

    [Test]
    public void IsAssignableTo ()
    {
      Assert.That (_mutableType.IsAssignableTo (_mutableType), Is.True);

      var underlyingSystemType = _mutableType.UnderlyingSystemType;
      Assert.That (underlyingSystemType, Is.Not.SameAs (_mutableType));
      Assert.That (_mutableType.IsAssignableTo (underlyingSystemType), Is.True);

      Assert.That (_mutableType.BaseType, Is.SameAs (typeof (DomainTypeBase)));
      Assert.That (_mutableType.IsAssignableTo (typeof (DomainTypeBase)), Is.True);

      Assert.IsNotNull (_mutableType.BaseType); // For ReSharper...
      Assert.That (_mutableType.BaseType.BaseType, Is.SameAs (typeof (C)));
      Assert.That (_mutableType.IsAssignableTo (typeof (C)), Is.True);
      Assert.That (_mutableType.IsAssignableTo (typeof (object)), Is.True);

      Assert.That (underlyingSystemType.GetInterfaces(), Has.Member (typeof (IDomainInterface)));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDomainInterface)), Is.True);

      Assert.That (_mutableType.GetInterfaces(), Has.No.Member (typeof (IDisposable)));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDisposable)), Is.False);
      _mutableType.AddInterface (typeof (IDisposable));
      Assert.That (_mutableType.IsAssignableTo (typeof (IDisposable)), Is.True);

      Assert.That (_mutableType.IsAssignableTo (typeof (UnrelatedType)), Is.False);
    }

    [Test]
    public void AddInterface ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();

      _mutableType.AddInterface (addedInterface);

      Assert.That (_mutableType.AddedInterfaces, Is.EqualTo (new[] { addedInterface }));
      Assert.That (_mutableType.ExistingInterfaces, Is.EqualTo (new[] { existingInterface }));
      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be an interface.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfNotAnInterface ()
    {
      _mutableType.AddInterface (typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Interface 'IDomainInterface' is already implemented.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfAlreadyImplemented ()
    {
      var existingInterface = _descriptor.Interfaces.First();
      _mutableType.AddInterface (existingInterface);
    }

    [Test]
    public void AddField ()
    {
      var newField = _mutableType.AddField (typeof (string), "_newField", FieldAttributes.Private);

      // Correct field info instance
      Assert.That (newField.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (newField.Name, Is.EqualTo ("_newField"));
      Assert.That (newField.FieldType, Is.EqualTo (typeof (string)));
      Assert.That (newField.Attributes, Is.EqualTo (FieldAttributes.Private));
      // Field info is stored
      Assert.That (_mutableType.AddedFields, Is.EqualTo (new[] { newField }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Field with equal name and signature already exists.\r\nParameter name: name")]
    public void AddField_ThrowsIfAlreadyExist ()
    {
      var field = _mutableType.ExistingMutableFields.Single();
      _mutableType.AddField (field.FieldType, field.Name, FieldAttributes.Private);
    }

    [Test]
    public void AddField_ReliesOnFieldSignature ()
    {
      var field = _mutableType.ExistingMutableFields.Single();
      Assert.That (field.FieldType, Is.Not.SameAs (typeof (string)));

      _mutableType.AddField (typeof (string), field.Name, FieldAttributes.Private);

      Assert.That (_mutableType.AddedFields, Has.Count.EqualTo (1));
    }

    [Test]
    public void GetMutableField ()
    {
      var existingField = _descriptor.Fields.Single (m => m.Name == "ProtectedField");
      Assert.That (existingField, Is.Not.AssignableTo<MutableFieldInfo>());

      var result = _mutableType.GetMutableField (existingField);

      Assert.That (result.UnderlyingSystemFieldInfo, Is.SameAs (existingField));
      Assert.That (_mutableType.ExistingMutableFields, Has.Member (result));
    }

    [Test]
    public void AddConstructor ()
    {
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = new[]
                                  {
                                      ParameterDeclarationObjectMother.Create (typeof(string), "param1"),
                                      ParameterDeclarationObjectMother.Create (typeof(int), "param2")
                                  };
      ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.Parameters.Select (p => p.Type), Is.EqualTo (new[] { typeof (string), typeof (int) }));
        Assert.That (context.Parameters.Select (p => p.Name), Is.EqualTo (new[] { "param1", "param2" }));
        Assert.That (context.This.Type, Is.SameAs (_mutableType));

        return fakeBody;
      };

      var ctorInfo = _mutableType.AddConstructor (attributes, parameterDeclarations.AsOneTime(), bodyProvider);

      // Correct constructor info instance
      Assert.That (ctorInfo.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (ctorInfo.Attributes, Is.EqualTo (attributes));
      var expectedParameterInfos =
          new[]
          {
              new { ParameterType = parameterDeclarations[0].Type },
              new { ParameterType = parameterDeclarations[1].Type }
          };
      var actualParameterInfos = ctorInfo.GetParameters().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Block (typeof (void), fakeBody);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, ctorInfo.Body);

      // Constructor info is stored
      Assert.That (_mutableType.AddedConstructors, Is.EqualTo (new[] { ctorInfo }));
    }

    [Test]
    public void AddConstructor_ThrowsForInvalidMethodAttributes ()
    {
      const string message = "The following MethodAttributes are not supported for constructors: " +
                             "Abstract, HideBySig, PinvokeImpl, RequireSecObject, UnmanagedExport, Virtual.\r\nParameter name: attributes";
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.Abstract), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.HideBySig), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.PinvokeImpl), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.RequireSecObject), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.UnmanagedExport), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddConstructor (_mutableType, MethodAttributes.Virtual), Throws.ArgumentException.With.Message.EqualTo (message));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Adding static constructors is not (yet) supported.\r\nParameter name: attributes")]
    public void AddConstructor_ThrowsForStatic ()
    {
      _mutableType.AddConstructor (MethodAttributes.Static, ParameterDeclaration.EmptyParameters, context => { throw new NotImplementedException(); });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Constructor with equal signature already exists.\r\nParameter name: parameterDeclarations")]
    public void AddConstructor_ThrowsIfAlreadyExists ()
    {
      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      Assert.That (_mutableType.ExistingMutableConstructors.Single().GetParameters(), Is.Empty);

      _mutableType.AddConstructor (0, ParameterDeclaration.EmptyParameters, context => Expression.Empty());
    }

    [Test]
    public void GetMutableConstructor ()
    {
      var existingCtor = _descriptor.Constructors.Single();
      Assert.That (existingCtor, Is.Not.AssignableTo<MutableConstructorInfo>());

      var result = _mutableType.GetMutableConstructor (existingCtor);

      Assert.That (result.UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (_mutableType.ExistingMutableConstructors, Has.Member (result));
    }

    [Test]
    public void AddMethod ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Public;
      var returnType = typeof(object);
      var parameterDeclarations = new[]
                                  {
                                      ParameterDeclarationObjectMother.Create (typeof (double), "hans"),
                                      ParameterDeclarationObjectMother.Create (typeof (string), "franz")
                                  };
      ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (int));
      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.This.Type, Is.SameAs (_mutableType));
        Assert.That (context.Parameters.Select (p => p.Type), Is.EqualTo (new[] { typeof (double), typeof (string) }));
        Assert.That (context.Parameters.Select (p => p.Name), Is.EqualTo (new[] { "hans", "franz" }));
        Assert.That (context.IsStatic, Is.False);
        Assert.That (context.HasBaseMethod, Is.False);

        return fakeBody;
      };

      var method = _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations.AsOneTime(), bodyProvider);

      // Correct inputMethod info instance
      Assert.That (method.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (method.UnderlyingSystemMethodInfo, Is.SameAs (method));
      Assert.That (method.Name, Is.EqualTo (name));
      Assert.That (method.ReturnType, Is.EqualTo (returnType));
      Assert.That (method.Attributes, Is.EqualTo (attributes));
      Assert.That (method.BaseMethod, Is.Null);
      Assert.That (method.IsGenericMethod, Is.False);
      Assert.That (method.IsGenericMethodDefinition, Is.False);
      Assert.That (method.ContainsGenericParameters, Is.False);
      var expectedParameterInfos =
          new[]
          {
              new { ParameterType = parameterDeclarations[0].Type },
              new { ParameterType = parameterDeclarations[1].Type }
          };
      var actualParameterInfos = method.GetParameters().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Convert (fakeBody, returnType);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);

      // Method info is stored
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { method }));
    }

    [Test]
    public void AddMethod_Static ()
    {
      var name = "StaticMethod";
      var attributes = MethodAttributes.Static;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (returnType);

      var method = _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations, context =>
      {
        Assert.That (context.IsStatic, Is.True);
        return fakeBody;
      });

      Assert.That (method.IsStatic, Is.True);
    }

    [Test]
    public void AddMethod_ThrowsForInvalidMethodAttributes ()
    {
      const string message = "The following MethodAttributes are not supported for methods: " +
                             "PinvokeImpl, RequireSecObject, UnmanagedExport.\r\nParameter name: attributes";
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.PinvokeImpl), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.RequireSecObject), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.UnmanagedExport), Throws.ArgumentException.With.Message.EqualTo (message));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "NewSlot methods must also be virtual.\r\nParameter name: attributes")]
    public void AddMethod_ThrowsIfNonVirtualAndNewSlot ()
    {
      _mutableType.AddMethod ("NotImportant", MethodAttributes.NewSlot, typeof (void), ParameterDeclaration.EmptyParameters, cx => Expression.Empty());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method 'VirtualMethod' with equal signature already exists.\r\nParameter name: name")]
    public void AddMethod_ThrowsIfAlreadyExists ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.VirtualMethod());
      Assert.That (method.GetParameters(), Is.Empty);

      _mutableType.AddMethod (method.Name, 0, method.ReturnType, ParameterDeclaration.EmptyParameters, cx => Expression.Empty());
    }

    [Test]
    public void AddMethod_Shadowing_NonVirtual ()
    {
      var baseMethod = GetBaseMethod (_mutableType, "ToString");
      Assert.That (baseMethod, Is.Not.Null);
      Assert.That (baseMethod.DeclaringType, Is.SameAs (typeof (object)));

      var nonVirtualAttributes = (MethodAttributes) 0;
      var newMethod = _mutableType.AddMethod ("ToString", nonVirtualAttributes, typeof (string), ParameterDeclaration.EmptyParameters, context =>
      {
        Assert.That (context.HasBaseMethod, Is.False);
        return Expression.Constant ("string");
      });

      Assert.That (newMethod, Is.Not.Null.And.Not.EqualTo (baseMethod));
      Assert.That (newMethod.BaseMethod, Is.Null);
      Assert.That (newMethod.GetBaseDefinition (), Is.SameAs (newMethod));
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { newMethod }));
    }

    [Test]
    public void AddMethod_Shadowing_VirtualAndNewSlot ()
    {
      var baseMethod = GetBaseMethod (_mutableType, "ToString");
      Assert.That (baseMethod, Is.Not.Null);
      Assert.That (baseMethod.DeclaringType, Is.SameAs (typeof (object)));

      var nonVirtualAttributes = MethodAttributes.Virtual | MethodAttributes.NewSlot;
      var newMethod = _mutableType.AddMethod ("ToString", nonVirtualAttributes, typeof (string), ParameterDeclaration.EmptyParameters, context =>
      {
        Assert.That (context.HasBaseMethod, Is.False);
        return Expression.Constant ("string");
      });

      Assert.That (newMethod, Is.Not.Null.And.Not.EqualTo (baseMethod));
      Assert.That (newMethod.BaseMethod, Is.Null);
      Assert.That (newMethod.GetBaseDefinition (), Is.SameAs (newMethod));
      Assert.That (_mutableType.AddedMethods, Is.EqualTo (new[] { newMethod }));
    }

    [Test]
    public void AddMethod_ImplicitOverride ()
    {
      var fakeOverridenMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.OverrideHierarchy (7));
      _relatedMethodFinderMock
          .Expect (mock => mock.GetMostDerivedVirtualMethod ("Method", new MethodSignature (typeof(int), Type.EmptyTypes, 0), _mutableType.BaseType))
          .Return (fakeOverridenMethod);

      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.HasBaseMethod, Is.True);
        Assert.That (context.BaseMethod, Is.SameAs (fakeOverridenMethod));

        return Expression.Default (typeof (int));
      };
      var addedMethod = _mutableType.AddMethod (
          "Method",
          MethodAttributes.Public | MethodAttributes.Virtual,
          typeof (int),
          ParameterDeclaration.EmptyParameters,
          bodyProvider);

      _relatedMethodFinderMock.VerifyAllExpectations ();
      Assert.That (addedMethod.BaseMethod, Is.EqualTo (fakeOverridenMethod));
      Assert.That (addedMethod.GetBaseDefinition (), Is.EqualTo (fakeOverridenMethod.GetBaseDefinition()));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot override final method 'B.FinalBaseMethodInB'.")]
    public void AddMethod_ThrowsIfOverridingFinalMethod ()
    {
      var signature = new MethodSignature (typeof (void), Type.EmptyTypes, 0);
      var fakeBaseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.FinalBaseMethodInB (7));
      _relatedMethodFinderMock
          .Expect (mock => mock.GetMostDerivedVirtualMethod ("MethodName", signature, _mutableType.BaseType))
          .Return (fakeBaseMethod);

      _mutableType.AddMethod (
          "MethodName",
          MethodAttributes.Public | MethodAttributes.Virtual,
          typeof (void),
          ParameterDeclaration.EmptyParameters,
          ctx => Expression.Empty());
    }

    [Test]
    [Ignore("TODO 5099")]
    public void AddMethod_AbstractMethod_UpdatesIsAbstract ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType ();
      Assert.That (mutableType.IsAbstract, Is.False);

      var mutableMethod = mutableType.AddMethod (
          "AbstractMethod", MethodAttributes.Abstract, typeof (void), ParameterDeclaration.EmptyParameters, null);
      Assert.That (mutableType.IsAbstract, Is.True);

      mutableMethod.SetBody (ctx => Expression.Default (mutableMethod.ReturnType));
      Assert.That (mutableType.IsAbstract, Is.False);
    }

    [Test]
    public void AddAbstractMethod ()
    {
      var name = "AbstractMethod";
      var attributes = MethodAttributes.Public;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);

      var method = _mutableType.AddAbstractMethod (name, attributes, returnType, parameterDeclarations.AsOneTime ());

      Assert.That (method.IsAbstract, Is.True);
    }

    [Test]
    public void GetOrAddMutableMethod_ExistingMethod_UsesMemberCollection ()
    {
      var existingMethod = _descriptor.Methods.Single (m => m.Name == "VirtualMethod");
      Assert.That (existingMethod, Is.Not.AssignableTo<MutableMethodInfo>());

      var result = _mutableType.GetOrAddMutableMethod (existingMethod);

      Assert.That (result.UnderlyingSystemMethodInfo, Is.SameAs (existingMethod));
      Assert.That (_mutableType.ExistingMutableMethods, Has.Member (result));
    }

    [Test]
    public void GetOrAddMutableMethod_ExistingOverride ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString ());
      var inputMethdod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.ToString());
      Assert.That (inputMethdod, Is.Not.EqualTo (baseDefinition));

      var fakeExistingOverride = MutableMethodInfoObjectMother.Create();
      _relatedMethodFinderMock
          .Expect (mock => mock.GetOverride (Arg.Is (baseDefinition), Arg<IEnumerable<MutableMethodInfo>>.List.Equal (_mutableType.AllMutableMethods)))
          .Return (fakeExistingOverride);

      var result = _mutableType.GetOrAddMutableMethod (inputMethdod);

      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeExistingOverride));
    }

    [Test]
    public void GetOrAddMutableMethod_BaseMethod_ImplicitOverride ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((A obj) => obj.OverrideHierarchy (7));
      var inputMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.OverrideHierarchy (7));
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((C obj) => obj.OverrideHierarchy (7));

      CallAndCheckGetOrAddMutableMethod (
          baseDefinition,
          inputMethod,
          baseMethod,
          false,
          baseMethod,
          new MethodInfo[0],
          "OverrideHierarchy",
          MethodAttributes.Public,
          MethodAttributes.ReuseSlot);
    }

    [Test]
    public void GetOrAddMutableMethod_BaseMethod_ImplicitOverride_AdjustsAttributes ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.ProtectedOrInternalVirtualNewSlotMethodInB (7));
      var inputMethod = baseDefinition;
      var baseMethod = baseDefinition;
      Assert.That (baseMethod.IsFamilyOrAssembly, Is.True);
      Assert.That (baseMethod.Attributes.IsSet (MethodAttributes.NewSlot), Is.True);

      CallAndCheckGetOrAddMutableMethod (
          baseDefinition,
          inputMethod,
          baseMethod,
          false,
          baseMethod,
          new MethodInfo[0],
          "ProtectedOrInternalVirtualNewSlotMethodInB",
          MethodAttributes.Family,
          MethodAttributes.ReuseSlot);
    }

    [Test]
    public void GetOrAddMutableMethod_ShadowedBaseMethod_ExplicitOverride ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((A obj) => obj.OverrideHierarchy (7));
      var inputMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.OverrideHierarchy (7));
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((C obj) => obj.OverrideHierarchy (7));
      Assert.That (baseMethod.Attributes.IsSet (MethodAttributes.NewSlot), Is.False);

      CallAndCheckGetOrAddMutableMethod (
          baseDefinition,
          inputMethod,
          baseMethod,
          true,
          null,
          new[] { baseDefinition },
          "Remotion.TypePipe.UnitTests.MutableReflection.MutableTypeTest+C_OverrideHierarchy",
          MethodAttributes.Private,
          MethodAttributes.NewSlot);
    }

    [Test]
    public void GetOrAddMutableMethod_BaseMethod_ImplicitOverride_Abstract ()
    {
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((AbstractTypeWithOneMethod obj) => obj.Method ());
      Assert.That (baseMethod.Attributes.IsSet (MethodAttributes.Abstract), Is.True);
      var mutableType = MutableTypeObjectMother.CreateForExistingType (
          typeof (DerivedAbstractTypeLeavesAbstractBaseMethod), relatedMethodFinder: _relatedMethodFinderMock);
      SetupExpectationsForGetOrAddMutableMethod (baseMethod, baseMethod, false, baseMethod, mutableType, typeof (AbstractTypeWithOneMethod));

      var result = mutableType.GetOrAddMutableMethod (baseMethod);

      _relatedMethodFinderMock.VerifyAllExpectations();

      Assert.That (result.BaseMethod, Is.SameAs (baseMethod));
      Assert.That (result.IsAbstract, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method is declared by a type outside of this type's class hierarchy: 'IDomainInterface'.\r\nParameter name: method")]
    public void GetOrAddMutableMethod_InterfaceDeclaringType ()
    {
      Assert.That (_descriptor.Interfaces, Has.Member (typeof (IDomainInterface)));
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.InterfaceMethod());
      _mutableType.GetOrAddMutableMethod (method);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method is declared by a type outside of this type's class hierarchy: 'String'.\r\nParameter name: method")]
    public void GetOrAddMutableMethod_UnrelatedDeclaringType ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((string obj) => obj.Trim ());
      _mutableType.GetOrAddMutableMethod (method);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "A method declared in a base type must be virtual in order to be modified.")]
    public void GetOrAddMutableMethod_NonVirtualMethod ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.ExistingBaseMethod());
      _mutableType.GetOrAddMutableMethod (method);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot override final method 'B.FinalBaseMethodInB'.")]
    public void GetOrAddMutableMethod_FinalBaseMethod ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((A obj) => obj.FinalBaseMethodInB (7));
      var inputMethod = baseDefinition;
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.FinalBaseMethodInB (7));
      var isBaseDefinitionShadowed = BooleanObjectMother.GetRandomBoolean();

      SetupExpectationsForGetOrAddMutableMethod (baseDefinition, baseMethod, isBaseDefinitionShadowed, null);

      _mutableType.GetOrAddMutableMethod (inputMethod);
    }

    [Test]
    public void Accept_UnmodifiedMutableMemberHandler_WithUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var unmodfiedField = _mutableType.ExistingMutableFields.Single ();

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single ();

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var unmodfiedMethod1 = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");
      var unmodfiedMethod2 = _mutableType.ExistingMutableMethods.Single (m => m.Name == "InterfaceMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeUnmodifiedMutableMemberHandler> ();

      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodfiedField));
      handlerMock.Expect (mock => mock.HandleUnmodifiedConstructor (unmodfiedConstructor));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod1));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodfiedMethod2));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_UnmodifiedMutableMemberHandler_WithAddedAndModifiedExistingMembers ()
    {
      // Currently, fields cannot be modified.
      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      var unmodifiedField = _mutableType.ExistingMutableFields.Single();
      _mutableType.AddField (ReflectionObjectMother.GetSomeType (), "name", FieldAttributes.Private);

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      MutableConstructorInfoTestHelper.ModifyConstructor (_mutableType.ExistingMutableConstructors.Single ());
      AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create ());

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      MutableMethodInfoTestHelper.ModifyMethod (_mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod"));
      AddMethod (_mutableType, "AddedMethod");
      // Currently, non-virual methods cannot be modified.
      var unmodifiedMethod = _mutableType.ExistingMutableMethods.Single (m => m.Name == "InterfaceMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeUnmodifiedMutableMemberHandler> ();

      // Currently, fields and non-virtual methods cannot be modified.
      handlerMock.Expect (mock => mock.HandleUnmodifiedField (unmodifiedField));
      handlerMock.Expect (mock => mock.HandleUnmodifiedMethod (unmodifiedMethod));

      _mutableType.Accept (handlerMock);

      // Fields cannot currently be mutated
      //handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedField (Arg<MutableFieldInfo>.Is.Anything));
      handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedConstructor (Arg<MutableConstructorInfo>.Is.Anything));
      handlerMock.AssertWasNotCalled (mock => mock.HandleUnmodifiedMethod (Arg<MutableMethodInfo>.Is.NotEqual(unmodifiedMethod)));
    }

    [Test]
    public void Accept_ModificationHandler_WithAddedAndUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.GetInterfaces(), Has.Length.EqualTo (1));
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType();
      _mutableType.AddInterface (addedInterface);
      Assert.That (_mutableType.GetInterfaces(), Has.Length.EqualTo (2));

      Assert.That (_mutableType.ExistingMutableFields, Has.Count.EqualTo (1));
      // There is currently no method HandleModifiedField, so we don't need to assert that the unmodified field isn't handled.
      var addedFieldInfo = _mutableType.AddField (ReflectionObjectMother.GetSomeType(), "name", FieldAttributes.Private);

      Assert.That (_mutableType.ExistingMutableConstructors, Has.Count.EqualTo (1));
      var unmodfiedConstructor = _mutableType.ExistingMutableConstructors.Single();
      var addedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());

      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var unmodfiedMethod = _mutableType.ExistingMutableMethods.Single(m => m.Name == "VirtualMethod");
      var addedMethod = AddMethod (_mutableType, "AddedMethod");

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();

      handlerMock.Expect (mock => mock.HandleAddedInterface (addedInterface));

      handlerMock.Expect (mock => mock.HandleAddedField (addedFieldInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (addedConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (addedMethod));

      _mutableType.Accept (handlerMock);

      handlerMock.AssertWasNotCalled (mock => mock.HandleModifiedConstructor (unmodfiedConstructor));
      handlerMock.AssertWasNotCalled (mock => mock.HandleModifiedMethod (unmodfiedMethod));

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_ModificationHandler_WithModifiedConstructors ()
    {
      Assert.That (_mutableType.ExistingMutableConstructors, Is.Not.Empty);
      var modifiedExistingConstructorInfo = _mutableType.ExistingMutableConstructors.First();
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedExistingConstructorInfo);

      var modifiedAddedConstructorInfo = AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create());
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedAddedConstructorInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();
      handlerMock.Expect (mock => mock.HandleModifiedConstructor (modifiedExistingConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (modifiedAddedConstructorInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_ModificationHandler_WithModifiedMethod ()
    {
      Assert.That (_mutableType.ExistingMutableMethods, Has.Count.EqualTo (2));
      var modifiedExistingMethodInfo = _mutableType.ExistingMutableMethods.Single (m => m.Name == "VirtualMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedExistingMethodInfo);

      var modifiedAddedMethodInfo = AddMethod (_mutableType, "ModifiedAddedMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedAddedMethodInfo);

      var handlerMock = MockRepository.GenerateStrictMock<IMutableTypeModificationHandler>();
      handlerMock.Expect (mock => mock.HandleModifiedMethod (modifiedExistingMethodInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (modifiedAddedMethodInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void GetCustomAttributeData ()
    {
      var result = _mutableType.GetCustomAttributeData ();

      Assert.That (result.Select (a => a.Type), Is.EquivalentTo (new[] { typeof (AbcAttribute) }));
      Assert.That (result, Is.SameAs (_mutableType.GetCustomAttributeData ()), "should be cached");
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAttributeFlagsImpl");

      Assert.That (result, Is.EqualTo (_descriptor.Attributes));
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
    }

    [Test]
    public void GetAllInterfaces ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single ();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType ();
      _mutableType.AddInterface (addedInterface);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllInterfaces");

      Assert.That (result, Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    public void GetAllFields ()
    {
      _mutableType.AddField (typeof (int), "added");
      var allFields = GetAllFields (_mutableType);
      Assert.That (allFields.AddedMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allFields.ExistingBaseMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllFields");

      Assert.That (result, Is.SameAs (allFields));
    }

    [Test]
    public void GetAllConstructors ()
    {
      AddConstructor (_mutableType, ParameterDeclarationObjectMother.Create ());
      var allConstructors = GetAllConstructors (_mutableType);
      Assert.That (allConstructors.AddedMembers, Is.Not.Empty);
      Assert.That (allConstructors.ExistingDeclaredMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllConstructors");

      Assert.That (result, Is.SameAs (allConstructors));
    }

    [Test]
    public void GetAllMethods ()
    {
      AddMethod (_mutableType, "Added");
      var allMethods = GetAllMethods (_mutableType);
      Assert.That (allMethods.AddedMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingDeclaredMembers, Is.Not.Empty);
      Assert.That (allMethods.ExistingBaseMembers, Is.Not.Empty);

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllMethods");

      Assert.That (result, Is.EqualTo (allMethods));
    }

    [Test]
    public void GetMethods_FiltersOverriddenMethods ()
    {
      var baseMethod = typeof (object).GetMethod ("ToString");
      _relatedMethodFinderMock
          .Stub (stub => stub.GetMostDerivedVirtualMethod (Arg.Is ("ToString"), Arg<MethodSignature>.Is.Anything, Arg<Type>.Is.Anything))
          .Return (baseMethod);
      var addedOverride = _mutableType.AddMethod (
          baseMethod.Name,
          MethodAttributes.Virtual,
          baseMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (baseMethod),
          ctx => ExpressionTreeObjectMother.GetSomeExpression (baseMethod.ReturnType));
      Assert.That (addedOverride.BaseMethod, Is.SameAs (baseMethod));

      var result = PrivateInvoke.InvokeNonPublicMethod (_mutableType, "GetAllMethods");

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (result, Has.Member (addedOverride));
      Assert.That (result, Has.No.Member (typeof (DomainType).GetMethod ("ToString")));
    }

    [Test]
    public void GetMutableMember_InvalidDeclaringType ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField (() => Type.EmptyTypes);
      var ctor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new string(new char[0]));

      Assert.That (
          () => _mutableType.GetMutableField (field),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given field is declared by a different type: 'System.Type'.\r\nParameter name: field"));
      Assert.That (
          () => _mutableType.GetMutableConstructor (ctor),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given constructor is declared by a different type: 'System.String'.\r\nParameter name: constructor"));
    }

    [Test]
    public void GetMutableMember_NoMapping ()
    {
      var fieldStub = MockRepository.GenerateStub<FieldInfo> ();
      fieldStub.Stub (stub => stub.DeclaringType).Return (_mutableType);
      var ctorStub = MockRepository.GenerateStub<ConstructorInfo> ();
      ctorStub.Stub (stub => stub.DeclaringType).Return (_mutableType);

      Assert.That (
          () => _mutableType.GetMutableField (fieldStub),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("The given field cannot be modified."));
      Assert.That (
          () => _mutableType.GetMutableConstructor (ctorStub),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo ("The given constructor cannot be modified."));
    }

    private MutableType CreateForExisting (Type originalType)
    {
      return MutableTypeObjectMother.CreateForExistingType (originalType);
    }

    private void CallAndCheckGetOrAddMutableMethod (
        MethodInfo baseDefinition,
        MethodInfo inputMethod,
        MethodInfo baseMethod,
        bool isBaseDefinitionShadowed,
        MethodInfo expectedBaseMethod,
        IEnumerable<MethodInfo> expectedAddedExplicitBaseDefinitions,
        string expectedOverrideMethodName,
        MethodAttributes expectedOverrideVisibility,
        MethodAttributes expectedVtableLayout)
    {
      SetupExpectationsForGetOrAddMutableMethod (baseDefinition, baseMethod, isBaseDefinitionShadowed, expectedBaseMethod);

      var result = _mutableType.GetOrAddMutableMethod (inputMethod);

      _relatedMethodFinderMock.VerifyAllExpectations();

      Assert.That (result.AddedExplicitBaseDefinitions, Is.EqualTo (expectedAddedExplicitBaseDefinitions));
      Assert.That (result.BaseMethod, Is.SameAs (expectedBaseMethod));
      Assert.That (result.Name, Is.EqualTo (expectedOverrideMethodName));
      var expectedAttributes = expectedOverrideVisibility | expectedVtableLayout | MethodAttributes.Virtual | MethodAttributes.HideBySig;
      Assert.That (result.Attributes, Is.EqualTo (expectedAttributes));
      Assert.That (result.ReturnType, Is.SameAs (typeof (void)));
      var parameter = result.GetParameters().Single();
      Assert.That (parameter.ParameterType, Is.SameAs (typeof (int)));
      Assert.That (parameter.Name, Is.EqualTo ("parameterName"));

      Assert.That (result.Body, Is.InstanceOf<MethodCallExpression>());
      var methodCallExpression = (MethodCallExpression) result.Body;
      Assert.That (methodCallExpression.Method, Is.TypeOf<NonVirtualCallMethodInfoAdapter>());
      var baceCallMethodInfoAdapter = (NonVirtualCallMethodInfoAdapter) methodCallExpression.Method;
      Assert.That (baceCallMethodInfoAdapter.AdaptedMethodInfo, Is.SameAs (baseMethod));
    }

    private void SetupExpectationsForGetOrAddMutableMethod (
        MethodInfo baseDefinition,
        MethodInfo baseMethod,
        bool isBaseDefinitionShadowed,
        MethodInfo fakeBaseMethod,
        MutableType mutableType = null,
        Type typeToStartSearchForMostDerivedOverride = null)
    {
      mutableType = mutableType ?? _mutableType;
      typeToStartSearchForMostDerivedOverride = typeToStartSearchForMostDerivedOverride ?? typeof (DomainTypeBase);

      _relatedMethodFinderMock
          .Expect (mock => mock.GetOverride (Arg.Is (baseDefinition), Arg<IEnumerable<MutableMethodInfo>>.List.Equal (mutableType.AllMutableMethods)))
          .Return (null);
      _relatedMethodFinderMock
          .Expect (mock => mock.IsShadowed (baseDefinition, GetAllMethods (mutableType)))
          .Return (isBaseDefinitionShadowed);
      _relatedMethodFinderMock
          .Expect (mock => mock.GetMostDerivedOverride (baseDefinition, typeToStartSearchForMostDerivedOverride))
          .Return (baseMethod);
      // Needed for AddMethod (will only be called for implicit overrides)
      if (!isBaseDefinitionShadowed)
      {
        _relatedMethodFinderMock
            .Expect (mock => mock.GetMostDerivedVirtualMethod (baseMethod.Name, MethodSignature.Create (baseMethod), mutableType.BaseType))
            .Return (fakeBaseMethod);
      }
    }

    private MutableConstructorInfo AddConstructor (MutableType mutableType, params ParameterDeclaration[] parameterDeclarations)
    {
      return mutableType.AddConstructor (MethodAttributes.Public, parameterDeclarations.AsOneTime(), context => Expression.Empty());
    }

    private MutableConstructorInfo AddConstructor (MutableType mutableType, MethodAttributes attributes)
    {
      return mutableType.AddConstructor (attributes, ParameterDeclaration.EmptyParameters, context => Expression.Empty());
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, string name, params ParameterDeclaration[] parameterDeclarations)
    {
      var returnType = ReflectionObjectMother.GetSomeType();
      var body = ExpressionTreeObjectMother.GetSomeExpression (returnType);

      return mutableType.AddMethod (name, MethodAttributes.Public, returnType, parameterDeclarations.AsOneTime(), ctx => body);
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, MethodAttributes attributes)
    {
      return mutableType.AddMethod ("dummy", attributes, typeof (void), ParameterDeclaration.EmptyParameters, context => Expression.Empty());
    }

    private MutableTypeMemberCollection<FieldInfo, MutableFieldInfo> GetAllFields (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<FieldInfo, MutableFieldInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_fields");
    }

    private MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> GetAllMethods (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<MethodInfo, MutableMethodInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_methods");
    }

    private MutableTypeMemberCollection<ConstructorInfo, MutableConstructorInfo> GetAllConstructors (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<ConstructorInfo, MutableConstructorInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_constructors");
    }

    private MethodInfo GetBaseMethod (MutableType mutableType, string name)
    {
      return GetAllMethods (mutableType).ExistingBaseMembers.Single (mi => mi.Name == name);
    }

    public class A
    {
      // base definition
      public virtual  void OverrideHierarchy (int aaa) { }

      public virtual void FinalBaseMethodInB (int i) { }
    }

    public class B : A
    {
      // GetOrAddMutableMethod input
      public override void OverrideHierarchy (int bbb) { }

      protected internal virtual void ProtectedOrInternalVirtualNewSlotMethodInB (int parameterName) { }
      public override sealed void FinalBaseMethodInB (int i) { }
    }

    public class C : B
    {
      // base inputMethod
      public override void OverrideHierarchy (int parameterName) { }
    }

    public class DomainTypeBase : C
    {
      public int BaseField;

      public void ExistingBaseMethod () { }
    }

    [Abc]
    public class DomainType : DomainTypeBase, IDomainInterface
    {
      // ReSharper disable UnaccessedField.Global
      protected int ProtectedField;
      // ReSharper restore UnaccessedField.Global

      public DomainType ()
      {
        ProtectedField = Dev<int>.Null;
      }

      public virtual string VirtualMethod () { return ""; }

      public void InterfaceMethod () { }
    }

    public interface IDomainInterface
    {
      void InterfaceMethod ();
    }

// ReSharper disable ClassWithVirtualMembersNeverInherited.Local
    private class UnrelatedType
// ReSharper restore ClassWithVirtualMembersNeverInherited.Local
    {
      public virtual string VirtualMethod () { return ""; }
    }

    public class AbcAttribute : Attribute { }

    class ConcreteType { }
    abstract class AbstractTypeWithoutMethods { }
    abstract class AbstractTypeWithOneMethod
    {
      public abstract void Method ();
    }
    abstract class DerivedAbstractTypeLeavesAbstractBaseMethod : AbstractTypeWithOneMethod { }
    abstract class DerivedAbstractTypeOverridesAbstractBaseMethod : AbstractTypeWithOneMethod
    {
      public override void Method () { }
    }
  }
}