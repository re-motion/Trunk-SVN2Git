/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class ClassEmitterTest : CodeGenerationBaseTest
  {
    private readonly BindingFlags _declaredInstanceBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [Test]
    public void FlattenTypeName ()
    {
      Assert.AreEqual ("Namespace.Parent/Nested", CustomClassEmitter.FlattenTypeName ("Namespace.Parent+Nested"));
    }

    [Test]
    public void EmitSimpleClass ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "SimpleClass", typeof (ClassEmitterTest), new Type[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();

      Assert.AreEqual ("SimpleClass", builtType.FullName);
      Assert.AreEqual (typeof (ClassEmitterTest), builtType.BaseType);
      Assert.IsTrue (typeof (IMarkerInterface).IsAssignableFrom (builtType));
      Assert.IsTrue (builtType.IsClass);
      Assert.IsTrue (builtType.IsPublic);
    }

    [Test]
    public void HasBeenBuilt ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "HasBeenBuilt", typeof (ClassEmitterTest));
      Assert.IsFalse (classEmitter.HasBeenBuilt);
      classEmitter.BuildType();
      Assert.IsTrue (classEmitter.HasBeenBuilt);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Base type must not be an interface (System.IConvertible).",
        MatchType = MessageMatch.Contains)]
    public void ThrowsWhenInterfaceAsBaseClass ()
    {
      new CustomClassEmitter (Scope, "ThrowsWhenInterfaceAsBaseClass", typeof (IConvertible));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Base type must not be sealed (System.Int32).",
        MatchType = MessageMatch.Contains)]
    public void ThrowsWhenSealedTypeAsBaseClass ()
    {
      new CustomClassEmitter (Scope, "ThrowsWhenSealedTypeAsBaseClass", typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Interface type must not be a class or value type (System.Object).",
        MatchType = MessageMatch.Contains)]
    public void ThrowsWhenNonInterfaceAsInterface ()
    {
      new CustomClassEmitter (Scope, "ThrowsWhenNonInterfaceAsInterface", typeof (object), new Type[] { typeof (object) },
          TypeAttributes.Public | TypeAttributes.Class, false);
    }

    [Test]
    public void CreateConstructorCreateField ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateConstructorCreateField", typeof (object));
      FieldReference field = classEmitter.CreateField ("_test", typeof (string));
      ConstructorEmitter constructor = classEmitter.CreateConstructor (new Type[] { typeof (string), typeof (int) });
      constructor.CodeBuilder.InvokeBaseConstructor();
      constructor.CodeBuilder
          .AddStatement (new AssignStatement (field, new ArgumentReference (typeof (string), 1).ToExpression()))
          .AddStatement (new ReturnStatement());

      object instance = Activator.CreateInstance (classEmitter.BuildType (), "bla", 0);
      Assert.AreEqual ("bla", instance.GetType ().GetField ("_test").GetValue (instance));
    }

    [Test]
    public void CreateField_WithAttributes ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateField_WithAttributes", typeof (object));
      classEmitter.CreateField ("_test", typeof (string), FieldAttributes.Private);

      Type t = classEmitter.BuildType ();
      Assert.AreEqual (FieldAttributes.Private, t.GetField ("_test", BindingFlags.NonPublic | BindingFlags.Instance).Attributes);
    }

    [Test]
    public void CreateStaticField_WithAttributes ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateStaticField_WithAttributes", typeof (object));
      classEmitter.CreateStaticField ("_test", typeof (string), FieldAttributes.Private);

      Type t = classEmitter.BuildType ();
      Assert.AreEqual (FieldAttributes.Static | FieldAttributes.Private, t.GetField ("_test", BindingFlags.NonPublic | BindingFlags.Static).Attributes);
    }

    [Test]
    public void CreateDefaultConstructor ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateDefaultConstructor", typeof (object));
      classEmitter.CreateDefaultConstructor ();
      Activator.CreateInstance (classEmitter.BuildType ());
    }

    [Test]
    public void CreateTypeConstructorCreateStaticField ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateTypeConstructorCreateStaticField", typeof (object));
      FieldReference field = classEmitter.CreateStaticField ("s_test", typeof (string));
      classEmitter.CreateTypeConstructor ().CodeBuilder
          .AddStatement (new AssignStatement (field, (new ConstReference ("Yay").ToExpression ())))
          .AddStatement (new ReturnStatement ());
      Type t = classEmitter.BuildType ();
      Assert.AreEqual ("Yay", t.GetField ("s_test").GetValue (null));
    }

    [Test]
    public void CreateMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateMethod", typeof (object));
      CustomMethodEmitter method = classEmitter.CreateMethod ("Check", MethodAttributes.Public);
      method.SetReturnType (typeof (string));
      method.AddStatement (new ReturnStatement (new ConstReference ("ret")));
      
      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.AreEqual ("ret", instance.GetType().GetMethod ("Check").Invoke (instance, null));
    }

    [Test]
    public void CreateStaticMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateStaticMethod", typeof (object));
      CustomMethodEmitter method = classEmitter.CreateMethod ("Check", MethodAttributes.Public | MethodAttributes.Static);
      method.SetReturnType (typeof (string));
      method.AddStatement (new ReturnStatement (new ConstReference ("stat")));

      Type t = classEmitter.BuildType ();
      Assert.AreEqual ("stat", t.GetMethod ("Check").Invoke (null, null));
    }

    [Test]
    public void CreateProperty ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateProperty", typeof (object));
      CustomPropertyEmitter property = classEmitter.CreateProperty ("Check", PropertyKind.Instance, typeof (string), Type.EmptyTypes, PropertyAttributes.None);
      property.CreateGetMethod ().AddStatement (new ReturnStatement (new ConstReference ("4711")));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.AreEqual ("4711", instance.GetType ().GetProperty ("Check").GetValue (instance, null));
    }

    [Test]
    public void CreateEvent ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateEvent", typeof (object));
      CustomEventEmitter eventEmitter = classEmitter.CreateEvent ("Eve", EventKind.Instance, typeof (Func<string>));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.IsNotNull (instance.GetType ().GetEvent ("Eve"));
    }

    [Test]
    public void CreateMethodOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateMethodOverride", typeof (object), new Type[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);
      
      CustomMethodEmitter toStringMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("ToString"));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      MethodInfo method =
          builtType.GetMethod ("ToString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNotNull (method);
      Assert.IsTrue (method.IsPublic);
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", instance.ToString());
    }

    [Test]
    public void CreatePrivateMethodOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateMethodOverride", typeof (object), new Type[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter toStringMethod = classEmitter.CreatePrivateMethodOverride (typeof (object).GetMethod ("ToString"));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      MethodInfo method = builtType.GetMethod ("ToString",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNull (method);
      method = builtType.GetMethod ("System.Object.ToString",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNotNull (method);
      Assert.IsTrue (method.IsPrivate);
      Assert.IsTrue (method.IsVirtual);
      Assert.IsTrue (method.IsFinal);
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", instance.ToString ());
    }

    [Test]
    public void MethodNameAndVisibilityArePreservedOnOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityArePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), new Type[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter toStringMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("ToString", _declaredInstanceBindingFlags));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      CustomMethodEmitter finalizeMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("Finalize", _declaredInstanceBindingFlags));
      finalizeMethod.AddStatement (new ReturnStatement ());

      CustomMethodEmitter getterMethod = classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("get_Property", _declaredInstanceBindingFlags));
      getterMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      MethodInfo overriddenToString = builtType.GetMethod ("ToString", _declaredInstanceBindingFlags);
      Assert.AreEqual ("ToString", overriddenToString.Name);
      Assert.IsTrue (overriddenToString.IsPublic);
      Assert.IsFalse (overriddenToString.IsFinal);
      Assert.IsFalse (overriddenToString.IsStatic);
      Assert.IsFalse (overriddenToString.IsSpecialName);
      Assert.AreEqual (MethodAttributes.ReuseSlot, overriddenToString.Attributes & MethodAttributes.ReuseSlot);

      MethodInfo overriddenFinalize = builtType.GetMethod ("Finalize", _declaredInstanceBindingFlags);
      Assert.AreEqual ("Finalize", overriddenFinalize.Name);
      Assert.IsFalse (overriddenFinalize.IsPublic);
      Assert.IsTrue (overriddenFinalize.IsFamily);
      Assert.IsFalse (overriddenFinalize.IsStatic);
      Assert.IsFalse (overriddenFinalize.IsSpecialName);
      Assert.AreEqual (MethodAttributes.ReuseSlot, overriddenToString.Attributes & MethodAttributes.ReuseSlot);

      MethodInfo overriddenGetter = builtType.GetMethod ("get_Property", _declaredInstanceBindingFlags);
      Assert.AreEqual ("get_Property", overriddenGetter.Name);
      Assert.IsTrue (overriddenGetter.IsPublic);
      Assert.IsFalse (overriddenGetter.IsStatic);
      Assert.IsTrue (overriddenGetter.IsSpecialName);
      Assert.AreEqual (MethodAttributes.ReuseSlot, overriddenGetter.Attributes & MethodAttributes.ReuseSlot);
    }

    [Test]
    public void CreateInterfaceMethodImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateInterfaceMethodImplementation", typeof (object), new Type[] { typeof (ICloneable) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter cloneMethod = classEmitter.CreateInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone"));
      cloneMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", ((ICloneable)instance).Clone ());
    }

    [Test]
    public void MethodNameAndVisibilityAreChangedOnInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityAreChangedOnInterfaceImplementation", typeof (object), new Type[] { typeof (ICloneable) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter method =
          classEmitter.CreateInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone", _declaredInstanceBindingFlags));
      method.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      MethodInfo implementedMethod = builtType.GetMethod ("System.ICloneable.Clone", _declaredInstanceBindingFlags);
      Assert.AreEqual ("System.ICloneable.Clone", implementedMethod.Name);
      Assert.IsFalse (implementedMethod.IsPublic);
      Assert.IsTrue (implementedMethod.IsPrivate);
      Assert.IsTrue (implementedMethod.IsFinal);
      Assert.IsFalse (implementedMethod.IsStatic);
      Assert.IsFalse (implementedMethod.IsSpecialName);
      Assert.AreEqual (MethodAttributes.NewSlot, implementedMethod.Attributes & MethodAttributes.NewSlot);
    }

    [Test]
    public void CreatePublicInterfaceMethodImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfaceMethodImplementation", typeof (object),
          new Type[] { typeof (ICloneable) }, TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter cloneMethod = classEmitter.CreatePublicInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone"));
      cloneMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", ((ICloneable) instance).Clone ());
    }

    [Test]
    public void MethodNameAndVisibilityAreUnchangedOnPublicInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityAreUnchangedOnPublicInterfaceImplementation",
          typeof (object), new Type[] { typeof (ICloneable) }, TypeAttributes.Public | TypeAttributes.Class, false);

      CustomMethodEmitter method =
          classEmitter.CreatePublicInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone", _declaredInstanceBindingFlags));
      method.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      MethodInfo implementedMethod = builtType.GetMethod ("Clone", _declaredInstanceBindingFlags);
      Assert.AreEqual ("Clone", implementedMethod.Name);
      Assert.IsTrue (implementedMethod.IsPublic);
      Assert.IsFalse (implementedMethod.IsFinal);
      Assert.AreEqual (MethodAttributes.NewSlot, implementedMethod.Attributes & MethodAttributes.NewSlot);
    }

    [Test]
    public void CreatePropertyOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreatePropertyOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property =
          classEmitter.CreatePropertyOverride (typeof (ClassWithAllKindsOfMembers).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      // only override getter, not setter
      property.GetMethod =
          classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("get_Property", _declaredInstanceBindingFlags));
      property.GetMethod.AddStatement (new ReturnStatement (new ConstReference (17)));

      Type builtType = classEmitter.BuildType ();
      ClassWithAllKindsOfMembers instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      Assert.AreEqual (17, instance.Property); // overridden
      instance.Property = 7; // inherited, not overridden
    }

    [Test]
    public void CreateIndexedPropertyOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateIndexedPropertyOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      PropertyInfo baseProperty = typeof (ClassWithAllKindsOfMembers).GetProperty ("Item", _declaredInstanceBindingFlags);
      CustomPropertyEmitter property = classEmitter.CreatePropertyOverride (baseProperty);

      property.CreateGetMethod ().ImplementByBaseCall (baseProperty.GetGetMethod ());
      property.CreateSetMethod ().ImplementByBaseCall (baseProperty.GetSetMethod ());

      Type builtType = classEmitter.BuildType ();
      ClassWithAllKindsOfMembers instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      Assert.AreEqual ("17", instance[17]);
      instance[18] = "foo";
    }

    [Test]
    public void PropertyNamePreservedOnOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "PropertyNamePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property =
          classEmitter.CreatePropertyOverride (typeof (ClassWithAllKindsOfMembers).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.AreEqual ("Property", property.PropertyBuilder.Name);

      classEmitter.BuildType ();
    }

    public interface IInterfaceWithProperty
    {
      int Property { set; }
    }

    [Test]
    public void CreateInterfacePropertyImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateInterfacePropertyImplementation", typeof (object), new Type[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreateInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      IInterfaceWithProperty instance = (IInterfaceWithProperty) Activator.CreateInstance (builtType);
      instance.Property = 7;
    }

    [Test]
    public void CreatePublicInterfacePropertyImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfacePropertyImplementation", typeof (object), new Type[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreatePublicInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      IInterfaceWithProperty instance = (IInterfaceWithProperty) Activator.CreateInstance (builtType);
      instance.Property = 7;
    }

    [Test]
    public void PropertyNameIsChangedOnInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "PropertyNameIsChangedOnInterfaceImplementation", typeof (object), new Type[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreateInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.AreNotEqual ("Property", property.PropertyBuilder.Name);
      Assert.AreEqual (typeof (IInterfaceWithProperty).FullName + ".Property", property.PropertyBuilder.Name);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      classEmitter.BuildType ();
    }

    [Test]
    public void PropertyNameIsNotChangedOnPublicInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "PropertyNameIsNotChangedOnPublicInterfaceImplementation", typeof (object), new Type[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreatePublicInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.AreEqual ("Property", property.PropertyBuilder.Name);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      classEmitter.BuildType ();
    }

    [Test]
    public void CreateEventOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateEventOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter =
          classEmitter.CreateEventOverride (typeof (ClassWithAllKindsOfMembers).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod =
          classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod =
          classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());
      
      Type builtType = classEmitter.BuildType ();
      ClassWithAllKindsOfMembers instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void EventNamePreservedOnOverride ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "EventNamePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter =
          classEmitter.CreateEventOverride (typeof (ClassWithAllKindsOfMembers).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod =
          classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod =
          classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      Assert.IsNotNull (builtType.GetEvent ("Event", _declaredInstanceBindingFlags));
    }

    public interface IInterfaceWithEvent
    {
      event EventHandler Event;
    }

    [Test]
    public void CreateInterfaceEventImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreateInterfaceEventImplementation", typeof (object), new Type[] { typeof (IInterfaceWithEvent) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter = classEmitter.CreateInterfaceEventImplementation (
          typeof (IInterfaceWithEvent).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      IInterfaceWithEvent instance = (IInterfaceWithEvent) Activator.CreateInstance (builtType);
      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void CreatePublicInterfaceEventImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfaceEventImplementation", typeof (object), new Type[] { typeof (IInterfaceWithEvent) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter = classEmitter.CreatePublicInterfaceEventImplementation (
          typeof (IInterfaceWithEvent).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      IInterfaceWithEvent instance = (IInterfaceWithEvent) Activator.CreateInstance (builtType);
      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void EventNameIsChangedOnInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "EventNameIsChangedOnInterfaceImplementation", typeof (object), new Type[] { typeof (IInterfaceWithEvent) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter = classEmitter.CreateInterfaceEventImplementation (
          typeof (IInterfaceWithEvent).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      Assert.IsNull (builtType.GetEvent ("Event"));
      Assert.IsNotNull (builtType.GetEvent (typeof (IInterfaceWithEvent).FullName + ".Event", _declaredInstanceBindingFlags));
    }

    [Test]
    public void EventNameIsNotChangedOnPublicInterfaceImplementation ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "EventNameIsNotChangedOnPublicInterfaceImplementation", typeof (object), new Type[] { typeof (IInterfaceWithEvent) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomEventEmitter eventEmitter = classEmitter.CreatePublicInterfaceEventImplementation (
          typeof (IInterfaceWithEvent).GetEvent ("Event", _declaredInstanceBindingFlags));

      eventEmitter.AddMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("add_Event", _declaredInstanceBindingFlags));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());

      eventEmitter.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithEvent).GetMethod ("remove_Event", _declaredInstanceBindingFlags));
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      Assert.IsNotNull (builtType.GetEvent ("Event", _declaredInstanceBindingFlags));
    }

    [Test]
    public void AddCustomAttribute ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "AddCustomAttribute", typeof (object));
      classEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0],
          typeof (SimpleAttribute).GetFields (), new object[] { "value" }));

      Type builtType = classEmitter.BuildType ();

      SimpleAttribute[] attributes = (SimpleAttribute[]) builtType.GetCustomAttributes (typeof (SimpleAttribute), false);
      Assert.AreEqual (1, attributes.Length);
      Assert.AreEqual ("value", attributes[0].S);
    }

    [Test]
    public void ReplicateBaseTypeConstructors ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ReplicateBaseTypeConstructors", typeof (List<int>));
      FieldReference field = classEmitter.CreateStaticField ("s_ctorsCalled", typeof (int));

      classEmitter.ReplicateBaseTypeConstructors (
          new ILStatement (delegate (IMemberEmitter member, ILGenerator generator)
          {
            field.LoadReference (generator);
            generator.Emit (OpCodes.Ldc_I4_1);
            generator.Emit (OpCodes.Add);
            field.StoreReference (generator);
          }));

      Type builtType = classEmitter.BuildType ();
      Assert.AreEqual (0, builtType.GetField ("s_ctorsCalled").GetValue (null));

      Activator.CreateInstance (builtType); // default ctor

      Assert.AreEqual (1, builtType.GetField ("s_ctorsCalled").GetValue (null));

      List<int> list = (List<int>) Activator.CreateInstance (builtType, 5); // capacity
      Assert.AreEqual (5, list.Capacity);

      Assert.AreEqual (2, builtType.GetField ("s_ctorsCalled").GetValue (null));

      list = (List<int>) Activator.CreateInstance (builtType, new int[] { 1, 2, 3 }); // IEnumerable
      Assert.AreEqual (3, list.Count);
      Assert.AreEqual (1, list[0]);
      Assert.AreEqual (2, list[1]);
      Assert.AreEqual (3, list[2]);

      Assert.AreEqual (3, builtType.GetField ("s_ctorsCalled").GetValue (null));
    }

    [Test]
    public void GetPublicMethodWrapper ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper", typeof (ClassWithProtectedMethod));
      CustomMethodEmitter emitter1 =
          classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));
      CustomMethodEmitter emitter2 =
          classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));
      Assert.AreSame (emitter1, emitter2);

      CustomMethodEmitter emitter3 =
          classEmitter.GetPublicMethodWrapper (typeof (object).GetMethod ("Finalize", _declaredInstanceBindingFlags));
      Assert.AreNotSame (emitter1, emitter3);

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      MethodInfo publicWrapper = instance.GetType().GetMethod ("__wrap__GetSecret");
      Assert.IsNotNull (publicWrapper);
      Assert.AreEqual (
          "The secret is to be more provocative and interesting than anything else in [the] environment.", publicWrapper.Invoke (instance, null));
    }

    [Test]
    public void ForceUnsignedTrue ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper", typeof (object),
          Type.EmptyTypes, TypeAttributes.Public, true);
      Type t = classEmitter.BuildType ();
      Assert.IsFalse (StrongNameUtil.IsAssemblySigned (t.Assembly));
    }

    [Test]
    public void ForceUnsignedFalse()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper", typeof (object),
          Type.EmptyTypes, TypeAttributes.Public, false);
      Type t = classEmitter.BuildType ();
      Assert.IsTrue (StrongNameUtil.IsAssemblySigned (t.Assembly));
    }
  }
}
