using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class ClassEmitterGenericsTest : CodeGenerationBaseTest
  {
    [Test]
    public void DeriveFromSimpleOpenGenericType ()
    {
      Type baseType = typeof (List<>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();

      Assert.IsTrue (builtType.ContainsGenericParameters);
      Type[] typeParameters = builtType.GetGenericArguments ();
      Assert.AreEqual (1, typeParameters.Length);
      Assert.IsTrue (builtType.BaseType.ContainsGenericParameters);
      Assert.AreEqual (typeParameters[0], builtType.BaseType.GetGenericArguments ()[0]);
    }

    [Test]
    public void DeriveFromClosedGenericTypeWithConstraints ()
    {
      Type baseType = typeof (GenericClassWithConstraints<ICloneable, List<string>, int, object, ICloneable, List<List<ICloneable[]>>>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();
      Assert.IsFalse (builtType.ContainsGenericParameters);
      Assert.IsFalse (builtType.BaseType.ContainsGenericParameters);
      Assert.AreEqual (typeof (GenericClassWithConstraints<ICloneable, List<string>, int, object, ICloneable, List<List<ICloneable[]>>>),
        builtType.BaseType);
    }

    [Test]
    public void DeriveFromOpenGenericTypeWithConstraints ()
    {
      Type baseType = typeof (GenericClassWithConstraints<,,,,,>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();
      Assert.IsTrue (builtType.ContainsGenericParameters);
      Type[] typeParameters = builtType.GetGenericArguments ();
      Assert.AreEqual (6, typeParameters.Length);
      Assert.IsTrue (builtType.BaseType.ContainsGenericParameters); // ?
      Assert.AreEqual (typeParameters[0], builtType.BaseType.GetGenericArguments ()[0]);
    }

    [Test]
    public void OverrideSimpleGenericMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (ClassWithSimpleGenericMethod), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (ClassWithSimpleGenericMethod).GetMethod ("GenericMethod");
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      ClassWithSimpleGenericMethod instance = (ClassWithSimpleGenericMethod) Activator.CreateInstance (builtType);

      string result = instance.GenericMethod ("1", 2, false);
      Assert.AreEqual ("1, 2, False", result);
    }

    [Test]
    public void OverrideConstrainedGenericMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (ClassWithConstrainedGenericMethod), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (ClassWithConstrainedGenericMethod).GetMethod ("GenericMethod");
      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      ClassWithConstrainedGenericMethod instance = (ClassWithConstrainedGenericMethod) Activator.CreateInstance (builtType);

      string result = instance.GenericMethod ("1", 2, "2");
      Assert.AreEqual ("1, 2, 2", result);
    }

    [Test]
    [Ignore ("This is currently not supported by DynamicProxy due to a CLR bug.")]
    public void OverrideGenericMethodInClosedGenericClassIsNotSupported ()
    {
      Type baseType = typeof (GenericClassWithGenericMethod<IConvertible, List<string>, int, object, IConvertible, List<List<IConvertible[]>>>);
      CustomClassEmitter classEmitter =
          new CustomClassEmitter (
              Scope,
              "Foo",
              baseType,
              Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = baseType.GetMethod ("GenericMethod");

      CustomMethodEmitter methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      GenericClassWithGenericMethod<IConvertible, List<string>, int, object, IConvertible, List<List<IConvertible[]>>> instance =
          (GenericClassWithGenericMethod<IConvertible, List<string>, int, object, IConvertible, List<List<IConvertible[]>>>)
          Activator.CreateInstance (builtType);

      string result = instance.GenericMethod (1, new List<int[]> (), new List<List<IConvertible[]>> ());
      Assert.AreEqual ("1, System.Collections.Generic.List`1[System.Int32[]], System.Collections.Generic.List`1[System.Collections.Generic.List`1["
          + "System.IConvertible[]]]", result);
    }

    [Test]
    public void OverridingSimpleMembersOfClosedGenericClass ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (GenericClassWithAllKindsOfMembers<int>), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
      CustomMethodEmitter overriddenMethod = classEmitter.CreateMethodOverride (baseMethod);
      overriddenMethod.ImplementByBaseCall (baseMethod);

      PropertyInfo baseProperty = typeof (GenericClassWithAllKindsOfMembers<int>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreatePropertyOverride (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateMethodOverride (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.ImplementByBaseCall (baseProperty.GetGetMethod ());

      EventInfo baseEvent = typeof (GenericClassWithAllKindsOfMembers<int>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateEventOverride (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateMethodOverride (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.ImplementByBaseCall (baseEvent.GetAddMethod ());
      overriddenEvent.RemoveMethod = classEmitter.CreateMethodOverride (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.ImplementByBaseCall (baseEvent.GetRemoveMethod ());

      Type builtType = classEmitter.BuildType ();
      GenericClassWithAllKindsOfMembers<int> instance = (GenericClassWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType);

      instance.Method (5);
      Assert.AreEqual (0, instance.Property);
      instance.Event += delegate { return 0;  };
      instance.Event -= delegate { return 0;  };
    }

    [Test]
    [Ignore ("Due to a bug in the .NET framework, this seems not to work on all .NET 2.0 installations at the moment. Waiting for a service pack...")]
    public void OverridingSimpleMembersOfOpenGenericClass ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (GenericClassWithAllKindsOfMembers<>), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericClassWithAllKindsOfMembers<>).GetMethod ("Method");
      CustomMethodEmitter overriddenMethod = classEmitter.CreateMethodOverride (baseMethod);
      overriddenMethod.ImplementByBaseCall (baseMethod);

      PropertyInfo baseProperty = typeof (GenericClassWithAllKindsOfMembers<>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreatePropertyOverride (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateMethodOverride (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.ImplementByBaseCall (baseProperty.GetGetMethod ());

      EventInfo baseEvent = typeof (GenericClassWithAllKindsOfMembers<>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateEventOverride (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateMethodOverride (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.ImplementByBaseCall (baseEvent.GetAddMethod ());
      overriddenEvent.RemoveMethod = classEmitter.CreateMethodOverride (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.ImplementByBaseCall (baseEvent.GetRemoveMethod ());

      Type builtType = classEmitter.BuildType ();
      GenericClassWithAllKindsOfMembers<int> instance =
          (GenericClassWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType.MakeGenericType (typeof (int)));

      instance.Method (5);
      Assert.AreEqual (0, instance.Property);
      instance.Event += delegate { return 0; };
      instance.Event -= delegate { return 0; };
    }

    [Test]
    public void ImplementingSimpleMembersOfOpenGenericInterface ()
    {
      CustomClassEmitter classEmitter =
          new CustomClassEmitter (
              Scope,
              "Foo",
              typeof (object),
              new Type[] {typeof (GenericInterfaceWithAllKindsOfMembers<int>)},
              TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetMethod ("Method");
      CustomMethodEmitter overriddenMethod = classEmitter.CreateInterfaceMethodImplementation (baseMethod);
      overriddenMethod.AddStatement (new ReturnStatement());

      PropertyInfo baseProperty = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreateInterfacePropertyImplementation (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateInterfaceMethodImplementation (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.AddStatement (new ReturnStatement (new ConstReference (13)));

      EventInfo baseEvent = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateInterfaceEventImplementation (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateInterfaceMethodImplementation (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.AddStatement (new ReturnStatement ());
      overriddenEvent.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();
      GenericInterfaceWithAllKindsOfMembers<int> instance = (GenericInterfaceWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType);

      instance.Method (7);
      Assert.AreEqual (13, instance.Property);
      instance.Event += delegate { return 0; };
      instance.Event -= delegate { return 0; };
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This emitter does not support nested types of non-closed generic types.")]
    public void ClassEmitterThrowsOnNestedGenericBase ()
    {
      new CustomClassEmitter (Scope, "Foo", typeof (GenericClassWithNested<>.Nested));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This emitter does not support open constructed types as base types. "
        + "Specify a closed type or a generic type definition.")]
    public void ClassEmitterThrowsOnOpenConstructedBase ()
    {
      Type genericParameter = typeof (ClassWithSimpleGenericMethod).GetMethod ("GenericMethod").GetGenericArguments()[0];
      Type openConstructedType = typeof (GenericClassWithNested<>).MakeGenericType (genericParameter);
      new CustomClassEmitter (Scope, "Foo", openConstructedType);
    }
  }
}