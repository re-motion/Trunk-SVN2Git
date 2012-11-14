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
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class SubclassProxyBuilderTest
  {
    private MutableType _mutableType;
    private ITypeBuilder _typeBuilderMock;
    private DebugInfoGenerator _debugInfoGeneratorStub;
    private IEmittableOperandProvider _emittableOperandProviderMock;
    private IMethodTrampolineProvider _methodTrampolineProvider;
    private IMemberEmitter _memberEmitterMock;

    private SubclassProxyBuilder _builder;

    private MemberEmitterContext _context;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = MutableTypeObjectMother.CreateForExisting ();
      _typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      _debugInfoGeneratorStub = MockRepository.GenerateStub<DebugInfoGenerator>();
      _emittableOperandProviderMock = MockRepository.GenerateStrictMock<IEmittableOperandProvider>();
      _methodTrampolineProvider = MockRepository.GenerateStrictMock<IMethodTrampolineProvider>();
      _memberEmitterMock = MockRepository.GenerateStrictMock<IMemberEmitter>();

      _builder = new SubclassProxyBuilder (_mutableType, _typeBuilderMock, _debugInfoGeneratorStub, _emittableOperandProviderMock, _methodTrampolineProvider, _memberEmitterMock);

      _context = _builder.MemberEmitterContext;
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_builder.MemberEmitter, Is.SameAs (_memberEmitterMock));

      Assert.That (_context.MutableType, Is.SameAs (_mutableType));
      Assert.That (_context.TypeBuilder, Is.SameAs (_typeBuilderMock));
      Assert.That (_context.DebugInfoGenerator, Is.SameAs (_debugInfoGeneratorStub));
      Assert.That (_context.EmittableOperandProvider, Is.SameAs (_emittableOperandProviderMock));
      Assert.That (_context.MethodTrampolineProvider, Is.SameAs (_methodTrampolineProvider));
      Assert.That (_context.PostDeclarationsActionManager.Actions, Is.Empty);
    }

    [Test]
    public void Initialization_NullDebugInfoGenerator ()
    {
      var handler = new SubclassProxyBuilder (_mutableType, _typeBuilderMock, null, _emittableOperandProviderMock, _methodTrampolineProvider, _memberEmitterMock);
      Assert.That (handler.MemberEmitterContext.DebugInfoGenerator, Is.Null);
    }

    [Test]
    public void HandleTypeInitializations ()
    {
      var expressions = new[] { ExpressionTreeObjectMother.GetSomeExpression() }.ToList().AsReadOnly();

      _memberEmitterMock.Expect (mock => mock.AddConstructor (Arg.Is (_context), Arg<MutableConstructorInfo>.Is.Anything))
          .WhenCalled (
              mi =>
              {
                var ctor = (MutableConstructorInfo) mi.Arguments[1];
                Assert.That (ctor.DeclaringType, Is.SameAs (_mutableType));
                Assert.That (ctor.Name, Is.EqualTo (".cctor"));
                Assert.That (ctor.Attributes, Is.EqualTo (MethodAttributes.Private | MethodAttributes.Static));
                Assert.That (ctor.GetParameters(), Is.Empty);
                Assert.That (ctor.Body, Is.InstanceOf<BlockExpression>());

                var blockExpression = (BlockExpression) ctor.Body;
                Assert.That (blockExpression.Type, Is.EqualTo (typeof (void)));
                Assert.That (blockExpression.Expressions, Is.EqualTo (expressions));
              });

      _builder.HandleTypeInitializations (expressions);

      _memberEmitterMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleTypeInitializations_Empty ()
    {
      var expressions = new Expression[0].ToList().AsReadOnly();

      _builder.HandleTypeInitializations (expressions);

      _memberEmitterMock.AssertWasNotCalled (
          mock => mock.AddConstructor (Arg<MemberEmitterContext>.Is.Anything, Arg<MutableConstructorInfo>.Is.Anything));
    }

    [Test]
    public void HandleInstanceInitializations ()
    {
      var expressions = new[] { ExpressionTreeObjectMother.GetSomeExpression() }.ToList().AsReadOnly();
      var methodAttributes = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig;

      _typeBuilderMock.Expect (mock => mock.AddInterfaceImplementation (typeof (IInitializableObject)));
      _memberEmitterMock
          .Expect (mock => mock.AddMethod (Arg.Is (_context), Arg<MutableMethodInfo>.Is.Anything, Arg.Is (methodAttributes)))
          .WhenCalled (
              mi =>
              {
                var method = (MutableMethodInfo) mi.Arguments[1];
                Assert.That (method.DeclaringType, Is.SameAs (_mutableType));
                Assert.That (method.Name, Is.EqualTo ("Remotion.TypePipe.Caching.IInitializableObject_Initialize"));
                Assert.That (method.Attributes, Is.EqualTo (methodAttributes));
                Assert.That (method.ReturnType, Is.SameAs (typeof (void)));
                Assert.That (method.GetParameters(), Is.Empty);
                Assert.That (method.Body, Is.InstanceOf<BlockExpression>());

                var blockExpression = (BlockExpression) method.Body;
                Assert.That (blockExpression.Type, Is.EqualTo (typeof (void)));
                Assert.That (blockExpression.Expressions, Is.EqualTo (expressions));
              });

      _builder.HandleInstanceInitializations (expressions);

      _typeBuilderMock.VerifyAllExpectations();
      _memberEmitterMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleInstanceInitializations_Empty ()
    {
      var expressions = new Expression[0].ToList().AsReadOnly();

      _builder.HandleInstanceInitializations (expressions);

      _typeBuilderMock.AssertWasNotCalled (mock => mock.AddInterfaceImplementation (Arg<Type>.Is.Anything));
      _memberEmitterMock.AssertWasNotCalled (
          mock => mock.AddMethod (Arg<MemberEmitterContext>.Is.Anything, Arg<MutableMethodInfo>.Is.Anything, Arg<MethodAttributes>.Is.Anything));
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
      _memberEmitterMock.Expect (mock => mock.AddField (_context, addedField));
      
      _builder.HandleAddedField (addedField);

      _memberEmitterMock.VerifyAllExpectations ();
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
      var addedCtor = MutableConstructorInfoObjectMother.CreateForNew ();
      _memberEmitterMock.Expect (mock => mock.AddConstructor (_context, addedCtor));

      _builder.HandleAddedConstructor (addedCtor);

      _memberEmitterMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleAddedConstructor_Throws ()
    {
      var message = "The supplied constructor must be a new constructor.\r\nParameter name: constructor";
      CheckThrowsForInvalidArguments (_builder.HandleAddedConstructor, message, isNew: false, isModified: true);
      CheckThrowsForInvalidArguments (_builder.HandleAddedConstructor, message, isNew: false, isModified: false);
    }

    [Test]
    public void HandleAddedMethod ()
    {
      var addedMethod = MutableMethodInfoObjectMother.CreateForNew ();
      _memberEmitterMock.Expect (mock => mock.AddMethod (_context, addedMethod, addedMethod.Attributes));

      _builder.HandleAddedMethod (addedMethod);

      _memberEmitterMock.VerifyAllExpectations ();
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
      var originalCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7, out Dev<double>.Dummy));
      var modifiedCtor = MutableConstructorInfoObjectMother.CreateForExistingAndModify (originalCtor);
      _memberEmitterMock.Expect (mock => mock.AddConstructor (_context, modifiedCtor));

      _builder.HandleModifiedConstructor (modifiedCtor);

      _memberEmitterMock.VerifyAllExpectations();
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
    public void HandleModifiedMethod ()
    {
      var originalMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType dt) => dt.Method (7, out Dev<double>.Dummy));
      var modifiedMethod = MutableMethodInfoObjectMother.CreateForExistingAndModify (underlyingMethod: originalMethod);

      var expectedAttributes = MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig;
      _memberEmitterMock.Expect (mock => mock.AddMethod (_context, modifiedMethod, expectedAttributes));

      _builder.HandleModifiedMethod (modifiedMethod);

      _memberEmitterMock.VerifyAllExpectations ();
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
      _emittableOperandProviderMock.Expect (mock => mock.AddMapping (field, field.UnderlyingSystemFieldInfo));

      _builder.HandleUnmodifiedField (field);

      _emittableOperandProviderMock.VerifyAllExpectations();
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
      var originalCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7, out Dev<double>.Dummy));
      var unmodifiedCtor = MutableConstructorInfoObjectMother.CreateForExisting (originalCtor);
      _memberEmitterMock.Expect (mock => mock.AddConstructor (_context, unmodifiedCtor));

      _builder.HandleUnmodifiedConstructor (unmodifiedCtor);

      _memberEmitterMock.VerifyAllExpectations();
    }

    [Test]
    public void HandleUnmodifiedConstructor_IgnoresCtorsThatAreNotVisibleFromSubclass ()
    {
      var internalCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType ());
      var unmodifiedCtor = MutableConstructorInfoObjectMother.CreateForExisting (internalCtor);

      _builder.HandleUnmodifiedConstructor (unmodifiedCtor);

      _memberEmitterMock.AssertWasNotCalled (
        mock => mock.AddConstructor (Arg<MemberEmitterContext>.Is.Anything, Arg<MutableConstructorInfo>.Is.Anything));
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
      var method = MutableMethodInfoObjectMother.CreateForExisting();

      _emittableOperandProviderMock.Expect (mock => mock.AddMapping (method, method.UnderlyingSystemMethodInfo));

      _builder.HandleUnmodifiedMethod (method);

      _emittableOperandProviderMock.VerifyAllExpectations();
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
      _context.PostDeclarationsActionManager.AddAction (() => buildActionCalled = true);

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
    public void Build_DisablesOperations ()
    {
      _typeBuilderMock.Stub (mock => mock.CreateType ());
      _builder.Build ();

      CheckThrowsForOperationAfterBuild (() => _builder.HandleTypeInitializations (new Expression[0].ToList().AsReadOnly()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleInstanceInitializations (new Expression[0].ToList().AsReadOnly()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedInterface (ReflectionObjectMother.GetSomeInterfaceType()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedField (MutableFieldInfoObjectMother.Create ()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedConstructor (MutableConstructorInfoObjectMother.CreateForNew()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleAddedMethod (MutableMethodInfoObjectMother.CreateForNew()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleModifiedConstructor (MutableConstructorInfoObjectMother.CreateForExistingAndModify()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleModifiedMethod (MutableMethodInfoObjectMother.CreateForNew ()));

      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedField (MutableFieldInfoObjectMother.CreateForExisting()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedConstructor (MutableConstructorInfoObjectMother.CreateForExisting()));
      CheckThrowsForOperationAfterBuild (() => _builder.HandleUnmodifiedMethod (MutableMethodInfoObjectMother.CreateForExisting()));

      CheckThrowsForOperationAfterBuild (() => _builder.Build());
    }

    private void CheckThrowsForOperationAfterBuild (Action action)
    {
      Assert.That (() => action(), Throws.InvalidOperationException.With.Message.EqualTo ("Subclass proxy has already been built."));
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
                           underlyingMethod: NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString()));
      if (isModified)
        MutableMethodInfoTestHelper.ModifyMethod (method);

      CheckThrowsForInvalidArguments (testedAction, method, isNew, isModified, exceptionMessage);
    }

    private void CheckThrowsForInvalidArguments<T> (Action<T> testedAction, T mutableMember, bool isNew, bool isModified, string exceptionMessage)
        where T: IMutableMember
    {
      Assert.That (mutableMember.IsNew, Is.EqualTo (isNew));
      Assert.That (mutableMember.IsModified, Is.EqualTo (isModified));

      Assert.That (() => testedAction (mutableMember), Throws.ArgumentException.With.Message.EqualTo (exceptionMessage));
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

      internal DomainType() { }

      protected internal virtual string Method (int i, out double d)
      {
        Dev.Null = i;
        d = Dev<double>.Null;
        return "";
      }
    }
  }
}