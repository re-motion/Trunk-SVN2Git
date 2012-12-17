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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class MemberEmitterTest
  {
    private IExpressionPreparer _expressionPreparerMock;
    private IILGeneratorFactory _ilGeneratorFactoryStub;

    private MemberEmitter _emitter;

    private ITypeBuilder _typeBuilderMock;
    private IEmittableOperandProvider _emittableOperandProviderMock;

    private MemberEmitterContext _context;

    private Expression _fakeBody;

    [SetUp]
    public void SetUp ()
    {
      _expressionPreparerMock = MockRepository.GenerateStrictMock<IExpressionPreparer>();
      _ilGeneratorFactoryStub = MockRepository.GenerateStub<IILGeneratorFactory>();

      _emitter = new MemberEmitter (_expressionPreparerMock, _ilGeneratorFactoryStub);

      _typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      _emittableOperandProviderMock = MockRepository.GenerateStrictMock<IEmittableOperandProvider>();

      _context = MemberEmitterContextObjectMother.GetSomeContext (
          typeBuilder: _typeBuilderMock, emittableOperandProvider: _emittableOperandProviderMock);

      _fakeBody = ExpressionTreeObjectMother.GetSomeExpression();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_emitter.ExpressionPreparer, Is.SameAs (_expressionPreparerMock));
      Assert.That (_emitter.ILGeneratorFactory, Is.SameAs (_ilGeneratorFactoryStub));
    }

    [Test]
    public void AddField ()
    {
      var field = MutableFieldInfoObjectMother.Create();
      var fieldBuilderMock = MockRepository.GenerateStrictMock<IFieldBuilder>();

      _typeBuilderMock
          .Expect (mock => mock.DefineField (field.Name, field.FieldType, field.Attributes))
          .Return (fieldBuilderMock);
      fieldBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, field));
      SetupDefineCustomAttribute (fieldBuilderMock, field);

      _emitter.AddField (_context, field);

      _typeBuilderMock.VerifyAllExpectations();
      fieldBuilderMock.VerifyAllExpectations();
    }

    [Test]
    public void AddConstructor ()
    {
      var constructor = MutableConstructorInfoObjectMother.CreateForNewWithParameters (
          new ParameterDeclaration (typeof (string), "p1", ParameterAttributes.In),
          new ParameterDeclaration (typeof (int).MakeByRefType(), "p2", ParameterAttributes.Out));
      var expectedAttributes = constructor.Attributes;
      var expectedParameterTypes = new[] { typeof (string), typeof (int).MakeByRefType() };

      var constructorBuilderMock = MockRepository.GenerateStrictMock<IConstructorBuilder>();
      _typeBuilderMock
          .Expect (mock => mock.DefineConstructor (expectedAttributes, CallingConventions.HasThis, expectedParameterTypes))
          .Return (constructorBuilderMock);
      constructorBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, constructor));

      SetupDefineCustomAttribute (constructorBuilderMock, constructor);
      constructorBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.In, "p1"));
      constructorBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "p2"));

      Assert.That (_context.PostDeclarationsActionManager.Actions, Is.Empty);

      _emitter.AddConstructor (_context, constructor);

      _typeBuilderMock.VerifyAllExpectations();
      constructorBuilderMock.VerifyAllExpectations();

      Assert.That (_context.PostDeclarationsActionManager.Actions.Count(), Is.EqualTo (1));
      CheckBodyBuildAction (_context.PostDeclarationsActionManager.Actions.Single(), constructorBuilderMock, constructor);
    }

    [Test]
    public void AddConstructor_Static ()
    {
      var ctor = MutableConstructorInfoObjectMother.Create (attributes: MethodAttributes.Static);
      var constructorBuilderStub = MockRepository.GenerateStub<IConstructorBuilder> ();
      _typeBuilderMock
          .Expect (mock => mock.DefineConstructor (MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes))
          .Return (constructorBuilderStub);

      _emitter.AddConstructor (_context, ctor);

      _typeBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void AddMethod ()
    {
      var method = MutableMethodInfoObjectMother.CreateForNew (
          MutableTypeObjectMother.CreateForExisting (typeof (DomainType)),
          "Method",
          MethodAttributes.Virtual,
          typeof (string),
          new[]
          {
              new ParameterDeclaration (typeof (int), "i", ParameterAttributes.None),
              new ParameterDeclaration (typeof (double).MakeByRefType(), "d", ParameterAttributes.Out)
          });

      var overriddenMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType dt) => dt.ExplicitBaseDefinition (7, out Dev<double>.Dummy));
      method.AddExplicitBaseDefinition (overriddenMethod);

      var expectedAttributes = MethodAttributes.HideBySig;
      var expectedParameterTypes = new[] { typeof (int), typeof (double).MakeByRefType() };

      var methodBuilderMock = MockRepository.GenerateStrictMock<IMethodBuilder>();
      _typeBuilderMock
          .Expect (mock => mock.DefineMethod ("Method", expectedAttributes, typeof (string), expectedParameterTypes))
          .Return (methodBuilderMock);
      methodBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, method));

      SetupDefineCustomAttribute (methodBuilderMock, method);
      methodBuilderMock.Expect (mock => mock.DefineParameter (1, ParameterAttributes.None, "i"));
      methodBuilderMock.Expect (mock => mock.DefineParameter (2, ParameterAttributes.Out, "d"));

      Assert.That (_context.PostDeclarationsActionManager.Actions, Is.Empty);

      _emitter.AddMethod (_context, method, expectedAttributes);

      _typeBuilderMock.VerifyAllExpectations ();
      methodBuilderMock.VerifyAllExpectations ();
      var actions = _context.PostDeclarationsActionManager.Actions.ToArray();
      Assert.That (actions, Has.Length.EqualTo (2));

      CheckBodyBuildAction (actions[0], methodBuilderMock, method);
      CheckExplicitOverrideAction (actions[1], overriddenMethod, method);
    }

    [Test]
    public void AddMethod_Abstract ()
    {
      var method = MutableMethodInfoObjectMother.CreateForNew (
          MutableTypeObjectMother.CreateForExisting (typeof (DomainType)),
          "AbstractMethod",
          MethodAttributes.Abstract,
          typeof (int),
          ParameterDeclaration.EmptyParameters);

      var methodBuilderMock = MockRepository.GenerateStrictMock<IMethodBuilder> ();
      _typeBuilderMock
          .Expect (mock => mock.DefineMethod ("AbstractMethod", MethodAttributes.Abstract, typeof (int), Type.EmptyTypes))
          .Return (methodBuilderMock);
      methodBuilderMock.Expect (mock => mock.RegisterWith (_emittableOperandProviderMock, method));

      _emitter.AddMethod (_context, method, MethodAttributes.Abstract);

      _typeBuilderMock.VerifyAllExpectations();
      methodBuilderMock.VerifyAllExpectations();
      var actions = _context.PostDeclarationsActionManager.Actions.ToArray();
      Assert.That (actions, Has.Length.EqualTo (1));

      // Executing the action has no side effect (strict mocks; empty override build action).
      _emittableOperandProviderMock.Stub (mock => mock.GetEmittableMethod (method));
      actions[0].Invoke();
    }

    private void SetupDefineCustomAttribute (ICustomAttributeTargetBuilder customAttributeTargetBuilderMock, IMutableInfo mutableInfo)
    {
      var declaration = CustomAttributeDeclarationObjectMother.Create();
      mutableInfo.AddCustomAttribute (declaration);
      customAttributeTargetBuilderMock.Expect (mock => mock.SetCustomAttribute (declaration));
    }

    private void CheckBodyBuildAction (Action testedAction, IMethodBaseBuilder methodBuilderMock, IMutableMethodBase mutableMethodBase)
    {
      methodBuilderMock.BackToRecord();
      _expressionPreparerMock.Expect (mock => mock.PrepareBody (_context, mutableMethodBase.Body)).Return (_fakeBody);
      methodBuilderMock
          .Expect (mock => mock.SetBody (Arg<LambdaExpression>.Is.Anything, Arg.Is (_ilGeneratorFactoryStub), Arg.Is (_context.DebugInfoGenerator)))
          .WhenCalled (
              mi =>
              {
                var lambdaExpression = (LambdaExpression) mi.Arguments[0];
                Assert.That (lambdaExpression.Body, Is.SameAs (_fakeBody));
                Assert.That (lambdaExpression.Parameters, Is.EqualTo (mutableMethodBase.ParameterExpressions));
              });
      methodBuilderMock.Replay();

      testedAction();

      _emittableOperandProviderMock.VerifyAllExpectations();
      methodBuilderMock.VerifyAllExpectations ();
    }

    private void CheckExplicitOverrideAction (Action testedAction, MethodInfo overriddenMethod, MutableMethodInfo overridingMethod)
    {
      _emittableOperandProviderMock.BackToRecord();
      _typeBuilderMock.BackToRecord();

      var fakeOverriddenMethod = ReflectionObjectMother.GetSomeMethod();
      var fakeOverridingMethod = ReflectionObjectMother.GetSomeMethod();
      _emittableOperandProviderMock.Expect (mock => mock.GetEmittableMethod (overriddenMethod)).Return (fakeOverriddenMethod);
      _emittableOperandProviderMock.Expect (mock => mock.GetEmittableMethod (overridingMethod)).Return (fakeOverridingMethod);

      _typeBuilderMock.Expect (mock => mock.DefineMethodOverride (fakeOverridingMethod, fakeOverriddenMethod));

      _emittableOperandProviderMock.Replay ();
      _typeBuilderMock.Replay ();

      testedAction ();

      _emittableOperandProviderMock.VerifyAllExpectations ();
      _typeBuilderMock.VerifyAllExpectations ();
    }

    class DomainType
    {
      public virtual string ExplicitBaseDefinition (int i, out double d) { Dev.Null = i; d = Dev<double>.Null; return ""; }
    }
  }
}