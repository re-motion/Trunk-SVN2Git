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
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  [TestFixture]
  public class MutableMemberFactoryTest
  {
    private MutableMemberFactory _mutableMemberFactory;

    private MutableType _mutableType;
    private IMemberSelector _memberSelectorMock;
    private IRelatedMethodFinder _relatedMethodFinderMock;

    private bool _isNewlyCreated;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = MutableTypeObjectMother.CreateForExisting (typeof (DomainType));
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      _relatedMethodFinderMock = MockRepository.GenerateMock<IRelatedMethodFinder>();

      _mutableMemberFactory = new MutableMemberFactory (_memberSelectorMock, _relatedMethodFinderMock);
    }

    [Test]
    public void CreateInitialization ()
    {
      var isStatic = BooleanObjectMother.GetRandomBoolean();
      var fakeExpression = ExpressionTreeObjectMother.GetSomeExpression();

      var result = _mutableMemberFactory.CreateInitialization (
          _mutableType,
          isStatic,
          ctx =>
          {
            Assert.That (ctx.DeclaringType, Is.SameAs (_mutableType));
            Assert.That (ctx.IsStatic, Is.EqualTo (isStatic));

            return fakeExpression;
          });

      Assert.That (result, Is.SameAs (fakeExpression));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Body provider must return non-null body.")]
    public void CreateInitialization_NullBody ()
    {
      _mutableMemberFactory.CreateInitialization (_mutableType, false, ctx => null);
    }

    [Test]
    public void CreateMutableField ()
    {
      var field = _mutableMemberFactory.CreateMutableField (_mutableType, "_newField", typeof (string), FieldAttributes.FamANDAssem);

      Assert.That (field.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (field.Name, Is.EqualTo ("_newField"));
      Assert.That (field.FieldType, Is.EqualTo (typeof (string)));
      Assert.That (field.Attributes, Is.EqualTo (FieldAttributes.FamANDAssem));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Field cannot be of type void.\r\nParameter name: type")]
    public void CreateMutableField_VoidType ()
    {
      _mutableMemberFactory.CreateMutableField (_mutableType, "NotImportant", typeof (void), FieldAttributes.ReservedMask);
    }

    [Test]
    public void CreateMutableField_ThrowsIfAlreadyExist ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.IntField);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableField (_mutableType, "OtherName", field.FieldType, 0),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableField (_mutableType, field.Name, typeof (string), 0),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableField (_mutableType, field.Name, field.FieldType, 0),
          Throws.ArgumentException.With.Message.EqualTo ("Field with equal signature already exists.\r\nParameter name: name"));
    }

    [Test]
    public void CreateMutableConstructor ()
    {
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = new[]
                                  {
                                      ParameterDeclarationObjectMother.Create (typeof (string), "param1"),
                                      ParameterDeclarationObjectMother.Create (typeof (int), "param2")
                                  };
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.Parameters.Select (p => p.Type), Is.EqualTo (new[] { typeof (string), typeof (int) }));
        Assert.That (ctx.Parameters.Select (p => p.Name), Is.EqualTo (new[] { "param1", "param2" }));
        Assert.That (ctx.This.Type, Is.SameAs (_mutableType));

        return fakeBody;
      };

      var constructor = _mutableMemberFactory.CreateMutableConstructor (_mutableType, attributes, parameterDeclarations.AsOneTime(), bodyProvider);

      Assert.That (constructor.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (constructor.Name, Is.EqualTo (".ctor"));
      Assert.That (constructor.Attributes, Is.EqualTo (attributes));
      var expectedParameterInfos =
          new[]
          {
              new { ParameterType = parameterDeclarations[0].Type },
              new { ParameterType = parameterDeclarations[1].Type }
          };
      var actualParameterInfos = constructor.GetParameters ().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Block (typeof (void), fakeBody);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, constructor.Body);
    }

    [Test]
    public void CreateMutableConstructor_ThrowsForInvalidMethodAttributes ()
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
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Type initializers (static constructors) cannot be added via this API, use MutableType.AddTypeInitialization instead.")]
    public void CreateMutableConstructor_ThrowsIfStaticAndNonEmptyParameters ()
    {
      _mutableMemberFactory.CreateMutableConstructor (_mutableType, MethodAttributes.Static, ParameterDeclaration.EmptyParameters, ctx => null);
    }

    [Test]
    public void CreateMutableConstructor_ThrowsIfAlreadyExists ()
    {
      var ctor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType());
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = ctx => Expression.Empty ();

      Assert.That (
          () => _mutableMemberFactory.CreateMutableConstructor (_mutableType, 0, ParameterDeclarationObjectMother.CreateMultiple (2), bodyProvider),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableConstructor (_mutableType, 0, ParameterDeclaration.CreateForEquivalentSignature (ctor), bodyProvider),
          Throws.ArgumentException.With.Message.EqualTo (
              "Constructor with equal signature already exists.\r\nParameter name: parameterDeclarations"));
    }

    [Test]
    public void CreateMutableMethod ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Public;
      var returnType = typeof (object);
      var parameterDeclarations = new[]
                                  {
                                      ParameterDeclarationObjectMother.Create (typeof (double), "hans"),
                                      ParameterDeclarationObjectMother.Create (typeof (string), "franz")
                                  };
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (int));
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.This.Type, Is.SameAs (_mutableType));
        Assert.That (ctx.Parameters.Select (p => p.Type), Is.EqualTo (new[] { typeof (double), typeof (string) }));
        Assert.That (ctx.Parameters.Select (p => p.Name), Is.EqualTo (new[] { "hans", "franz" }));
        Assert.That (ctx.IsStatic, Is.False);
        Assert.That (ctx.HasBaseMethod, Is.False);

        return fakeBody;
      };

      var method = _mutableMemberFactory.CreateMutableMethod (
          _mutableType, name, attributes, returnType, parameterDeclarations.AsOneTime(), bodyProvider);

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
      var actualParameterInfos = method.GetParameters ().Select (pi => new { pi.ParameterType });
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
      var expectedBody = Expression.Convert (fakeBody, returnType);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }

    [Test]
    public void CreateMutableMethod_Static ()
    {
      var name = "StaticMethod";
      var attributes = MethodAttributes.Static;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);

      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.IsStatic, Is.True);

        return ExpressionTreeObjectMother.GetSomeExpression (returnType);
      };
      var method = _mutableMemberFactory.CreateMutableMethod (_mutableType, name, attributes, returnType, parameterDeclarations, bodyProvider);

      Assert.That (method.IsStatic, Is.True);
    }

    [Test]
    public void CreateMutableMethod_Shadowing_NonVirtual ()
    {
      var baseMethod = GetBaseMethod (_mutableType, "ToString");
      Assert.That (baseMethod, Is.Not.Null);
      Assert.That (baseMethod.DeclaringType, Is.SameAs (typeof (object)));

      var nonVirtualAttributes = (MethodAttributes) 0;
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.HasBaseMethod, Is.False);
        return Expression.Constant ("string");
      };
      var method = _mutableMemberFactory.CreateMutableMethod (
          _mutableType,
          "ToString",
          nonVirtualAttributes,
          typeof (string),
          ParameterDeclaration.EmptyParameters,
          bodyProvider);

      Assert.That (method, Is.Not.Null.And.Not.EqualTo (baseMethod));
      Assert.That (method.BaseMethod, Is.Null);
      Assert.That (method.GetBaseDefinition (), Is.SameAs (method));
    }

    [Test]
    public void CreateMutableMethod_Shadowing_VirtualAndNewSlot ()
    {
      var baseMethod = GetBaseMethod (_mutableType, "ToString");
      Assert.That (baseMethod, Is.Not.Null);
      Assert.That (baseMethod.DeclaringType, Is.SameAs (typeof (object)));

      var nonVirtualAttributes = MethodAttributes.Virtual | MethodAttributes.NewSlot;
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.HasBaseMethod, Is.False);
        return Expression.Constant ("string");
      };
      var method = _mutableMemberFactory.CreateMutableMethod (
          _mutableType,
          "ToString",
          nonVirtualAttributes,
          typeof (string),
          ParameterDeclaration.EmptyParameters,
          bodyProvider);

      Assert.That (method, Is.Not.Null.And.Not.EqualTo (baseMethod));
      Assert.That (method.BaseMethod, Is.Null);
      Assert.That (method.GetBaseDefinition (), Is.SameAs (method));
    }

    [Test]
    public void CreateMutableMethod_ImplicitOverride ()
    {
      var fakeOverridenMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.OverrideHierarchy (7));
      _relatedMethodFinderMock
          .Expect (mock => mock.GetMostDerivedVirtualMethod ("Method", new MethodSignature (typeof (int), Type.EmptyTypes, 0), _mutableType.BaseType))
          .Return (fakeOverridenMethod);

      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx =>
      {
        Assert.That (ctx.HasBaseMethod, Is.True);
        Assert.That (ctx.BaseMethod, Is.SameAs (fakeOverridenMethod));

        return Expression.Default (typeof (int));
      };
      var method = _mutableMemberFactory.CreateMutableMethod (
          _mutableType,
          "Method",
          MethodAttributes.Public | MethodAttributes.Virtual,
          typeof (int),
          ParameterDeclaration.EmptyParameters,
          bodyProvider);

      _relatedMethodFinderMock.VerifyAllExpectations ();
      Assert.That (method.BaseMethod, Is.EqualTo (fakeOverridenMethod));
      Assert.That (method.GetBaseDefinition (), Is.EqualTo (fakeOverridenMethod.GetBaseDefinition ()));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Non-abstract methods must have a body.\r\nParameter name: bodyProvider")]
    public void CreateMutableMethod_ThrowsIfNotAbstractAndNullBodyProvider ()
    {
      _mutableMemberFactory.CreateMutableMethod (_mutableType, "NotImportant", 0, typeof (void), ParameterDeclaration.EmptyParameters, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Abstract methods cannot have a body.\r\nParameter name: bodyProvider")]
    public void CreateMutableMethod_ThrowsIfAbstractAndBodyProvider ()
    {
      _mutableMemberFactory.CreateMutableMethod (
          _mutableType, "NotImportant", MethodAttributes.Abstract, typeof (void), ParameterDeclaration.EmptyParameters, ctx => null);
    }

    [Test]
    public void CreateMutableMethod_ThrowsForInvalidMethodAttributes ()
    {
      const string message = "The following MethodAttributes are not supported for methods: " +
                             "PinvokeImpl, RequireSecObject, UnmanagedExport.\r\nParameter name: attributes";
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.PinvokeImpl), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.RequireSecObject), Throws.ArgumentException.With.Message.EqualTo (message));
      Assert.That (() => AddMethod (_mutableType, MethodAttributes.UnmanagedExport), Throws.ArgumentException.With.Message.EqualTo (message));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Abstract methods must also be virtual.\r\nParameter name: attributes")]
    public void CreateMutableMethod_ThrowsIfAbstractAndNotVirtual ()
    {
      _mutableMemberFactory.CreateMutableMethod (
          _mutableType, "NotImportant", MethodAttributes.Abstract, typeof (void), ParameterDeclaration.EmptyParameters, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "NewSlot methods must also be virtual.\r\nParameter name: attributes")]
    public void CreateMutableMethod_ThrowsIfNonVirtualAndNewSlot ()
    {
      _mutableMemberFactory.CreateMutableMethod (
          _mutableType, "NotImportant", MethodAttributes.NewSlot, typeof (void), ParameterDeclaration.EmptyParameters, ctx => Expression.Empty());
    }

    [Test]
    public void CreateMutableMethod_ThrowsIfAlreadyExists ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InterfaceMethod());
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx => Expression.Empty();

      Assert.That (
          () => _mutableMemberFactory.CreateMutableMethod (
              _mutableType, "OtherName", 0, method.ReturnType, ParameterDeclaration.CreateForEquivalentSignature (method), bodyProvider),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableMethod (
              _mutableType, method.Name, 0, typeof (int), ParameterDeclaration.CreateForEquivalentSignature (method), ctx => Expression.Constant (7)),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableMethod (
              _mutableType, method.Name, 0, method.ReturnType, ParameterDeclarationObjectMother.CreateMultiple (2), bodyProvider),
          Throws.Nothing);

      Assert.That (
          () => _mutableMemberFactory.CreateMutableMethod (
              _mutableType, method.Name, 0, method.ReturnType, ParameterDeclaration.CreateForEquivalentSignature (method), bodyProvider),
          Throws.ArgumentException.With.Message.EqualTo (
              "Method 'InterfaceMethod' with equal signature already exists.\r\nParameter name: parameterDeclarations"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot override final method 'B.FinalBaseMethodInB'.")]
    public void CreateMutableMethod_ThrowsIfOverridingFinalMethod ()
    {
      var signature = new MethodSignature (typeof (void), Type.EmptyTypes, 0);
      var fakeBaseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.FinalBaseMethodInB (7));
      _relatedMethodFinderMock
          .Expect (mock => mock.GetMostDerivedVirtualMethod ("MethodName", signature, _mutableType.BaseType))
          .Return (fakeBaseMethod);

      _mutableMemberFactory.CreateMutableMethod (
          _mutableType,
          "MethodName",
          MethodAttributes.Public | MethodAttributes.Virtual,
          typeof (void),
          ParameterDeclaration.EmptyParameters,
          ctx => Expression.Empty ());
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_ExistingOverride ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString());
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.ToString());
      Assert.That (method, Is.Not.EqualTo (baseDefinition));

      var fakeExistingOverride = MutableMethodInfoObjectMother.Create ();
      _relatedMethodFinderMock
          .Expect (mock => mock.GetOverride (Arg.Is (baseDefinition), Arg<IEnumerable<MutableMethodInfo>>.List.Equal (_mutableType.AllMutableMethods)))
          .Return (fakeExistingOverride);

      bool isNewlyCreated;
      var result = _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, method, out isNewlyCreated);

      _relatedMethodFinderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeExistingOverride));
      Assert.That (isNewlyCreated, Is.False);
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_BaseMethod_ImplicitOverride ()
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
    public void GetOrCreateMutableMethodOverride_BaseMethod_ImplicitOverride_AdjustsAttributes ()
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
    public void GetOrCreateMutableMethodOverride_BaseMethod_ImplicitOverride_Abstract ()
    {
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((AbstractTypeWithOneMethod obj) => obj.Method());
      Assert.That (baseMethod.Attributes.IsSet (MethodAttributes.Abstract), Is.True);
      var mutableType = MutableTypeObjectMother.CreateForExisting (
          typeof (DerivedAbstractTypeLeavesAbstractBaseMethod), relatedMethodFinder: _relatedMethodFinderMock);
      SetupExpectationsForGetOrAddMutableMethod (baseMethod, baseMethod, false, baseMethod, mutableType, typeof (AbstractTypeWithOneMethod));

      var result = _mutableMemberFactory.GetOrCreateMutableMethodOverride (mutableType, baseMethod, out _isNewlyCreated);

      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (result.BaseMethod, Is.SameAs (baseMethod));
      Assert.That (result.IsAbstract, Is.True);
      Assert.That (_isNewlyCreated, Is.True);
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_ShadowedBaseMethod_ExplicitOverride ()
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
          "Remotion.TypePipe.UnitTests.MutableReflection.Implementation.MutableMemberFactoryTest+C_OverrideHierarchy",
          MethodAttributes.Private,
          MethodAttributes.NewSlot);
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_InterfaceMethod_ExistingImplementation ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.InterfaceMethod());

      bool isNewlyCreated;
      var result = _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, interfaceMethod, out isNewlyCreated);

      var implementation = _mutableType.AllMutableMethods.Single (m => m.Name == "InterfaceMethod");
      Assert.That (result, Is.SameAs (implementation));
      Assert.That (isNewlyCreated, Is.False);
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_InterfaceMethod_AddImplementation ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IAddedInterface obj) => obj.AddedInterfaceMethod (7));
      _mutableType.AddInterface (typeof (IAddedInterface));

      bool isNewlyCreated;
      var result = _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, interfaceMethod, out isNewlyCreated);

      Assert.That (isNewlyCreated, Is.True);
      CheckMethodData (
          result,
          "AddedInterfaceMethod",
          MethodAttributes.Public,
          MethodAttributes.NewSlot | MethodAttributes.Abstract,
          expectedBaseMethod: null,
          expectedAddedExplicitBaseDefinitions: new MethodInfo[0]);

      Assert.That (result.GetBaseDefinition(), Is.SameAs (result));
      Assert.That (result.IsAbstract, Is.True);
    }

    [Test]
    public void GetOrCreateMutableMethodOverride_InterfaceMethod_OverrideImplementationInBase ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IBaseInterface obj) => obj.BaseInterfaceMethod (7));
      var implementation = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.BaseInterfaceMethod (7));

      CallAndCheckGetOrAddMutableMethod (
          implementation,
          interfaceMethod,
          implementation,
          false,
          implementation,
          new MethodInfo[0],
          "BaseInterfaceMethod",
          MethodAttributes.Public,
          MethodAttributes.ReuseSlot);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Interface method 'InvalidCandidate' cannot be implemented because a method with equal name and signature already exists. "
        + "Use MutableType.AddExplicitOverride to create an explicit implementation.")]
    public void GetOrCreateMutableMethodOverride_InterfaceMethod_InvalidCandidate ()
    {
      _mutableType.AddInterface (typeof (IAddedInterface));
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IAddedInterface obj) => obj.InvalidCandidate());
      _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, interfaceMethod, out _isNewlyCreated);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method is declared by a type outside of this type's class hierarchy: 'String'.\r\nParameter name: method")]
    public void GetOrCreateMutableMethodOverride_UnrelatedDeclaringType ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((string obj) => obj.Trim ());
      _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, method, out _isNewlyCreated);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "A method declared in a base type must be virtual in order to be modified.")]
    public void GetOrCreateMutableMethodOverride_NonVirtualMethod ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.NonVirtualBaseMethod());
      _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, method, out _isNewlyCreated);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot override final method 'B.FinalBaseMethodInB'.")]
    public void GetOrCreateMutableMethodOverride_FinalBaseMethod ()
    {
      var baseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((A obj) => obj.FinalBaseMethodInB (7));
      var inputMethod = baseDefinition;
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((B obj) => obj.FinalBaseMethodInB (7));
      var isBaseDefinitionShadowed = BooleanObjectMother.GetRandomBoolean ();

      SetupExpectationsForGetOrAddMutableMethod (baseDefinition, baseMethod, isBaseDefinitionShadowed, null);

      _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, inputMethod, out _isNewlyCreated);
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

      var result = _mutableMemberFactory.GetOrCreateMutableMethodOverride (_mutableType, inputMethod, out _isNewlyCreated);

      _relatedMethodFinderMock.VerifyAllExpectations ();
      Assert.That (_isNewlyCreated, Is.True);

      CheckMethodData (
        result, expectedOverrideMethodName, expectedOverrideVisibility, expectedVtableLayout, expectedBaseMethod, expectedAddedExplicitBaseDefinitions);
      Assert.That (result.Body, Is.InstanceOf<MethodCallExpression>());
      var methodCallExpression = (MethodCallExpression) result.Body;
      Assert.That (methodCallExpression.Method, Is.TypeOf<NonVirtualCallMethodInfoAdapter>());
      var baceCallMethodInfoAdapter = (NonVirtualCallMethodInfoAdapter) methodCallExpression.Method;
      Assert.That (baceCallMethodInfoAdapter.AdaptedMethod, Is.SameAs (baseMethod));
    }

    private static void CheckMethodData (
        MutableMethodInfo result,
        string expectedMethodName,
        MethodAttributes expectedVisibility,
        MethodAttributes expectedVtableLayout,
        MethodInfo expectedBaseMethod,
        IEnumerable<MethodInfo> expectedAddedExplicitBaseDefinitions)
    {
      Assert.That (result.Name, Is.EqualTo (expectedMethodName));
      var expectedAttributes = expectedVisibility | expectedVtableLayout | MethodAttributes.Virtual | MethodAttributes.HideBySig;
      Assert.That (result.Attributes, Is.EqualTo (expectedAttributes));
      Assert.That (result.ReturnType, Is.SameAs (typeof (void)));
      var parameter = result.GetParameters().Single();
      Assert.That (parameter.ParameterType, Is.SameAs (typeof (int)));
      Assert.That (parameter.Name, Is.EqualTo ("parameterName"));
      Assert.That (result.BaseMethod, Is.SameAs (expectedBaseMethod));
      Assert.That (result.AddedExplicitBaseDefinitions, Is.EqualTo (expectedAddedExplicitBaseDefinitions));
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
          .Expect (mock => mock.IsShadowed (Arg.Is (baseDefinition), Arg<IEnumerable<MethodInfo>>.List.Equal (GetAllMethods (mutableType))))
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
    
    private MutableConstructorInfo AddConstructor (MutableType mutableType, MethodAttributes attributes)
    {
      return _mutableMemberFactory.CreateMutableConstructor (
          mutableType, attributes, ParameterDeclaration.EmptyParameters, ctx => Expression.Empty ());
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, MethodAttributes attributes)
    {
      return _mutableMemberFactory.CreateMutableMethod (
          mutableType,
          "dummy",
          attributes,
          typeof (void),
          ParameterDeclaration.EmptyParameters,
          ctx => Expression.Empty ());
    }

    private MethodInfo GetBaseMethod (MutableType mutableType, string name)
    {
      return GetAllMethods (mutableType).ExistingBaseMembers.Single (mi => mi.Name == name);
    }

    private MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> GetAllMethods (MutableType mutableType)
    {
      return (MutableTypeMemberCollection<MethodInfo, MutableMethodInfo>) PrivateInvoke.GetNonPublicField (mutableType, "_methods");
    }

    public class A
    {
      // base definition
      public virtual void OverrideHierarchy (int aaa) { }

      public virtual void FinalBaseMethodInB (int i) { }
    }

    public class B : A
    {
      // CreateMutableMethodOverride input
      public override void OverrideHierarchy (int bbb) { }

      protected internal virtual void ProtectedOrInternalVirtualNewSlotMethodInB (int parameterName) { }
      public override sealed void FinalBaseMethodInB (int i) { }
    }

    public class C : B
    {
      // base inputMethod
      public override void OverrideHierarchy (int parameterName) { }
    }

    public class DomainTypeBase : C, IBaseInterface
    {
      public virtual void BaseInterfaceMethod (int parameterName) { }
      public void NonVirtualBaseMethod () { }
    }

    public class DomainType : DomainTypeBase, IDomainInterface
    {
      public int IntField;

      public void InterfaceMethod () { }
      public void InvalidCandidate () { } // Not virtual.
    }

    public interface IBaseInterface
    {
      void BaseInterfaceMethod (int parameterName);
    }
    public interface IDomainInterface
    {
      void InterfaceMethod ();
    }
    public interface IAddedInterface
    {
      void AddedInterfaceMethod (int parameterName);
      void InvalidCandidate ();
    }

    abstract class AbstractTypeWithOneMethod
    {
      public abstract void Method ();
    }
    abstract class DerivedAbstractTypeLeavesAbstractBaseMethod : AbstractTypeWithOneMethod { }

  }
}