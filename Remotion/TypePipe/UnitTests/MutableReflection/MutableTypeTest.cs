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
    private IEqualityComparer<MemberInfo> _memberInfoEqualityComparerStub;
    private IBindingFlagsEvaluator _bindingFlagsEvaluatorMock;

    private MutableType _mutableType;
    
    [SetUp]
    public void SetUp ()
    {
      _descriptor = UnderlyingTypeDescriptorObjectMother.Create(originalType: typeof (DomainClass));
      _memberInfoEqualityComparerStub = MockRepository.GenerateStub<IEqualityComparer<MemberInfo>>();
      _bindingFlagsEvaluatorMock = MockRepository.GenerateMock<IBindingFlagsEvaluator>();

      _mutableType = new MutableType (_descriptor, _memberInfoEqualityComparerStub, _bindingFlagsEvaluatorMock);
    }

    [Test]
    public void Initialization ()
    {
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
    }

    [Test]
    public void Initialization_Fields ()
    {
      Assert.That (_descriptor.Fields, Is.Not.Empty);

      Assert.That (_mutableType.ExistingFields, Is.EqualTo (_descriptor.Fields));
    }

    [Test]
    public void Initialization_Constructors ()
    {
      var ctors = _descriptor.Constructors;
      Assert.That (ctors, Is.Not.Empty.And.Count.EqualTo (1));
      var expectedCtor = ctors.Single ();

      Assert.That (_mutableType.ExistingConstructors, Has.Count.EqualTo (1));
      var mutableCtor = _mutableType.ExistingConstructors.Single();

      Assert.That (mutableCtor.UnderlyingSystemConstructorInfo, Is.EqualTo (expectedCtor));
      Assert.That (mutableCtor.DeclaringType, Is.SameAs (_mutableType));
    }

    [Test]
    public void Initialization_Methods ()
    {
      var methods = _descriptor.Methods;
      Assert.That (methods, Is.Not.Empty); // ToString(), Equals(), ...
      var expectedMethod = methods.Single (m => m.Name == "PublicMethod");

      Assert.That (_mutableType.ExistingMethods.Count, Is.EqualTo(methods.Count));
      var mutableMethod = _mutableType.ExistingMethods.Single (m => m.Name == "PublicMethod");

      Assert.That (mutableMethod.UnderlyingSystemMethodInfo, Is.EqualTo (expectedMethod));
      Assert.That (mutableMethod.DeclaringType, Is.SameAs (_mutableType));
    }

    [Test]
    public void AllInterfaces ()
    {
      Assert.That (_descriptor.Interfaces, Has.Count.EqualTo (1));
      var existingInterface = _descriptor.Interfaces.Single();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      _mutableType.AddInterface (addedInterface);

      var allInterfaces = _mutableType.AllInterfaces;

      Assert.That (allInterfaces, Is.EqualTo (new[] { existingInterface, addedInterface }));
    }

    [Test]
    public void AllFields ()
    {
      Assert.That (_descriptor.Fields, Has.Count.EqualTo (2));
      var existingFields = _descriptor.Fields;
      var addedField = _mutableType.AddField (ReflectionObjectMother.GetSomeType(), "_addedField");

      var allFields = _mutableType.AllFields;

      Assert.That (allFields, Is.EqualTo (new[] { existingFields[0], existingFields[1], addedField }));
    }

    [Test]
    public void AllConstructors ()
    {
      Assert.That (_descriptor.Constructors, Has.Count.EqualTo (1));
      var existingCtor = _descriptor.Constructors.Single ();
      var addedCtor = AddConstructor (_mutableType, new ArgumentTestHelper (7).ParameterDeclarations); // Need different signature

      var allConstructors = _mutableType.AllConstructors.ToArray();

      Assert.That (allConstructors, Has.Length.EqualTo (2));
      Assert.That (allConstructors[0].DeclaringType, Is.SameAs(_mutableType));
      Assert.That (allConstructors[0].UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (allConstructors[1], Is.SameAs (addedCtor));
    }

    [Test]
    public void AllMethods ()
    {
      Assert.That (_descriptor.Methods, Is.Not.Empty);
      var existingMethods = _descriptor.Methods;
      var addedMethod = AddMethod (_mutableType, "NewMethod");

      var allMethods = _mutableType.AllMethods.ToArray();

      var expectedMethodCount = _descriptor.Methods.Count + 1;
      Assert.That (allMethods, Has.Length.EqualTo (expectedMethodCount));
      for (int i = 0; i < expectedMethodCount - 1; i++)
      {
        Assert.That (allMethods[i].DeclaringType, Is.SameAs (_mutableType));
        Assert.That (allMethods[i].UnderlyingSystemMethodInfo, Is.SameAs (existingMethods[i]));
      }
      Assert.That (allMethods.Last (), Is.SameAs (addedMethod));
    }

    [Test]
    public void UnderlyingSystemType ()
    {
      Assert.That (_descriptor.UnderlyingSystemType, Is.Not.Null);

      Assert.That (_mutableType.UnderlyingSystemType, Is.SameAs (_descriptor.UnderlyingSystemType));
    }

    [Test]
    public void IsNewType ()
    {
      Assert.That (_mutableType.IsNewType, Is.False);
    }

    [Test]
    public void Assembly ()
    {
      Assert.That (_mutableType.Assembly, Is.Null);
    }

    [Test]
    public void Module ()
    {
      Assert.That (_mutableType.Module, Is.Null);
    }

    [Test]
    public void BaseType ()
    {
      Assert.That (_descriptor.BaseType, Is.Not.Null);

      Assert.That (_mutableType.BaseType, Is.SameAs (_descriptor.BaseType));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_descriptor.Name, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.Name, Is.EqualTo (_descriptor.Name));
    }

    [Test]
    public void Namespace ()
    {
      Assert.That (_descriptor.Namespace, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.Namespace, Is.EqualTo (_descriptor.Namespace));
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_descriptor.FullName, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.FullName, Is.EqualTo (_descriptor.FullName));
    }

    [Test]
    public new void ToString ()
    {
      Assert.That (_descriptor.StringRepresentation, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.ToString(), Is.EqualTo (_descriptor.StringRepresentation));
    }

    [Test]
    public void ToDebugString ()
    {
      Assert.That (_descriptor.StringRepresentation, Is.Not.Null.And.Not.Empty);

      Assert.That (_mutableType.ToDebugString(), Is.EqualTo ("MutableType = \"" + _descriptor.Name + "\""));
    }

    [Test]
    public void IsEquivalentTo_Type_False ()
    {
      var type = ReflectionObjectMother.GetSomeDifferentType();

      Assert.That (_mutableType.IsEquivalentTo (type), Is.False);
    }

    [Test]
    public void IsEquivalentTo_MutableType_True ()
    {
      var mutableType = _mutableType;

      Assert.That (_mutableType.IsEquivalentTo (mutableType), Is.True);
    }

    [Test]
    public void IsEquivalentTo_MutableType_False ()
    {
      var mutableType = MutableTypeObjectMother.Create();

      Assert.That (_mutableType.IsEquivalentTo(mutableType), Is.False);
    }

    [Test]
    public void AddInterface ()
    {
      var interface1 = ReflectionObjectMother.GetSomeInterfaceType();
      var interface2 = ReflectionObjectMother.GetSomeDifferentInterfaceType();

      _mutableType.AddInterface (interface1);
      _mutableType.AddInterface (interface2);

      Assert.That (_mutableType.AddedInterfaces, Is.EqualTo (new[] { interface1, interface2 }));
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
    public void GetInterfaces ()
    {
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType();
      _mutableType.AddInterface (addedInterface);

      Assert.That (_mutableType.GetInterfaces(), Is.EqualTo (_mutableType.AllInterfaces));
    }

    [Test]
    public void GetInterface_NoMatch ()
    {
      Assert.That (_mutableType.AllInterfaces.Count (), Is.EqualTo (1));

      var result = _mutableType.GetInterface ("IMyInterface", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive_NoMatch ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.AllInterfaces.Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("Imyinterface", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.AllInterfaces.Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("IMyInterface", false);

      Assert.That (result, Is.SameAs (typeof (IMyInterface)));
    }

    [Test]
    public void GetInterface_IgnoreCase ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      Assert.That (_mutableType.AllInterfaces.Count (), Is.EqualTo (2));

      var result = _mutableType.GetInterface ("Imyinterface", true);

      Assert.That (result, Is.SameAs (typeof (IMyInterface)));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Ambiguous interface name 'Imyinterface'.")]
    public void GetInterface_IgnoreCase_Ambiguous ()
    {
      _mutableType.AddInterface (typeof (IMyInterface));
      _mutableType.AddInterface (typeof (Imyinterface));
      Assert.That (_mutableType.AllInterfaces.Count (), Is.EqualTo (3));

      _mutableType.GetInterface ("Imyinterface", true);
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
      var field = _descriptor.Fields.First();
      _memberInfoEqualityComparerStub
          .Stub (stub => stub.Equals (Arg<FieldInfo>.Is.Anything, Arg<FieldInfo>.Is.Anything))
          .Return (true);

      _mutableType.AddField (field.FieldType, field.Name, FieldAttributes.Private);
    }

    [Test]
    public void AddField_ReliesOnFieldSignature ()
    {
      var field = _descriptor.Fields.First ();
      _memberInfoEqualityComparerStub
          .Stub (stub => stub.Equals (Arg<FieldInfo>.Is.Anything, Arg<FieldInfo>.Is.Anything))
          .Return (false);

      _mutableType.AddField (field.FieldType, field.Name, FieldAttributes.Private);

      Assert.That (_mutableType.AddedFields, Has.Count.EqualTo (1));
    }

    [Test]
    public void GetFields ()
    {
      Assert.That (_mutableType.AllFields, Is.Not.Empty);
      _bindingFlagsEvaluatorMock
        .Stub (stub => stub.HasRightAttributes (Arg<FieldAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
        .Return (true);

      var fields = _mutableType.GetFields (0);

      Assert.That (fields, Is.EqualTo(_mutableType.AllFields));
    }

    [Test]
    public void GetFields_FilterAddedWithUtility ()
    {
      var allFields = _mutableType.AllFields.ToArray();
      Assert.That (allFields, Has.Length.EqualTo(2));
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (allFields[0].Attributes, bindingFlags)).Return (false).Repeat.Once();
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (allFields[1].Attributes, bindingFlags)).Return (false).Repeat.Once();

      var fields = _mutableType.GetFields (bindingFlags);

      _bindingFlagsEvaluatorMock.VerifyAllExpectations ();
      Assert.That (fields, Is.Empty);
    }

    [Test]
    public void GetField ()
    {
      Assert.That (_descriptor.Fields, Has.Count.GreaterThan (1));
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<FieldAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      var field = _descriptor.Fields.Last();

      var resultField = _mutableType.GetField (field.Name, BindingFlags.NonPublic | BindingFlags.Instance);

      Assert.That (resultField, Is.SameAs (field));
    }

    [Test]
    public void GetField_NoMatch ()
    {
      Assert.That (_mutableType.GetField ("field"), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Ambiguous field name 'Field1'.")]
    public void GetField_Ambigious ()
    {
      var fieldName = "Field1";
      _mutableType.AddField (typeof (string), fieldName, 0);
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<FieldAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      Assert.That (_mutableType.GetFields().Where (f => f.Name == fieldName).ToArray(), Has.Length.GreaterThan (1));

      _mutableType.GetField (fieldName, 0);
    }

    [Test]
    public void AddConstructor ()
    {
      var attributes = MethodAttributes.Public;
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.Parameters, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
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
      Assert.That (_mutableType.ExistingConstructors, Has.Count.EqualTo (1));
      Assert.That (_mutableType.ExistingConstructors.Single ().GetParameters (), Is.Empty);
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      _memberInfoEqualityComparerStub.Stub (stub => stub.Equals (Arg<MemberInfo>.Is.Anything, Arg<MemberInfo>.Is.Anything)).Return (true);

      _mutableType.AddConstructor (0, ParameterDeclaration.EmptyParameters, context => Expression.Empty());
    }

    [Test]
    public void GetConstructors ()
    {
      Assert.That (_mutableType.AllConstructors, Is.Not.Empty);
      _bindingFlagsEvaluatorMock
          .Stub (mock => mock.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);

      var constructors = _mutableType.GetConstructors (0);

      Assert.That (constructors, Is.EqualTo(_mutableType.AllConstructors));
    }

    [Test]
    public void GetConstructors_FilterWithUtility ()
    {
      Assert.That (_mutableType.AllConstructors.Count(), Is.EqualTo(1));
      var ctor = _mutableType.AllConstructors.Single();
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (ctor.Attributes, bindingFlags)).Return (false);

      var constructors = _mutableType.GetConstructors (bindingFlags);

      _bindingFlagsEvaluatorMock.VerifyAllExpectations ();
      Assert.That (constructors, Is.Empty);
    }

    [Test]
    public void GetMutableConstructor ()
    {
      var existingCtor = _descriptor.Constructors.Single ();
      Assert.That (existingCtor, Is.Not.AssignableTo<MutableConstructorInfo>());

      var result = _mutableType.GetMutableConstructor (existingCtor);

      Assert.That (result.UnderlyingSystemConstructorInfo, Is.SameAs (existingCtor));
      Assert.That (_mutableType.ExistingConstructors, Has.Member (result));
    }

    [Test]
    public void AddMethod ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Public;
      var returnType = typeof (object);
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (int));
      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.Parameters, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
        Assert.That (context.IsStatic, Is.False);
        Assert.That (context.This.Type, Is.SameAs (_mutableType));

        return fakeBody;
      };

      var method = _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations.AsOneTime(), bodyProvider);

      // Correct method info instance
      Assert.That (method.DeclaringType, Is.SameAs (_mutableType));
      Assert.That (method.UnderlyingSystemMethodInfo, Is.SameAs (method));
      Assert.That (method.Name, Is.EqualTo (name));
      Assert.That (method.ReturnType, Is.EqualTo (returnType));
      Assert.That (method.Attributes, Is.EqualTo (attributes));
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
      Func<MethodBodyCreationContext, Expression> bodyProvider = context =>
      {
        Assert.That (context.IsStatic, Is.True);
        return fakeBody;
      };

      _mutableType.AddMethod (name, attributes, returnType, parameterDeclarations, bodyProvider);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Method 'PublicMethod' with equal signature already exists.\r\nParameter name: name")]
    public void AddMethod_ThrowsIfAlreadyExists ()
    {
      var method = _mutableType.ExistingMethods.Single (m => m.Name == "PublicMethod");
      Assert.That (method, Is.Not.Null);
      Assert.That (method.ReturnType, Is.SameAs(typeof(void)));
      Assert.That (method.GetParameters(), Is.Empty);
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      _memberInfoEqualityComparerStub.Stub (stub => stub.Equals (Arg<MemberInfo>.Is.Anything, Arg<MemberInfo>.Is.Anything)).Return (true);

      _mutableType.AddMethod ("PublicMethod", 0, typeof (void), ParameterDeclaration.EmptyParameters, cx => Expression.Empty());
    }

    [Test]
    public void AddMethod_AllowsShadowing ()
    {
      var toStringMethod = _mutableType.ExistingMethods.Single (m => m.Name == "ToString");
      Assert.That (toStringMethod, Is.Not.Null);
      Assert.That (toStringMethod.GetParameters (), Is.Empty);
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      _memberInfoEqualityComparerStub.Stub (stub => stub.Equals (Arg<MemberInfo>.Is.Anything, Arg<MemberInfo>.Is.Anything)).Return (true);

      Assert.That (
          () => _mutableType.AddMethod ("ToString", 0, typeof (string), ParameterDeclaration.EmptyParameters, ctx => Expression.Constant ("string")),
          Throws.Nothing);
    }

    [Test]
    public void GetMethods ()
    {
      AddMethod (_mutableType, "Method");

      Assert.That (_mutableType.ExistingMethods, Is.Not.Empty);
      Assert.That (_mutableType.AddedMethods, Is.Not.Empty);

      _bindingFlagsEvaluatorMock
          .Stub (mock => mock.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);

      var methods = _mutableType.GetMethods (0);

      Assert.That (methods, Is.EqualTo (_mutableType.AllMethods));
    }

    [Test]
    public void GetMethods_FilterWithUtility ()
    {
      Assert.That (_mutableType.AllMethods, Is.Not.Empty);
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      _bindingFlagsEvaluatorMock.Expect (mock => mock.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg.Is(bindingFlags))).Return (false);

      var methods = _mutableType.GetMethods (bindingFlags);

      _bindingFlagsEvaluatorMock.VerifyAllExpectations ();
      Assert.That (methods, Is.Empty);
    }

    [Test]
    public void GetMutableMethod ()
    {
      var existingMethod = _descriptor.Methods.Single (m => m.Name == "PublicMethod");
      Assert.That (existingMethod, Is.Not.AssignableTo<MutableMethodInfo> ());

      var result = _mutableType.GetMutableMethod (existingMethod);

      Assert.That (result.UnderlyingSystemMethodInfo, Is.SameAs (existingMethod));
      Assert.That (_mutableType.ExistingMethods, Has.Member (result));
    }

    [Test]
    public void Accept_WithAddedAndUnmodifiedExistingMembers ()
    {
      Assert.That (_mutableType.ExistingInterfaces, Is.Not.Empty);
      var addedInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType ();
      _mutableType.AddInterface (addedInterface);

      Assert.That (_mutableType.ExistingFields, Is.Not.Empty);
      var addedFieldInfo = _mutableType.AddField (ReflectionObjectMother.GetSomeType (), "name", FieldAttributes.Private);

      Assert.That (_mutableType.ExistingConstructors, Is.Not.Empty);
      var addedConstructorInfo = AddConstructor (_mutableType);

      Assert.That (_mutableType.ExistingMethods, Is.Not.Empty);
      var addedMethod = AddMethod (_mutableType, "AddedMethod");

      var handlerMock = MockRepository.GenerateStrictMock<ITypeModificationHandler>();
      handlerMock.Expect (mock => mock.HandleAddedInterface (addedInterface));
      handlerMock.Expect (mock => mock.HandleAddedField (addedFieldInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (addedConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (addedMethod));
      
      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_WithModifiedConstructors ()
    {
      Assert.That (_mutableType.ExistingConstructors, Is.Not.Empty);
      var modifiedExistingConstructorInfo = _mutableType.ExistingConstructors.First();
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedExistingConstructorInfo);

      var modifiedAddedConstructorInfo = AddConstructor (_mutableType);
      MutableConstructorInfoTestHelper.ModifyConstructor (modifiedAddedConstructorInfo);

      var handlerMock = MockRepository.GenerateStrictMock<ITypeModificationHandler> ();
      handlerMock.Expect (mock => mock.HandleModifiedConstructor (modifiedExistingConstructorInfo));
      handlerMock.Expect (mock => mock.HandleAddedConstructor (modifiedAddedConstructorInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_WitModifiedMethod ()
    {
      Assert.That (_mutableType.ExistingMethods, Is.Not.Empty);
      var modifiedExistingMethodInfo = _mutableType.ExistingMethods.Single (m => m.Name == "VirtualMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedExistingMethodInfo);

      var modifiedAddedMethodInfo = AddMethod (_mutableType, "ModifiedAddedMethod");
      MutableMethodInfoTestHelper.ModifyMethod (modifiedAddedMethodInfo);

      var handlerMock = MockRepository.GenerateStrictMock<ITypeModificationHandler> ();
      handlerMock.Expect (mock => mock.HandleModifiedMethod (modifiedExistingMethodInfo));
      handlerMock.Expect (mock => mock.HandleAddedMethod (modifiedAddedMethodInfo));

      _mutableType.Accept (handlerMock);

      handlerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetElementType ()
    {
      Assert.That (_mutableType.GetElementType(), Is.Null);
    }

    [Test]
    public void HasElementTypeImpl ()
    {
      Assert.That (_mutableType.HasElementType, Is.False);
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      Assert.That (_mutableType.Attributes, Is.EqualTo (_descriptor.Attributes));
    }

    [Test]
    public void IsByRefImpl ()
    {
      Assert.That (_mutableType.IsByRef, Is.False);
    }

    [Test]
    public void IsArrayImpl ()
    {
      Assert.That (_mutableType.IsArray, Is.False);
    }

    [Test]
    public void IsPointerImpl ()
    {
      Assert.That (_mutableType.IsPointer, Is.False);
    }

    [Test]
    public void IsPrimitiveImpl ()
    {
      Assert.That (_mutableType.IsPrimitive, Is.False);
    }

    [Test]
    public void IsCOMObjectImpl ()
    {
      Assert.That (_mutableType.IsCOMObject, Is.False);
    }

    [Test]
    public void GetConstructorImpl ()
    {
      var arguments = new ArgumentTestHelper (typeof (int));
      var addedConstructor = _mutableType.AddConstructor(0, arguments.ParameterDeclarations, ctx => Expression.Empty());
      
      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      Assert.That (_mutableType.GetConstructors (), Has.Length.GreaterThan (1));

      var resultCtor = _mutableType.GetConstructor (arguments.Types);
      Assert.That (resultCtor, Is.SameAs (addedConstructor));
    }

    [Test]
    public void GetConstructorImpl_NoMatch ()
    {
      var arguments = new ArgumentTestHelper (typeof (int));
      Assert.That (_mutableType.GetConstructor (arguments.Types), Is.Null);
    }

    [Test]
    public void GetMethodImpl ()
    {
      var addedMethod1 = AddMethod (_mutableType, "AddedMethod");
      var addedMethod2 = AddMethod (_mutableType, "AddedMethod", new ParameterDeclaration(typeof(int), "i"));

      _bindingFlagsEvaluatorMock
          .Stub (stub => stub.HasRightAttributes (Arg<MethodAttributes>.Is.Anything, Arg<BindingFlags>.Is.Anything))
          .Return (true);
      Assert.That (_mutableType.GetMethods ().Select (m => m.Name == "AddedMethod").Count(), Is.GreaterThan (1));

      var result1 = _mutableType.GetMethod ("AddedMethod", Type.EmptyTypes);
      var result2 = _mutableType.GetMethod ("AddedMethod", new[] { typeof (int) });
      Assert.That (result1, Is.SameAs (addedMethod1));
      Assert.That (result2, Is.SameAs (addedMethod2));
    }

    [Test]
    public void GetMethodImpl_NoMatch ()
    {
      Assert.That (_mutableType.GetMethod ("DoesNotExist"), Is.Null);
    }

    [Test]
    public void VirtualMethodsImplementedByType ()
    {
      // None of these members should throw an exception 
      Dev.Null = _mutableType.MemberType;
      Dev.Null = _mutableType.DeclaringType;
      Dev.Null = _mutableType.DeclaringMethod;
      Dev.Null = _mutableType.ReflectedType;
      Dev.Null = _mutableType.IsGenericType;
      Dev.Null = _mutableType.IsGenericTypeDefinition;
      Dev.Null = _mutableType.IsGenericParameter;
      Dev.Null = _mutableType.ContainsGenericParameters;

      Dev.Null = _mutableType.IsValueType; // IsValueTypeImpl()
      Dev.Null = _mutableType.IsContextful; // IsContextfulImpl()
      Dev.Null = _mutableType.IsMarshalByRef; // IsMarshalByRefImpl()

      _mutableType.FindInterfaces ((type, filterCriteria) => true, filterCriteria: null);
      _mutableType.GetEvents ();
      _mutableType.GetMember ("name", BindingFlags.Default);
      _mutableType.GetMember ("name", MemberTypes.All, BindingFlags.Default);
      _mutableType.FindMembers (MemberTypes.All, BindingFlags.Default, filter: null, filterCriteria: null);
      _mutableType.IsSubclassOf (null);
      _mutableType.IsInstanceOfType (null);
      _mutableType.IsAssignableFrom (null);
    }

    [Test]
    public void UnsupportedMembers ()
    {
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.MetadataToken, "Property", "MetadataToken");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GUID, "Property", "GUID");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.AssemblyQualifiedName, "Property", "AssemblyQualifiedName");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.StructLayoutAttribute, "Property", "StructLayoutAttribute");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GenericParameterAttributes, "Property", "GenericParameterAttributes");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.GenericParameterPosition, "Property", "GenericParameterPosition");
      CheckThrowsNotSupported (() => Dev.Null = _mutableType.TypeHandle, "Property", "TypeHandle");

      CheckThrowsNotSupported (() => _mutableType.GetDefaultMembers (), "Method", "GetDefaultMembers");
      CheckThrowsNotSupported (() => _mutableType.GetInterfaceMap (null), "Method", "GetInterfaceMap");
      CheckThrowsNotSupported (() => _mutableType.InvokeMember (null, 0, null, null, null), "Method", "InvokeMember");
      CheckThrowsNotSupported (() => _mutableType.MakePointerType(), "Method", "MakePointerType");
      CheckThrowsNotSupported (() => _mutableType.MakeByRefType(), "Method", "MakeByRefType");
      CheckThrowsNotSupported (() => _mutableType.MakeArrayType(), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _mutableType.MakeArrayType (7), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _mutableType.GetArrayRank(), "Method", "GetArrayRank");
      CheckThrowsNotSupported (() => _mutableType.GetGenericParameterConstraints(), "Method", "GetGenericParameterConstraints");
      CheckThrowsNotSupported (() => _mutableType.MakeGenericType(), "Method", "MakeGenericType");
      CheckThrowsNotSupported (() => _mutableType.GetGenericArguments(), "Method", "GetGenericArguments");
      CheckThrowsNotSupported (() => _mutableType.GetGenericTypeDefinition (), "Method", "GetGenericTypeDefinition");
    }

    private void CheckThrowsNotSupported(TestDelegate memberInvocation, string memberType, string memberName)
    {
      var message = string.Format ("{0} MutableType.{1} is not supported.", memberType, memberName);
      Assert.That (memberInvocation, Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
    }

    private MutableConstructorInfo AddConstructor (MutableType mutableType, params ParameterDeclaration[] parameterDeclarations)
    {
      return mutableType.AddConstructor (MethodAttributes.Public, parameterDeclarations.AsOneTime(), context => Expression.Empty());
    }

    private MutableMethodInfo AddMethod (MutableType mutableType, string name, params ParameterDeclaration[] parameterDeclarations)
    {
      var returnType = ReflectionObjectMother.GetSomeType();
      var body = ExpressionTreeObjectMother.GetSomeExpression (returnType);

      return mutableType.AddMethod (name, MethodAttributes.Public, returnType, parameterDeclarations.AsOneTime(), ctx => body);
    }

    public class DomainClass : IDomainInterface
    {
      protected int Field1 = 1;
      protected int Field2 = 2;

      public DomainClass ()
      {
        Dev.Null = Field1;
        Dev.Null = Field2;
      }

      public void PublicMethod () { }

      public virtual void VirtualMethod () { }
    }

    public interface IDomainInterface
    {
    }

    interface IMyInterface { }
    interface Imyinterface { }
  }
}