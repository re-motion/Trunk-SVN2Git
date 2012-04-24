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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class SubclassProxyBuilderTest
  {
    private IExpressionPreparer _expressionPreparerMock;
    private ITypeBuilder _typeBuilderMock;
    private IILGeneratorFactory _ilGeneratorFactoryStub;
    private DebugInfoGenerator _debugInfoGeneratorStub;
    private EmittableOperandProvider _emittableOperandProvider;

    private SubclassProxyBuilder _builder;

    private Expression _fakeBody;

    [SetUp]
    public void SetUp ()
    {
      _expressionPreparerMock = MockRepository.GenerateStrictMock<IExpressionPreparer>();
      _typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      _ilGeneratorFactoryStub = MockRepository.GenerateStub<IILGeneratorFactory>();
      _debugInfoGeneratorStub = MockRepository.GenerateStub<DebugInfoGenerator>();
      _emittableOperandProvider = new EmittableOperandProvider();

      _builder = new SubclassProxyBuilder (
          _typeBuilderMock, _expressionPreparerMock, _emittableOperandProvider, _ilGeneratorFactoryStub, _debugInfoGeneratorStub);

      _fakeBody = ExpressionTreeObjectMother.GetSomeExpression();
    }

    [Test]
    public void Initialization_NullDebugInfoGenerator ()
    {
      var handler = new SubclassProxyBuilder (
          _typeBuilderMock, _expressionPreparerMock, _emittableOperandProvider, _ilGeneratorFactoryStub, null);
      Assert.That (handler.DebugInfoGenerator, Is.Null);
    }

    [Test]
    public void HandleAddedInterface ()
    {
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      _typeBuilderMock.Expect (mock => mock.AddInterfaceImplementation (addedInterface));

      _builder.HandleAddedInterface (addedInterface);

      _typeBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleAddedField ()
    {
      var addedField = MutableFieldInfoObjectMother.Create();
      var fieldBuilderMock = MockRepository.GenerateStrictMock<IFieldBuilder>();

      _typeBuilderMock
          .Expect(mock => mock.DefineField (addedField.Name, addedField.FieldType, addedField.Attributes))
          .Return (fieldBuilderMock);
      var emittableOperand = MockRepository.GenerateStub<IEmittableOperand>();
      fieldBuilderMock.Expect (mock => mock.GetEmittableOperand ()).Return (emittableOperand);

      _builder.HandleAddedField (addedField);

      _typeBuilderMock.VerifyAllExpectations();
      fieldBuilderMock.VerifyAllExpectations();
      Assert.That (_emittableOperandProvider.GetEmittableField (addedField), Is.SameAs (emittableOperand));
    }

    [Test]
    public void HandleAddedField_WithCustomAttribute ()
    {
      var constructor = MemberInfoFromExpressionUtility.GetConstructor (() => new CustomAttribute(""));
      var property = MemberInfoFromExpressionUtility.GetProperty ((CustomAttribute attr) => attr.Property);
      var field = MemberInfoFromExpressionUtility.GetField ((CustomAttribute attr) => attr.Field);
      var constructorArguments = new object[] { "ctorArgs" };
      var declaration = new CustomAttributeDeclaration (
          constructor,
          constructorArguments,
          new NamedAttributeArgumentDeclaration (property, 7),
          new NamedAttributeArgumentDeclaration (field, "test"));
      var addedField = MutableFieldInfoObjectMother.Create ();
      addedField.AddCustomAttribute (declaration);

      var fieldBuilderMock = MockRepository.GenerateMock<IFieldBuilder>();
      _typeBuilderMock
          .Stub (stub => stub.DefineField (addedField.Name, addedField.FieldType, addedField.Attributes))
          .Return (fieldBuilderMock);
      var emittableOperand = MockRepository.GenerateStub<IEmittableOperand>();
      fieldBuilderMock.Expect (mock => mock.GetEmittableOperand()).Return (emittableOperand);
      fieldBuilderMock
          .Expect (mock => mock.SetCustomAttribute (Arg<CustomAttributeBuilder>.Is.Anything))
          .WhenCalled (mi => CheckCustomAttributeBuilder (
              (CustomAttributeBuilder) mi.Arguments[0], 
              constructor,
              constructorArguments,
              new[] { property },
              new object[]  { 7 },
              new[] { field },
              new[] { "test" }));
      
      _builder.HandleAddedField (addedField);

      fieldBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleAddedField_Throws ()
    {
      var message = "The supplied field must be a new field.\r\nParameter name: field";
      // Modifying existing fields is not supported (TODO 4695)
      //CheckThrowsForInvalidArguments (_builder.HandleAddedField, message, isNew: false, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleAddedField, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleAddedConstructor ()
    {
      var ctor = MutableConstructorInfoObjectMother.CreateForNewWithParameters (
          ParameterDeclarationObjectMother.Create (typeof (string), "p1", ParameterAttributes.In),
          ParameterDeclarationObjectMother.Create (typeof (int).MakeByRefType(), "p2", ParameterAttributes.Out));
      var expectedAttributes = ctor.Attributes;
      var expectedParameterTypes = new[] { typeof (string), typeof (int).MakeByRefType() };
      Action<IConstructorBuilder> expectedParameterDefinitions = methodBuilderMock =>
      {
        methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.In, "p1"));
        methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "p2"));
      };

      CheckConstrucorIsDefined (_builder.HandleAddedConstructor, ctor, expectedAttributes, expectedParameterTypes, expectedParameterDefinitions);
    }

    [Test]
    public void HandleAddedConstructor_RegistersBuildAction ()
    {
      var constructor = MutableConstructorInfoObjectMother.CreateForNew();
      CheckSetBodyBuildActionIsRegistered (_builder.HandleAddedConstructor, constructor);
    }

    [Test]
    public void HandleAddedConstructor_Throws ()
    {
      var message = "The supplied constructor must be a new constructor.\r\nParameter name: constructor";
      CheckThrowsForInvalidArguments (_builder.HandleAddedConstructor, message, isNew: false, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleAddedConstructor, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleAddedMethod_DefinesMethod ()
    {
      var mutableMethod = MutableMethodInfoObjectMother.CreateForNew (
          parameterDeclarations: new[]
                                 {
                                     ParameterDeclarationObjectMother.Create (typeof (string), "p1", ParameterAttributes.In),
                                     ParameterDeclarationObjectMother.Create (typeof (int).MakeByRefType(), "p2", ParameterAttributes.Out)
                                 });

      var expectedName = mutableMethod.Name;
      var expectedAttributes = mutableMethod.Attributes;
      var expectedReturnType = mutableMethod.ReturnType;
      var expectedParameterTypes = new[] { typeof (string), typeof (int).MakeByRefType () };
      Action<IMethodBuilder> expectedParameterDefinitions = methodBuilderMock =>
      {
        methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.In, "p1"));
        methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "p2"));
      };

      CheckMethodIsDefined (
          _builder.HandleAddedMethod,
          mutableMethod,
          expectedName,
          expectedAttributes,
          expectedReturnType,
          expectedParameterTypes,
          false,
          null,
          expectedParameterDefinitions);
    }

    [Test]
    public void HandleAddedMethod_RegistersBuildAction ()
    {
      var mutableMethod = MutableMethodInfoObjectMother.Create ();
      CheckSetBodyBuildActionIsRegistered (_builder.HandleAddedMethod, mutableMethod);
    }

    [Test]
    public void HandleAddedMethod_Throws ()
    {
      var message = "The supplied method must be a new method.\r\nParameter name: method";
      CheckThrowsForInvalidArguments (_builder.HandleAddedMethod, message, isNew: false, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleAddedMethod, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleModifiedConstructor ()
    {
      var originalCtor = MemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7, out Dev<double>.Dummy));
      var ctor = MutableConstructorInfoObjectMother.CreateForExistingAndModify (originalCtor);

      var expectedAttributes = ctor.Attributes;
      var expectedParameterTypes = new[] { typeof (int), typeof (double).MakeByRefType () };
      Action<IConstructorBuilder> expectedParameterDefinitions = methodBuilderMock =>
      {
        methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.None, "i"));
        methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "d"));
      };

      CheckConstrucorIsDefined (_builder.HandleModifiedConstructor, ctor, expectedAttributes, expectedParameterTypes, expectedParameterDefinitions);
    }

    [Test]
    public void HandleModifiedConstructor_RegistersBuildAction ()
    {
      var constructor = MutableConstructorInfoObjectMother.CreateForExistingAndModify();
      CheckSetBodyBuildActionIsRegistered (_builder.HandleModifiedConstructor, constructor);
    }

    [Test]
    public void HandleModifiedConstructor_Throws ()
    {
      var message = "The supplied constructor must be a modified existing constructor.\r\nParameter name: constructor";
      CheckThrowsForInvalidArguments (_builder.HandleModifiedConstructor, message, isNew: true, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleModifiedConstructor, message, isNew: true, isModified: false);
      CheckThrowsForInvalidArguments (_builder.HandleModifiedConstructor, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleModifiedMethod_DefinesMethod ()
    {
      var originalMethod = MemberInfoFromExpressionUtility.GetMethod ((DomainType dt) => dt.Method (7, out Dev<double>.Dummy));
      var modifiedMethod = MutableMethodInfoObjectMother.CreateForExistingAndModify (originalMethodInfo: originalMethod);

      var expectedName = "Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit.SubclassProxyBuilderTest+DomainType.Method";
      var expectedAttributes = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig;
      var expectedReturnType = typeof(string);
      var expectedParameterTypes = new[] { typeof (int), typeof (double).MakeByRefType() };
      Action<IMethodBuilder> expectedParameterDefinitions = methodBuilderMock =>
      {
        methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.None, "i"));
        methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "d"));
      };

      CheckMethodIsDefined (
          _builder.HandleModifiedMethod,
          modifiedMethod,
          expectedName,
          expectedAttributes,
          expectedReturnType,
          expectedParameterTypes,
          true,
          originalMethod,
          expectedParameterDefinitions);
    }

    [Test]
    public void HandleModifiedMethod_RegistersBuildAction ()
    {
      var modifiedMethod = MutableMethodInfoObjectMother.CreateForExistingAndModify();
      CheckSetBodyBuildActionIsRegistered (_builder.HandleModifiedMethod, modifiedMethod);
    }

    [Test]
    public void HandleModifiedMethod_Throws ()
    {
      var message = "The supplied method must be a modified existing method.\r\nParameter name: method";
      CheckThrowsForInvalidArguments (_builder.HandleModifiedMethod, message, isNew: true, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleModifiedMethod, message, isNew: true, isModified: false);
      CheckThrowsForInvalidArguments (_builder.HandleModifiedMethod, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleUnmodifiedField ()
    {
      var field = MutableFieldInfoObjectMother.CreateForExisting();

      _builder.HandleUnmodifiedField (field);

      var result = _emittableOperandProvider.GetEmittableField (field);
      Assert.That (result, Is.TypeOf<EmittableField>());

      var innerField = PrivateInvoke.GetNonPublicField (result, "_fieldInfo");
      Assert.That (innerField, Is.SameAs (field.UnderlyingSystemFieldInfo));
    }

    [Test]
    public void HandleUnmodifiedField_Throws()
    {
      var message = "The supplied field must be a unmodified existing field.\r\nParameter name: field";
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedField, message, isNew: true, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedField, message, isNew: true, isModified: false);
      // Modifying existing fields is not supported (TODO 4695)
      //CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedField, message, isNew: false, isModified: true);
    }

    [Test]
    public void HandleUnmodifiedConstructor ()
    {
      var originalCtor = MemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7, out Dev<double>.Dummy));
      var ctor = MutableConstructorInfoObjectMother.CreateForExisting (originalCtor);

      var expectedAttributes = ctor.Attributes;
      var expectedParameterTypes = new[] { typeof (int), typeof (double).MakeByRefType () };
      Action<IConstructorBuilder> expectedParameterDefinitions = methodBuilderMock =>
      {
        methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.None, "i"));
        methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "d"));
      };

      CheckConstrucorIsDefined (_builder.HandleUnmodifiedConstructor, ctor, expectedAttributes, expectedParameterTypes, expectedParameterDefinitions);
    }

    [Test]
    public void HandleUnmodifiedConstructor_RegistersBuildAction ()
    {
      var constructor = MutableConstructorInfoObjectMother.CreateForExisting();
      CheckSetBodyBuildActionIsRegistered (_builder.HandleUnmodifiedConstructor, constructor);
    }

    [Test]
    public void HandleUnmodifiedConstructor_Throws ()
    {
      var message = "The supplied constructor must be a unmodified existing constructor.\r\nParameter name: constructor";
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedConstructor, message, isNew: true, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedConstructor, message, isNew: true, isModified: false);
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedConstructor, message, isNew: false, isModified: true);
    }

    [Test]
    public void HandleUnmodifiedMethod ()
    {
      var method = MutableMethodInfoObjectMother.CreateForExisting ();

      _builder.HandleUnmodifiedMethod (method);

      var result = _emittableOperandProvider.GetEmittableMethod (method);
      Assert.That (result, Is.TypeOf<EmittableMethod> ());

      var innerMethod = PrivateInvoke.GetNonPublicField (result, "_methodInfo");
      Assert.That (innerMethod, Is.SameAs (method.UnderlyingSystemMethodInfo));
    }

    [Test]
    public void HandleUnmodifiedMethod_Throws ()
    {
      var message = "The supplied method must be a unmodified existing method.\r\nParameter name: method";
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedMethod, message, isNew: true, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedMethod, message, isNew: true, isModified: false);
      CheckThrowsForInvalidArguments (_builder.HandleUnmodifiedMethod, message, isNew: false, isModified: true);
    }

    [Test]
    public void Build ()
    {
      bool buildActionCalled = false;
      AddBuildAction (_builder, () => buildActionCalled = true);

      var fakeType = ReflectionObjectMother.GetSomeType();
      _typeBuilderMock
          .Expect (mock => mock.CreateType())
          .Return (fakeType)
          .WhenCalled (mi => Assert.That (buildActionCalled, Is.True));

      var result = _builder.Build ();

      _typeBuilderMock.VerifyAllExpectations();
      Assert.That (buildActionCalled, Is.True);
      Assert.That (result, Is.SameAs (fakeType));
    }

    [Test]
    public void Build_Twice ()
    {
      _typeBuilderMock.Stub (mock => mock.CreateType ());

      _builder.Build ();
      Assert.That (() => _builder.Build (), Throws.InvalidOperationException.With.Message.EqualTo ("Build can only be called once."));
    }

    [Test]
    public void Build_DisablesOperations ()
    {
      _typeBuilderMock.Stub (mock => mock.CreateType ());
      _builder.Build ();

      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedInterface (ReflectionObjectMother.GetSomeInterfaceType()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedField (MutableFieldInfoObjectMother.Create ()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedConstructor (MutableConstructorInfoObjectMother.CreateForNew()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedMethod (MutableMethodInfoObjectMother.CreateForNew()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleModifiedConstructor (MutableConstructorInfoObjectMother.CreateForExistingAndModify()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleModifiedMethod (MutableMethodInfoObjectMother.CreateForNew ()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedField (MutableFieldInfoObjectMother.CreateForExisting()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedConstructor (MutableConstructorInfoObjectMother.CreateForExisting()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedMethod (MutableMethodInfoObjectMother.CreateForExisting()));
    }

    private void CheckThrowsForOperationAfterBuild (Action action)
    {
      Assert.That (() => action(), Throws.InvalidOperationException.With.Message.EqualTo ("Subclass proxy has already been built."));
    }

    private void AddBuildAction (SubclassProxyBuilder handler, Action action)
    {
      var buildActions = GetBuildActions(handler);
      buildActions.Add (action);
    }

    private List<Action> GetBuildActions (SubclassProxyBuilder handler)
    {
      return (List<Action>) PrivateInvoke.GetNonPublicField (handler, "_buildActions");
    }

    private void CheckCustomAttributeBuilder (
        CustomAttributeBuilder builder,
        ConstructorInfo expectedCtor,
        object[] expectedCtorArgs,
        PropertyInfo[] expectedPropertyInfos,
        object[] expectedPropertyValues,
        FieldInfo[] expectedFieldInfos,
        object[] expectedFieldValues)
    {
      var actualConstructor = (ConstructorInfo) PrivateInvoke.GetNonPublicField (builder, "m_con");
      var actualConstructorArgs = (object[]) PrivateInvoke.GetNonPublicField (builder, "m_constructorArgs");
      var actualBlob = (byte[]) PrivateInvoke.GetNonPublicField (builder, "m_blob");

      Assert.That (actualConstructor, Is.SameAs (expectedCtor));
      Assert.That (actualConstructorArgs, Is.EqualTo (expectedCtorArgs));

      var testBuilder = new CustomAttributeBuilder (
          expectedCtor, expectedCtorArgs, expectedPropertyInfos, expectedPropertyValues, expectedFieldInfos, expectedFieldValues);
      var expectedBlob = (byte[]) PrivateInvoke.GetNonPublicField (testBuilder, "m_blob");
      Assert.That (actualBlob, Is.EqualTo (expectedBlob));
    }

    private void CheckThrowsForInvalidArguments (Action<MutableFieldInfo> testedAction, string exceptionMessage, bool isNew, bool isModified)
    {
      var field = isNew ? MutableFieldInfoObjectMother.CreateForNew () : MutableFieldInfoObjectMother.CreateForExisting ();
      if (isModified)
        MutableFieldInfoTestHelper.ModifyField (field);

      CheckThrowsForInvalidArguments (testedAction, field, isNew, isModified, exceptionMessage);
    }

    private void CheckThrowsForInvalidArguments (Action<MutableConstructorInfo> testedAction, string exceptionMessage, bool isNew, bool isModified)
    {
      var constructor = isNew ? MutableConstructorInfoObjectMother.CreateForNew() : MutableConstructorInfoObjectMother.CreateForExisting();
      if (isModified)
        MutableConstructorInfoTestHelper.ModifyConstructor (constructor);

      CheckThrowsForInvalidArguments (testedAction, constructor, isNew, isModified, exceptionMessage);
    }

    private void CheckThrowsForInvalidArguments (Action<MutableMethodInfo> testedAction, string exceptionMessage, bool isNew, bool isModified)
    {
      var method = isNew
                       ? MutableMethodInfoObjectMother.CreateForNew (attributes: MethodAttributes.Virtual)
                       : MutableMethodInfoObjectMother.CreateForExisting (
                           originalMethodInfo: MemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString()));
      if (isModified)
        MutableMethodInfoTestHelper.ModifyMethod (method);

      CheckThrowsForInvalidArguments (testedAction, method, isNew, isModified, exceptionMessage);
    }

    private void CheckThrowsForInvalidArguments<T> (Action<T> testedAction, T mutableMethodBase, bool isNew, bool isModified, string exceptionMessage)
        where T: IMutableMember
    {
      Assert.That (mutableMethodBase.IsNew, Is.EqualTo (isNew));
      Assert.That (mutableMethodBase.IsModified, Is.EqualTo (isModified));

      Assert.That (() => testedAction (mutableMethodBase), Throws.ArgumentException.With.Message.EqualTo (exceptionMessage));
    }

    private void CheckConstrucorIsDefined (
        Action<MutableConstructorInfo> testedAction,
        MutableConstructorInfo definedConstructor,
        MethodAttributes expectedAttributes,
        Type[] expectedParameterTypes,
        Action<IConstructorBuilder> parameterDefinitionExpectationAction)
    {
      var constructorBuilderMock = MockRepository.GenerateStrictMock<IConstructorBuilder> ();
      _typeBuilderMock
          .Expect (mock => mock.DefineConstructor (expectedAttributes, CallingConventions.HasThis, expectedParameterTypes))
          .Return (constructorBuilderMock);
      var emittableOperandStub = MockRepository.GenerateStub<IEmittableOperand> ();
      constructorBuilderMock.Expect (mock => mock.GetEmittableOperand ()).Return (emittableOperandStub);

      _expressionPreparerMock
          .Expect (mock => mock.PrepareConstructorBody (definedConstructor))
          .Return (_fakeBody)
          .WhenCalled (mi => Assert.That (_emittableOperandProvider.GetEmittableConstructor (definedConstructor), Is.SameAs (emittableOperandStub)));

      parameterDefinitionExpectationAction (constructorBuilderMock);

      testedAction (definedConstructor);

      _typeBuilderMock.VerifyAllExpectations ();
      _expressionPreparerMock.VerifyAllExpectations ();
      constructorBuilderMock.VerifyAllExpectations ();
    }

    private void CheckMethodIsDefined (
        Action<MutableMethodInfo> testedAction,
        MutableMethodInfo definedMethod,
        string expectedName,
        MethodAttributes expectedAttributes,
        Type expectedReturnType,
        Type[] expectedParameterTypes,
        bool expectOverride,
        MethodInfo overriddenMethod,
        Action<IMethodBuilder> parameterDefinitionExpectationAction)
    {
      var methodBuilderMock = MockRepository.GenerateStrictMock<IMethodBuilder> ();
      _typeBuilderMock
          .Expect (mock => mock.DefineMethod (expectedName, expectedAttributes, expectedReturnType, expectedParameterTypes))
          .Return (methodBuilderMock);
      var emittableMethodOperand = MockRepository.GenerateStrictMock<IEmittableMethodOperand>();
      methodBuilderMock.Expect (mock => mock.GetEmittableOperand()).Return (emittableMethodOperand);

      if (expectOverride)
        methodBuilderMock.Expect (mock => mock.DefineOverride (overriddenMethod));

      _expressionPreparerMock
          .Expect (mock => mock.PrepareMethodBody (definedMethod))
          .Return (_fakeBody)
          .WhenCalled (mi => Assert.That (_emittableOperandProvider.GetEmittableMethod (definedMethod), Is.SameAs (emittableMethodOperand)));

      parameterDefinitionExpectationAction (methodBuilderMock);

      testedAction (definedMethod);

      _typeBuilderMock.VerifyAllExpectations ();
      _expressionPreparerMock.VerifyAllExpectations ();
      methodBuilderMock.VerifyAllExpectations ();
    }

    private void CheckSetBodyBuildActionIsRegistered (Action<MutableConstructorInfo> testedAction, MutableConstructorInfo constructor)
    {
      // Use a dynamic mock to ignore any DefineParameter calls.
      var constructorBuilderMock = MockRepository.GenerateMock<IConstructorBuilder> ();
      _typeBuilderMock
          .Stub (mock => mock.DefineConstructor (Arg<MethodAttributes>.Is.Anything, Arg<CallingConventions>.Is.Anything, Arg<Type[]>.Is.Anything))
          .Return (constructorBuilderMock);
      var emittableOperand = MockRepository.GenerateStub<IEmittableOperand> ();
      constructorBuilderMock.Expect (mock => mock.GetEmittableOperand ()).Return (emittableOperand);

      _expressionPreparerMock.Stub (mock => mock.PrepareConstructorBody (constructor)).Return (_fakeBody);

      Assert.That (GetBuildActions (_builder), Has.Count.EqualTo (0));

      testedAction (constructor);

      CheckSingleSetBodyBuildAction (constructorBuilderMock, constructor.ParameterExpressions);
    }

    private void CheckSetBodyBuildActionIsRegistered (Action<MutableMethodInfo> testedAction, MutableMethodInfo method)
    {
      // Use a dynamic mock to ignore any DefineParameter and DefineOverride calls.
      var methodBuilderMock = MockRepository.GenerateMock<IMethodBuilder> ();
      _typeBuilderMock
          .Stub (mock => mock.DefineMethod (Arg<string>.Is.Anything, Arg<MethodAttributes>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything))
          .Return (methodBuilderMock);
      var emittableMethodOperand = MockRepository.GenerateStub<IEmittableMethodOperand> ();
      methodBuilderMock.Expect (mock => mock.GetEmittableOperand ()).Return (emittableMethodOperand);

      _expressionPreparerMock.Stub (mock => mock.PrepareMethodBody (method)).Return (_fakeBody);

      Assert.That (GetBuildActions (_builder), Has.Count.EqualTo (0));

      testedAction (method);

      CheckSingleSetBodyBuildAction (methodBuilderMock, method.ParameterExpressions);
    }

    private void CheckSingleSetBodyBuildAction (IMethodBaseBuilder methodBuilderMock, IEnumerable<ParameterExpression> parameterExpressions)
    {
      // To check the build action registers by AddConstructor, we need to invoke it and observe its effects.
      methodBuilderMock
          .Expect (mock => mock.SetBody (Arg<LambdaExpression>.Is.Anything, Arg.Is (_ilGeneratorFactoryStub), Arg.Is (_debugInfoGeneratorStub)))
          .WhenCalled (
              mi =>
              {
                var lambdaExpression = (LambdaExpression) mi.Arguments[0];
                Assert.That (lambdaExpression.Body, Is.SameAs (_fakeBody));
                Assert.That (lambdaExpression.Parameters, Is.EqualTo (parameterExpressions));
              });

      var buildActions = GetBuildActions (_builder);
      Assert.That (buildActions, Has.Count.EqualTo (1));
      var action = buildActions.Single ();
      action ();

      methodBuilderMock.VerifyAllExpectations ();
    }

    public class CustomAttribute : Attribute
    {
// ReSharper disable UnassignedField.Global
      public string Field;
// ReSharper restore UnassignedField.Global

      public CustomAttribute (string ctorArgument)
      {
        CtorArgument = ctorArgument;

        Dev.Null = CtorArgument;
        Dev.Null = Property;
        Property = 0;
      }

      public string CtorArgument { get; private set; }
      public int Property { get; set; }
    }

    public class DomainType
    {
      public DomainType (int i, out double d)
      {
        Dev.Null = i;
        d = Dev<double>.Null;
      }

      public virtual string Method (int i, out double d)
      {
        Dev.Null = i;
        d = Dev<double>.Null;
        return "";
      }
    }
  }
}