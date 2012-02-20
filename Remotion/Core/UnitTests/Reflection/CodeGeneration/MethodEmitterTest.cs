// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.UnitTests.Reflection.CodeGeneration.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class MethodEmitterTest : MethodGenerationTestBase
  {
    [Test]
    [Obsolete ("TODO RM-4648 : MethodEmitter.ParameterTypes returns invalid results when the parameter types are copied from a MethodInfo. Fix or remove.")]
    public void SimpleMethod ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleMethod", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .SetParameterTypes (new[] { typeof (string) });
      method.ImplementByReturning (
              new MethodInvocationExpression (null, typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) }),
              method.ArgumentReferences[0].ToExpression (), new ConstReference ("Simple").ToExpression ()));

      object returnValue = BuildInstanceAndInvokeMethod(method, "Param");
      Assert.AreEqual ("ParamSimple", returnValue);

      Assert.IsNotNull (method.MethodBuilder);
      Assert.That (method.ParameterTypes, Is.EqualTo (new[] { typeof (string) }));
    }

    [Test]
    public void StaticMethod ()
    {
      var method = ClassEmitter.CreateMethod ("StaticMethod", MethodAttributes.Public | MethodAttributes.Static)
          .SetReturnType (typeof (string))
          .SetParameterTypes (new[] { typeof (string) });
      method.ImplementByReturning (
          new MethodInvocationExpression (null, typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) }),
          method.ArgumentReferences[0].ToExpression (), new ConstReference ("Simple").ToExpression ()));

      object returnValue = BuildTypeAndInvokeMethod (method, "Param");
      Assert.AreEqual ("ParamSimple", returnValue);
    }

    [Test]
    public void ILGenerator ()
    {
      var method = ClassEmitter.CreateMethod ("StaticMethod", MethodAttributes.Public | MethodAttributes.Static)
          .SetReturnType (typeof (string));
      ILGenerator gen = method.ILGenerator;
      Assert.IsNotNull (gen);
      gen.Emit (OpCodes.Ldstr, "manual retval");
      gen.Emit (OpCodes.Ret);

      object returnValue = BuildTypeAndInvokeMethod (method);
      Assert.AreEqual ("manual retval", returnValue);
    }

    [Test]
    public void GetArgumentExpressions ()
    {
      var method = ClassEmitter.CreateMethod ("StaticMethod", MethodAttributes.Public | MethodAttributes.Static)
          .SetReturnType (typeof (string))
          .SetParameterTypes (new[] { typeof (string) });
      Expression[] argumentExpressions = method.GetArgumentExpressions ();

      Assert.AreEqual (method.ArgumentReferences.Length, argumentExpressions.Length);
      for (int i = 0; i < argumentExpressions.Length; ++i)
        Assert.AreEqual (method.ArgumentReferences[i], PrivateInvoke.GetNonPublicField (argumentExpressions[i], "reference"));
    }

    [Test]
    public void CopyParametersAndReturnTypeSimple ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleClone", MethodAttributes.Public)
          .CopyParametersAndReturnType (typeof (object).GetMethod ("Equals", new[] { typeof (object) }))
          .ImplementByReturningDefault ();

      MethodInfo builtMethod = BuildTypeAndGetMethod (method);

      Assert.AreEqual (typeof (bool), builtMethod.ReturnType);
      ParameterInfo[] parameters = builtMethod.GetParameters ();
      Assert.AreEqual (1, parameters.Length);
      Assert.AreEqual (typeof (object), parameters[0].ParameterType);
    }

    [Test]
    public void CopyParametersAndReturnTypeGeneric ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleClone", MethodAttributes.Public)
          .CopyParametersAndReturnType (typeof (ClassWithConstrainedGenericMethod).GetMethod ("GenericMethod"))
          .ImplementByReturningDefault ();

      MethodInfo builtMethod = BuildTypeAndGetMethod (method);

      Assert.AreEqual (typeof (string), builtMethod.ReturnType);
      ParameterInfo[] parameters = builtMethod.GetParameters ();
      Assert.AreEqual (3, parameters.Length);
      
      Assert.IsTrue (parameters[0].ParameterType.IsGenericParameter);
      Assert.AreEqual(builtMethod, parameters[0].ParameterType.DeclaringMethod);
      Assert.AreEqual (GenericParameterAttributes.None, parameters[0].ParameterType.GenericParameterAttributes);
      Type[] constraints = parameters[0].ParameterType.GetGenericParameterConstraints();
      Assert.AreEqual (1, constraints.Length);
      Assert.AreEqual (typeof (IConvertible), constraints[0]);

      Assert.IsTrue (parameters[1].ParameterType.IsGenericParameter);
      Assert.AreEqual (builtMethod, parameters[1].ParameterType.DeclaringMethod);
      Assert.AreEqual (GenericParameterAttributes.NotNullableValueTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
          parameters[1].ParameterType.GenericParameterAttributes);
      constraints = parameters[1].ParameterType.GetGenericParameterConstraints ();
      Assert.AreEqual (1, constraints.Length);
      Assert.AreEqual (typeof (ValueType), constraints[0]);

      Assert.IsTrue (parameters[2].ParameterType.IsGenericParameter);
      Assert.AreEqual (builtMethod, parameters[2].ParameterType.DeclaringMethod);
      Assert.AreEqual (GenericParameterAttributes.None, parameters[2].ParameterType.GenericParameterAttributes);
      constraints = parameters[2].ParameterType.GetGenericParameterConstraints ();
      Assert.AreEqual (1, constraints.Length);
      Assert.AreEqual (parameters[0].ParameterType, constraints[0]);
    }

    [Test]
    public void CopyParametersAndReturnTypeOutRef ()
    {
      var method = ClassEmitter.CreateMethod ("MethodWithOutRef", MethodAttributes.Public)
          .CopyParametersAndReturnType (typeof (ClassWithAllKindsOfMembers).GetMethod ("MethodWithOutRef"));
      method.AddStatement (new AssignStatement (new IndirectReference (method.ArgumentReferences[0]), NullExpression.Instance))
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();
      MethodInfo builtMethod = GetMethod (instance, method);

      Assert.AreEqual (typeof (void), builtMethod.ReturnType);
      ParameterInfo[] parameters = builtMethod.GetParameters ();
      Assert.AreEqual (2, parameters.Length);
      Assert.AreEqual (typeof (string).MakeByRefType(), parameters[0].ParameterType);
      Assert.IsTrue (parameters[0].ParameterType.IsByRef);
      Assert.IsTrue (parameters[0].IsOut);
      Assert.IsFalse (parameters[0].IsIn);

      Assert.AreEqual (typeof (int).MakeByRefType (), parameters[1].ParameterType);
      Assert.IsTrue (parameters[1].ParameterType.IsByRef);
      Assert.IsFalse (parameters[1].IsOut);
      Assert.IsFalse (parameters[1].IsIn);

      var arguments = new object[] { "foo", 5 };
      InvokeMethod (instance, method, arguments);
      Assert.AreEqual (null, arguments[0]);
      Assert.AreEqual (5, arguments[1]);
    }

    [Test]
    public void ImplementByReturning ()
    {
      var method = ClassEmitter.CreateMethod ("MethodReturning", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturning (new ConstReference ("none").ToExpression());

      Assert.AreEqual ("none", BuildInstanceAndInvokeMethod (method));
    }

    [Test]
    public void ImplementByReturningVoid ()
    {
      var method = ClassEmitter.CreateMethod ("MethodReturningVoid", MethodAttributes.Public)
          .ImplementByReturningVoid ();

      Assert.AreEqual (null, BuildInstanceAndInvokeMethod (method));
    }

    [Test]
    public void ImplementByReturningDefaultValueType ()
    {
      var intMethod = ClassEmitter.CreateMethod ("IntMethod", MethodAttributes.Public)
          .SetReturnType (typeof (int))
          .ImplementByReturningDefault ();
      var dateTimeMethod = ClassEmitter.CreateMethod ("DateTimeMethod", MethodAttributes.Public)
          .SetReturnType (typeof (DateTime))
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.AreEqual (0, InvokeMethod (instance, intMethod));
      Assert.AreEqual (new DateTime (), InvokeMethod (instance, dateTimeMethod));
    }

    [Test]
    public void ImplementByReturningDefaultReferenceType ()
    {
      var objectMethod = ClassEmitter.CreateMethod ("ObjectMethod", MethodAttributes.Public)
          .SetReturnType (typeof (object))
          .ImplementByReturningDefault ();
      var stringMethod = ClassEmitter.CreateMethod ("StringMethod", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.AreEqual (null, InvokeMethod (instance, objectMethod));
      Assert.AreEqual (null, InvokeMethod (instance, stringMethod));
    }

    [Test]
    public void ImplementByReturningDefaultVoid ()
    {
      var voidMethod = ClassEmitter.CreateMethod ("VoidMethod", MethodAttributes.Public)
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.AreEqual (null, InvokeMethod (instance, voidMethod));
      Assert.AreEqual (typeof (void), GetMethod (instance, voidMethod).ReturnType);
    }

    [Test]
    public void ImplementByDelegating ()
    {
      var method = ClassEmitter.CreateMethod ("EqualsSelf", MethodAttributes.Public | MethodAttributes.Static)
          .SetParameterTypes (typeof (object))
          .SetReturnType (typeof (bool));
      method.ImplementByDelegating (method.ArgumentReferences[0], typeof (object).GetMethod ("Equals", new[] {typeof (object)}));

      object instance = BuildInstance ();

      Assert.AreEqual (true, InvokeMethod (instance, method, 5));
      Assert.AreEqual (true, InvokeMethod (instance, method, "five"));
    }

    [Test]
    public void ImplementByDelegatingToValueType ()
    {
      var method = ClassEmitter.CreateMethod ("IntEqualsSelf", MethodAttributes.Public | MethodAttributes.Static)
          .SetParameterTypes (typeof (int))
          .SetReturnType (typeof (bool));
      LocalReference local = method.DeclareLocal (typeof (int));
      method.AddStatement (new AssignStatement (local, method.ArgumentReferences[0].ToExpression()));
      method.ImplementByDelegating (local, typeof (int).GetMethod ("Equals", new[] { typeof (int) }));

      object instance = BuildInstance ();
      Assert.AreEqual (true, InvokeMethod (instance, method, 5));
    }

    [Test]
    public void ImplementByBaseCall ()
    {
      var method = ClassEmitter.CreateMethod ("NewEquals", MethodAttributes.Public)
          .SetParameterTypes (typeof (object))
          .SetReturnType (typeof (bool));
      method.ImplementByBaseCall (typeof (object).GetMethod ("Equals", new[] { typeof (object) }));

      object instance = BuildInstance ();

      Assert.AreEqual (false, InvokeMethod (instance, method, 5));
      Assert.AreEqual (true, InvokeMethod (instance, method, instance));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given method System.ICloneable.Clone is abstract.",
        MatchType = MessageMatch.Contains)]
    public void ImplementByBaseCallThrowsOnAbstractMethod ()
    {
      var method = ClassEmitter.CreateMethod ("NewEquals", MethodAttributes.Public)
          .SetParameterTypes (typeof (object))
          .SetReturnType (typeof (bool));
      method.ImplementByBaseCall (typeof (ICloneable).GetMethod ("Clone"));
    }

    [Test]
    [ExpectedException (typeof (NotFiniteNumberException), ExpectedMessage = "Who would have expected this?")]
    public void ImplementByThrowing ()
    {
      var method = ClassEmitter.CreateMethod ("ThrowingMethod", MethodAttributes.Public)
          .ImplementByThrowing (typeof (NotFiniteNumberException), "Who would have expected this?");

      try
      {
        BuildInstanceAndInvokeMethod (method);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void AddStatement ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public)
        .AddStatement (new ReturnStatement ());

      BuildInstanceAndInvokeMethod (method);
    }

    [Test]
    public void DeclareLocal ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public)
          .SetReturnType (typeof (int));
      LocalReference local = method.DeclareLocal (typeof (int));
      method.ImplementByReturning (local.ToExpression ());

      Assert.AreEqual (0, BuildInstanceAndInvokeMethod (method));
    }

    [Test]
    public void AddCustomAttribute ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public);
      method.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      MethodInfo methodInfo = BuildTypeAndGetMethod (method);
      Assert.AreEqual (1, methodInfo.GetCustomAttributes (typeof (SimpleAttribute), false).Length);
    }

    [Test]
    public void AcceptStatement ()
    {
      var fakeGenerator = new DynamicMethod ("Test", typeof (void), Type.EmptyTypes).GetILGenerator ();
      var statementMock = MockRepository.GenerateMock<Statement> ();

      var method = ClassEmitter.CreateMethod ("AcceptStatement", MethodAttributes.Public);
      method.AcceptStatement (statementMock, fakeGenerator);

      statementMock.AssertWasCalled (mock => mock.Emit (Arg<IMemberEmitter>.Matches (e => e.Member == method.MethodBuilder), Arg.Is (fakeGenerator)));
    }

    [Test]
    public void AcceptExpression ()
    {
      var fakeGenerator = new DynamicMethod ("Test", typeof (void), Type.EmptyTypes).GetILGenerator ();
      var expressionMock = MockRepository.GenerateMock<Expression> ();

      var method = ClassEmitter.CreateMethod ("AcceptStatement", MethodAttributes.Public);
      method.AcceptExpression (expressionMock, fakeGenerator);

      expressionMock.AssertWasCalled (mock => mock.Emit (Arg<IMemberEmitter>.Matches (e => e.Member == method.MethodBuilder), Arg.Is(fakeGenerator)));
    }
  }
}
