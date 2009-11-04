// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class ClassEmitterTest : CodeGenerationBaseTest
  {
    private const BindingFlags _declaredInstanceBindingFlags = 
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [Test]
    public void FlattenTypeName ()
    {
      Assert.AreEqual ("Namespace.Parent/Nested", CustomClassEmitter.FlattenTypeName ("Namespace.Parent+Nested"));
    }

    [Test]
    public void EmitSimpleClass ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "SimpleClass", typeof (ClassEmitterTest), new[] { typeof (IMarkerInterface) },
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
      var classEmitter = new CustomClassEmitter (Scope, "HasBeenBuilt", typeof (ClassEmitterTest));
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
      new CustomClassEmitter (Scope, "ThrowsWhenNonInterfaceAsInterface", typeof (object), new[] { typeof (object) },
          TypeAttributes.Public | TypeAttributes.Class, false);
    }

    [Test]
    public void CreateConstructorCreateField ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateConstructorCreateField", typeof (object));
      FieldReference field = classEmitter.CreateField ("_test", typeof (string));
      ConstructorEmitter constructor = classEmitter.CreateConstructor (new[] { typeof (string), typeof (int) });
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateField_WithAttributes", typeof (object));
      classEmitter.CreateField ("_test", typeof (string), FieldAttributes.Private);

      Type t = classEmitter.BuildType ();
      Assert.AreEqual (FieldAttributes.Private, t.GetField ("_test", BindingFlags.NonPublic | BindingFlags.Instance).Attributes);
    }

    [Test]
    public void CreateStaticField_WithAttributes ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateStaticField_WithAttributes", typeof (object));
      classEmitter.CreateStaticField ("_test", typeof (string), FieldAttributes.Private);

      Type t = classEmitter.BuildType ();
      Assert.AreEqual (FieldAttributes.Static | FieldAttributes.Private, t.GetField ("_test", BindingFlags.NonPublic | BindingFlags.Static).Attributes);
    }

    [Test]
    public void CreateDefaultConstructor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateDefaultConstructor", typeof (object));
      classEmitter.CreateDefaultConstructor ();
      Activator.CreateInstance (classEmitter.BuildType ());
    }

    [Test]
    public void CreateTypeConstructorCreateStaticField ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateTypeConstructorCreateStaticField", typeof (object));
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateMethod", typeof (object));
      var method = classEmitter.CreateMethod ("Check", MethodAttributes.Public);
      method.SetReturnType (typeof (string));
      method.AddStatement (new ReturnStatement (new ConstReference ("ret")));
      
      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.AreEqual ("ret", instance.GetType().GetMethod ("Check").Invoke (instance, null));
    }

    [Test]
    public void CreateStaticMethod ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateStaticMethod", typeof (object));
      var method = classEmitter.CreateMethod ("Check", MethodAttributes.Public | MethodAttributes.Static);
      method.SetReturnType (typeof (string));
      method.AddStatement (new ReturnStatement (new ConstReference ("stat")));

      Type t = classEmitter.BuildType ();
      Assert.AreEqual ("stat", t.GetMethod ("Check").Invoke (null, null));
    }

    [Test]
    public void CreateProperty ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateProperty", typeof (object));
      CustomPropertyEmitter property = classEmitter.CreateProperty ("Check", PropertyKind.Instance, typeof (string), Type.EmptyTypes, PropertyAttributes.None);
      property.CreateGetMethod ().AddStatement (new ReturnStatement (new ConstReference ("4711")));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.AreEqual ("4711", instance.GetType ().GetProperty ("Check").GetValue (instance, null));
    }

    [Test]
    public void CreateEvent ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateEvent", typeof (object));
      CustomEventEmitter eventEmitter = classEmitter.CreateEvent ("Eve", EventKind.Instance, typeof (Func<string>));
      eventEmitter.AddMethod.AddStatement (new ReturnStatement ());
      eventEmitter.RemoveMethod.AddStatement (new ReturnStatement ());

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      Assert.IsNotNull (instance.GetType ().GetEvent ("Eve"));
    }

    [Test]
    public void CreateMethodOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateMethodOverride", typeof (object), new[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var toStringMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("ToString"));
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
    public void CreateFullNamedMethodOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateFullNamedMethodOverride", typeof (object), new[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var toStringMethod = classEmitter.CreateFullNamedMethodOverride (typeof (object).GetMethod ("ToString"));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      MethodInfo method = builtType.GetMethod ("ToString",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNull (method);
      method = builtType.GetMethod ("System.Object.ToString",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNotNull (method);
      Assert.IsTrue (method.IsPublic);
      Assert.IsTrue (method.IsVirtual);
      Assert.IsTrue (method.IsFinal);
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", instance.ToString ());
    }

    [Test]
    public void CreateFullNamedMethodOverride_ProtectedMethod ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateFullNamedMethodOverride_ProtectedMethod", typeof (ClassWithProtectedVirtualMethod), new[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var toStringMethod = classEmitter.CreateFullNamedMethodOverride (typeof (ClassWithProtectedVirtualMethod).GetMethod (
          "GetSecret", BindingFlags.NonPublic | BindingFlags.Instance));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      MethodInfo method = builtType.GetMethod ("GetSecret",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNull (method);
      method = builtType.GetMethod (typeof (ClassWithProtectedVirtualMethod).FullName + ".GetSecret",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.IsNotNull (method);
      Assert.IsTrue (method.IsFamily);
      Assert.IsTrue (method.IsVirtual);
      Assert.IsTrue (method.IsFinal);
      
      var instance = (ClassWithProtectedVirtualMethod) Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", method.Invoke (instance, null));
    }

    [Test]
    public void MethodNameAndVisibilityArePreservedOnOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityArePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), new[] { typeof (IMarkerInterface) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var toStringMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("ToString", _declaredInstanceBindingFlags));
      toStringMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      var finalizeMethod = classEmitter.CreateMethodOverride (typeof (object).GetMethod ("Finalize", _declaredInstanceBindingFlags));
      finalizeMethod.AddStatement (new ReturnStatement ());

      var getterMethod = classEmitter.CreateMethodOverride (typeof (ClassWithAllKindsOfMembers).GetMethod ("get_Property", _declaredInstanceBindingFlags));
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateInterfaceMethodImplementation", typeof (object), new[] { typeof (ICloneable) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var cloneMethod = classEmitter.CreateInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone"));
      cloneMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", ((ICloneable)instance).Clone ());
    }

    [Test]
    public void MethodNameAndVisibilityAreChangedOnInterfaceImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityAreChangedOnInterfaceImplementation", typeof (object), new[] { typeof (ICloneable) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      var method =
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
      var classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfaceMethodImplementation", typeof (object),
          new[] { typeof (ICloneable) }, TypeAttributes.Public | TypeAttributes.Class, false);

      var cloneMethod = classEmitter.CreatePublicInterfaceMethodImplementation (typeof (ICloneable).GetMethod ("Clone"));
      cloneMethod.AddStatement (new ReturnStatement (new ConstReference ("P0wned!")));

      Type builtType = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (builtType);
      Assert.AreEqual ("P0wned!", ((ICloneable) instance).Clone ());
    }

    [Test]
    public void MethodNameAndVisibilityAreUnchangedOnPublicInterfaceImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "MethodNameAndVisibilityAreUnchangedOnPublicInterfaceImplementation",
          typeof (object), new[] { typeof (ICloneable) }, TypeAttributes.Public | TypeAttributes.Class, false);

      var method =
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
      var classEmitter = new CustomClassEmitter (Scope, "CreatePropertyOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
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
      var instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      Assert.AreEqual (17, instance.Property); // overridden
      instance.Property = 7; // inherited, not overridden
    }

    [Test]
    public void CreateIndexedPropertyOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateIndexedPropertyOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      PropertyInfo baseProperty = typeof (ClassWithAllKindsOfMembers).GetProperty ("Item", _declaredInstanceBindingFlags);
      CustomPropertyEmitter property = classEmitter.CreatePropertyOverride (baseProperty);

      property.CreateGetMethod ().ImplementByBaseCall (baseProperty.GetGetMethod ());
      property.CreateSetMethod ().ImplementByBaseCall (baseProperty.GetSetMethod ());

      Type builtType = classEmitter.BuildType ();
      var instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      Assert.AreEqual ("17", instance[17]);
      instance[18] = "foo";
    }

    [Test]
    public void PropertyNamePreservedOnOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "PropertyNamePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateInterfacePropertyImplementation", typeof (object), new[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreateInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      var instance = (IInterfaceWithProperty) Activator.CreateInstance (builtType);
      instance.Property = 7;
    }

    [Test]
    public void CreatePublicInterfacePropertyImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfacePropertyImplementation", typeof (object), new[] { typeof (IInterfaceWithProperty) },
          TypeAttributes.Public | TypeAttributes.Class, false);

      CustomPropertyEmitter property = classEmitter.CreatePublicInterfacePropertyImplementation (
          typeof (IInterfaceWithProperty).GetProperty ("Property", _declaredInstanceBindingFlags));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      property.SetMethod = classEmitter.CreateInterfaceMethodImplementation (
          typeof (IInterfaceWithProperty).GetMethod ("set_Property", _declaredInstanceBindingFlags));
      property.SetMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();

      var instance = (IInterfaceWithProperty) Activator.CreateInstance (builtType);
      instance.Property = 7;
    }

    [Test]
    public void PropertyNameIsChangedOnInterfaceImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "PropertyNameIsChangedOnInterfaceImplementation", typeof (object), new[] { typeof (IInterfaceWithProperty) },
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
      var classEmitter = new CustomClassEmitter (Scope, "PropertyNameIsNotChangedOnPublicInterfaceImplementation", typeof (object), new[] { typeof (IInterfaceWithProperty) },
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateEventOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
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
      var instance = (ClassWithAllKindsOfMembers) Activator.CreateInstance (builtType);

      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void EventNamePreservedOnOverride ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "EventNamePreservedOnOverride", typeof (ClassWithAllKindsOfMembers), Type.EmptyTypes,
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
      var classEmitter = new CustomClassEmitter (Scope, "CreateInterfaceEventImplementation", typeof (object), new[] { typeof (IInterfaceWithEvent) },
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

      var instance = (IInterfaceWithEvent) Activator.CreateInstance (builtType);
      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void CreatePublicInterfaceEventImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreatePublicInterfaceEventImplementation", typeof (object), new[] { typeof (IInterfaceWithEvent) },
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

      var instance = (IInterfaceWithEvent) Activator.CreateInstance (builtType);
      instance.Event += delegate { };
      instance.Event -= delegate { };
    }

    [Test]
    public void EventNameIsChangedOnInterfaceImplementation ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "EventNameIsChangedOnInterfaceImplementation", typeof (object), new[] { typeof (IInterfaceWithEvent) },
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
      var classEmitter = new CustomClassEmitter (Scope, "EventNameIsNotChangedOnPublicInterfaceImplementation", typeof (object), new[] { typeof (IInterfaceWithEvent) },
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
      var classEmitter = new CustomClassEmitter (Scope, "AddCustomAttribute", typeof (object));
      classEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0],
          typeof (SimpleAttribute).GetFields (), new object[] { "value" }));

      Type builtType = classEmitter.BuildType ();

      var attributes = (SimpleAttribute[]) builtType.GetCustomAttributes (typeof (SimpleAttribute), false);
      Assert.AreEqual (1, attributes.Length);
      Assert.AreEqual ("value", attributes[0].S);
    }

    [Test]
    public void ReplicateBaseTypeConstructors ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ReplicateBaseTypeConstructors", typeof (ClassWithReplicatableConstructors));
      var fieldReference = new FieldInfoReference (SelfReference.Self, typeof (ClassWithReplicatableConstructors).GetField ("CtorString"));

      var concatMethod = typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) });
      var preConcatExpression = new MethodInvocationExpression (null, concatMethod, fieldReference.ToExpression (), new ConstReference ("pre").ToExpression ());
      var postConcatExpression = new MethodInvocationExpression (null, concatMethod, fieldReference.ToExpression (), new ConstReference ("post").ToExpression ());

      classEmitter.ReplicateBaseTypeConstructors (
        emitter => emitter.CodeBuilder.AddStatement (new AssignStatement (fieldReference, preConcatExpression)),
        emitter => emitter.CodeBuilder.AddStatement (new AssignStatement (fieldReference, postConcatExpression)));

      Type builtType = classEmitter.BuildType ();

      var instance1 = (ClassWithReplicatableConstructors) Activator.CreateInstance (builtType); // default ctor
      Assert.That (instance1.CtorString, Is.EqualTo ("preClassWithReplicatableConstructors()post"));

      var instance2 = (ClassWithReplicatableConstructors) Activator.CreateInstance (builtType, 7); // int ctor
      Assert.That (instance2.CtorString, Is.EqualTo ("preClassWithReplicatableConstructors(7)post"));
    }

    [Test]
    public void GetPublicMethodWrapper ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper", typeof (ClassWithProtectedMethod));
      classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      MethodInfo publicWrapper = instance.GetType().GetMethod ("__wrap__GetSecret");
      Assert.IsNotNull (publicWrapper);
      Assert.AreEqual ("The secret is to be more provocative and interesting than anything else in [the] environment.", 
          publicWrapper.Invoke (instance, null));
    }

    [Test]
    public void GetPublicMethodWrapper_Cached ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper_Cached", typeof (ClassWithProtectedMethod));
      MethodInfo emitter1 =
          classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));
      MethodInfo emitter2 =
          classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));
      Assert.AreSame (emitter1, emitter2);

      MethodInfo emitter3 =
      classEmitter.GetPublicMethodWrapper (typeof (object).GetMethod ("Finalize", _declaredInstanceBindingFlags));
      Assert.AreNotSame (emitter1, emitter3);

      classEmitter.BuildType ();
    }

    [Test]
    public void GetPublicMethodWrapper_HasAttribute()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper_HasAttribute", typeof (ClassWithProtectedMethod));
      classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      MethodInfo publicWrapper = instance.GetType ().GetMethod ("__wrap__GetSecret");
      
      var attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (publicWrapper, false);
      Assert.That (attribute, Is.Not.Null);
    }

    [Test]
    public void GetPublicMethodWrapper_Attribute_HasRightGenericTypeArguments_Empty ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper_Attribute_HasRightGenericTypeArguments_Empty", typeof (ClassWithProtectedMethod));
      classEmitter.GetPublicMethodWrapper (typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      MethodInfo publicWrapper = instance.GetType ().GetMethod ("__wrap__GetSecret");

      var attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (publicWrapper, false);
      Assert.That (attribute.GenericTypeArguments, Is.Empty);
    }

    [Test]
    public void GetPublicMethodWrapper_Attribute_HasRightGenericTypeArguments_NotEmpty ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper_Attribute_HasRightGenericTypeArguments_NotEmpty", typeof (GenericClassWithAllKindsOfMembers<string>));
      classEmitter.GetPublicMethodWrapper (typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method", _declaredInstanceBindingFlags));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      MethodInfo publicWrapper = instance.GetType ().GetMethod ("__wrap__Method");

      var attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (publicWrapper, false);
      Assert.That (attribute.GenericTypeArguments, Is.EqualTo (new[] {typeof (string)}));
    }

    [Test]
    public void GetPublicMethodWrapper_Attribute_HasRightToken ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "GetPublicMethodWrapper_Attribute_HasRightToken", typeof (ClassWithProtectedMethod));
      var methodToBeWrapped = typeof (ClassWithProtectedMethod).GetMethod ("GetSecret", _declaredInstanceBindingFlags);
      classEmitter.GetPublicMethodWrapper (methodToBeWrapped);

      Type type = classEmitter.BuildType ();
      MethodInfo publicWrapper = type.GetMethod ("__wrap__GetSecret");

      int expectedToken = Scope.WeakNamedModule.GetMethodToken (methodToBeWrapped).Token;

      var attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (publicWrapper, false);
      Assert.That (attribute.WrappedMethodRefToken, Is.EqualTo (expectedToken));

      Assert.That (attribute.ResolveWrappedMethod (type.Module), Is.EqualTo (methodToBeWrapped));
    }

    [Test]
    public void ForceUnsignedTrue ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ForceUnsignedTrue", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      Type t = classEmitter.BuildType ();
      Assert.IsFalse (StrongNameUtil.IsAssemblySigned (t.Assembly));
    }

    [Test]
    public void ForceUnsignedFalse()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ForceUnsignedFalse", typeof (object), Type.EmptyTypes, TypeAttributes.Public, false);
      Type t = classEmitter.BuildType ();
      Assert.IsTrue (StrongNameUtil.IsAssemblySigned (t.Assembly));
    }

    [Test]
    public void CreateNestedClass ()
    {
      // must be unsigned because the nested class uses unsigned interface and base type
      var classEmitter = new CustomClassEmitter (Scope, "CreateNestedClass", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      var innerClassEmitter = classEmitter.CreateNestedClass (
          "InnerClass", 
          typeof (ClassWithAllKindsOfMembers), 
          new[] { typeof (IInterfaceWithMethod) });

      var innerT = innerClassEmitter.BuildType ();
      var outerT = classEmitter.BuildType ();

      Assert.That (innerT.FullName, Is.EqualTo ("CreateNestedClass+InnerClass"));
      Assert.That (innerT.Attributes, Is.EqualTo (TypeAttributes.NestedPublic | TypeAttributes.Sealed));
      Assert.That (innerT.BaseType, Is.SameAs (typeof (ClassWithAllKindsOfMembers)));
      Assert.That (innerT.GetInterfaces (), Is.EquivalentTo (new[] { typeof (IInterfaceWithMethod) }));
      Assert.That (innerT.DeclaringType, Is.SameAs (outerT));
      Assert.That (outerT.GetNestedType ("InnerClass"), Is.SameAs (innerT));
    }

    [Test]
    public void CreateNestedClass_Flags ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "CreateNestedClass", typeof (object), Type.EmptyTypes, TypeAttributes.Public, false);
      var innerClassEmitter = classEmitter.CreateNestedClass (
          "IInnerInterface",
          null,
          new[] { typeof (IServiceProvider) },
          TypeAttributes.Abstract | TypeAttributes.Interface | TypeAttributes.NestedAssembly);

      var innerT = innerClassEmitter.BuildType ();
      var outerT = classEmitter.BuildType ();

      Assert.That (innerT.FullName, Is.EqualTo ("CreateNestedClass+IInnerInterface"));
      Assert.That (innerT.Attributes, Is.EqualTo (TypeAttributes.NestedAssembly | TypeAttributes.Abstract | TypeAttributes.Interface));
      Assert.That (innerT.BaseType, Is.Null);
      Assert.That (innerT.GetInterfaces (), Is.EquivalentTo (new[] { typeof (IServiceProvider) }));
      Assert.That (innerT.DeclaringType, Is.SameAs (outerT));
      Assert.That (outerT.GetNestedType ("IInnerInterface", BindingFlags.NonPublic), Is.SameAs (innerT));
    }
  }
}
