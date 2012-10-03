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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.TestDomain;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class PropertyEmitterTest : CodeGenerationBaseTest
  {
    private CustomClassEmitter _classEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = new CustomClassEmitter (Scope, UniqueName, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true); // force unsigned because we use SimpleAttribute below
    }

    public override void TearDown ()
    {
      if (!_classEmitter.HasBeenBuilt)
        _classEmitter.BuildType();

      base.TearDown();
    }

    private object BuildInstance ()
    {
      return Activator.CreateInstance (_classEmitter.BuildType ());
    }

    private object GetPropertyValue (object instance, CustomPropertyEmitter property, params object[] arguments)
    {
      return GetProperty(instance, property).GetValue (instance, arguments);
    }

    private object GetPropertyValue (Type type, CustomPropertyEmitter property, params object[] arguments)
    {
      return GetProperty (type, property).GetValue (null, arguments);
    }

    private void SetPropertyValue (object value, object instance, CustomPropertyEmitter property, params object[] arguments)
    {
      GetProperty (instance, property).SetValue (instance, value, arguments);
    }

    private void SetPropertyValue (object value, Type type, CustomPropertyEmitter property, params object[] arguments)
    {
      GetProperty (type, property).SetValue (null, value, arguments);
    }

    private PropertyInfo GetProperty (Type builtType, CustomPropertyEmitter property)
    {
      return builtType.GetProperty (property.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private PropertyInfo GetProperty (object instance, CustomPropertyEmitter property)
    {
      return GetProperty (instance.GetType (), property);
    }

    [Test]
    public void SimpleProperty ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SimpleProperty", PropertyKind.Instance, typeof (int));

      Assert.AreEqual ("SimpleProperty", property.Name);
      Assert.AreEqual (typeof (int), property.PropertyType);
      Assert.AreEqual (PropertyKind.Instance, property.PropertyKind);
      Assert.IsEmpty (property.IndexParameters);

      property.CreateGetMethod();
      property.CreateSetMethod();
      property.ImplementWithBackingField ();

      object instance = BuildInstance ();
      Assert.AreEqual (0, GetPropertyValue (instance, property));
      SetPropertyValue (17, instance, property);
      Assert.AreEqual (17, GetPropertyValue (instance, property));
    }

    [Test]
    public void StaticProperty ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("StaticProperty", PropertyKind.Static, typeof (string));

      Assert.AreEqual ("StaticProperty", property.Name);
      Assert.AreEqual (typeof (string), property.PropertyType);
      Assert.AreEqual (PropertyKind.Static, property.PropertyKind);
      Assert.IsEmpty (property.IndexParameters);

      property.CreateGetMethod();
      property.CreateSetMethod();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();
      Assert.AreEqual (null, GetPropertyValue (type, property));
      SetPropertyValue ("bla", type, property);
      Assert.AreEqual ("bla", GetPropertyValue (type, property));
    }

    [Test]
    public void IndexedProperty ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "StaticProperty", PropertyKind.Static, typeof (string), new Type[] {typeof (int), typeof (double)}, PropertyAttributes.None);

      Assert.That (property.IndexParameters, Is.EqualTo (new Type[] { typeof (int), typeof (double) }));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();

      Assert.AreEqual (null, GetPropertyValue (type, property, 2, 2.0));
      SetPropertyValue ("whatever", type, property, 2, 2.0);
      Assert.AreEqual ("whatever", GetPropertyValue (type, property, 2, 2.0));
    }

    [Test]
    public void NoGetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("NoGetMethod", PropertyKind.Static, typeof (string));
      Assert.IsNull (property.GetMethod);
      Type type = _classEmitter.BuildType ();
      Assert.IsNull (GetProperty (type, property).GetGetMethod ());
    }

    [Test]
    public void NoSetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("NoSetMethod", PropertyKind.Static, typeof (string));
      Assert.IsNull (property.SetMethod);
      Type type = _classEmitter.BuildType ();
      Assert.IsNull (GetProperty (type, property).GetSetMethod ());
    }

    [Test]
    public void SpecificGetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SpecificGetMethod", PropertyKind.Static, typeof (string));
      property.CreateGetMethod ().ImplementByReturning (new ConstReference ("You are my shunsine").ToExpression ());
      Type type = _classEmitter.BuildType ();
      Assert.AreEqual ("You are my shunsine", GetPropertyValue (type, property));
    }

    [Test]
    public void SpecificSetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SpecificSetMethod", PropertyKind.Static, typeof (string));
      property.CreateSetMethod ().ImplementByThrowing (typeof (Exception), "My only shunsine");
      Type type = _classEmitter.BuildType ();
      try
      {
        SetPropertyValue ("", type, property);
      }
      catch (TargetInvocationException ex)
      {
        Assert.IsTrue (ex.InnerException.GetType () == typeof (Exception));
        Assert.AreEqual ("My only shunsine", ex.InnerException.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Due to limitations in Reflection.Emit, property accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void GetMethodCannotBeSetToNull()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("GetMethodCannotBeSetToNull", PropertyKind.Static, typeof (string));
      property.CreateGetMethod ();
      property.GetMethod = null;
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Due to limitations in Reflection.Emit, property accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void SetMethodCannotBeSetToNull ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SetMethodCannotBeSetToNull", PropertyKind.Static, typeof (string));
      property.CreateSetMethod ();
      property.SetMethod = null;
    }

    [Test]
    public void ImplementWithBackingFieldStatic ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ( "StaticProperty", PropertyKind.Static, typeof (string));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();

      FieldInfo backingField = type.GetField ("_fieldForStaticProperty");
      Assert.IsNotNull (backingField);
      Assert.IsTrue (backingField.IsStatic);

      SetPropertyValue ("test", type, property);
      Assert.AreEqual ("test", backingField.GetValue (null));

      backingField.SetValue (null, "yup");

      Assert.AreEqual ("yup", GetPropertyValue (type, property));
    }

    [Test]
    public void ImplementWithBackingFieldInstance ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("InstanceProperty", PropertyKind.Instance, typeof (string));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      object instance = BuildInstance ();
      Type type = instance.GetType();

      FieldInfo backingField = type.GetField ("_fieldForInstanceProperty");
      Assert.IsNotNull (backingField);
      Assert.IsFalse (backingField.IsStatic);

      SetPropertyValue ("what you see", instance, property);
      Assert.AreEqual ("what you see", backingField.GetValue (instance));

      backingField.SetValue (instance, "is what you get");

      Assert.AreEqual ("is what you get", GetPropertyValue (instance, property));
    }

    [Test]
    public void ImplementWithBackingFieldWithoutMethods ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("PropertyWithoutAccessors", PropertyKind.Instance, typeof (string));

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);

      property.ImplementWithBackingField ();

      Assert.IsNull (property.GetMethod);
      Assert.IsNull (property.SetMethod);
    }

    [Test]
    public void CreateGetMethodStatic ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateGetMethodStatic", PropertyKind.Static, typeof (string), new Type[] {typeof (int)}, PropertyAttributes.None);

      Assert.IsNull (property.GetMethod);
      var method = property.CreateGetMethod ();
      Assert.IsTrue (method.MethodBuilder.IsStatic);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int) }));
      Assert.AreEqual (typeof (string), method.ReturnType);
    }

    [Test]
    public void CreateGetMethodInstance ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateGetMethodStatic", PropertyKind.Instance, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.IsNull (property.GetMethod);
      var method = property.CreateGetMethod ();
      Assert.IsFalse (method.MethodBuilder.IsStatic);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int) }));
      Assert.AreEqual (typeof (string), method.ReturnType);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This property already has a getter method.")]
    public void CreateGetMethodThrowsOnDuplicateMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("CreateGetMethodThrowsOnDuplicateGetMethod", PropertyKind.Instance,
          typeof (string));

      property.CreateGetMethod ();
      property.CreateGetMethod ();
    }

    [Test]
    public void CreateSetMethodStatic ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateSetMethodStatic", PropertyKind.Static, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.IsNull (property.SetMethod);
      var method = property.CreateSetMethod ();
      Assert.IsTrue (method.MethodBuilder.IsStatic);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
      Assert.AreEqual (typeof (void), method.ReturnType);
    }

    [Test]
    public void CreateSetMethodInstance ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateSetMethodStatic", PropertyKind.Instance, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.IsNull (property.SetMethod);
      var method = property.CreateSetMethod ();
      Assert.IsFalse (method.MethodBuilder.IsStatic);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
      Assert.AreEqual (typeof (void), method.ReturnType);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This property already has a setter method.")]
    public void CreateSetMethodThrowsOnDuplicateMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("CreateSetMethodThrowsOnDuplicateMethod", PropertyKind.Instance,
          typeof (string));

      property.CreateSetMethod ();
      property.CreateSetMethod ();
    }

    [Test]
    public void AddCustomAttribute ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("AddCustomAttribute", PropertyKind.Static, typeof (string));
      property.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      Type type = _classEmitter.BuildType ();
      Assert.IsTrue (GetProperty (type, property).IsDefined (typeof (SimpleAttribute), false));
      Assert.AreEqual (1, GetProperty (type, property).GetCustomAttributes (false).Length);
      Assert.AreEqual (new SimpleAttribute(), GetProperty (type, property).GetCustomAttributes (false)[0]);
    }
  }
}
