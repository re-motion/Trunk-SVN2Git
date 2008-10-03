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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Uses (typeof (NullMixin))]
    public class ClassWithCtors
    {
      public object O;

      public ClassWithCtors (int i)
      {
        O = i;
      }

      public ClassWithCtors (string s)
      {
        O = s;
      }
    }

    [Test]
    public void GeneratedTypeIsAssignableButDifferent ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.IsTrue (typeof (BaseType1).IsAssignableFrom (t));
      Assert.AreNotEqual (typeof (BaseType1), t);

      t = TypeFactory.GetConcreteType (typeof (BaseType2));
      Assert.IsTrue (typeof (BaseType2).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (typeof (BaseType3).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType4));
      Assert.IsTrue (typeof (BaseType4).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType5));
      Assert.IsTrue (typeof (BaseType5).IsAssignableFrom (t));

      Assert.IsNotNull (ObjectFactory.Create<BaseType1> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType2> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType3> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType4> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType5> ());
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated1 ()
    {
      ObjectFactory.Create<ClassWithCtors> ().With ();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated2 ()
    {
      ObjectFactory.Create<ClassWithCtors> ().With (2.0);
    }

    [Test]
    public void ConstructorsAreReplicated3 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With (3);
      Assert.AreEqual (3, c.O);
    }

    [Test]
    public void ConstructorsAreReplicated4 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With ("a");
      Assert.AreEqual ("a", c.O);
    }

    [Test]
    public void ConstructorsAreReplicated5 ()
    {
      NullMixin nullMixin = new NullMixin ();
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> (nullMixin).With ("a");
      Assert.AreEqual ("a", c.O);
      Assert.AreSame (nullMixin, Mixin.Get<NullMixin> (c));
    }

    [Test]
    public void GeneratedTypeHasMixedTypeAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixedTypeAttribute), false));

      ConcreteMixedTypeAttribute[] attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void GeneratedTypeHasDebuggerDisplayAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (generatedType.IsDefined (typeof (DebuggerDisplayAttribute), false));

      DebuggerDisplayAttribute[] attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void DebuggerDisplayAttribute_NotAddedIfExistsViaMixin ()
    {
      Type generatedType = CreateMixedType (typeof (NullTarget), typeof (MixinAddingDebuggerDisplay));
      DebuggerDisplayAttribute[] attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.AreEqual (1, attributes.Length);
      Assert.AreEqual ("Y", attributes[0].Value);
    }

    [Test]
    public void MixedTypeAttributeCanBeUsedToGetClassContext ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      ConcreteMixedTypeAttribute[] attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      ClassContext context = attributes[0].GetClassContext ();
      Assert.AreEqual (context, TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).ConfigurationContext);
    }

    [Test]
    public void MixedTypeAttributeCanBeUsedToGetTargetClassDefinition ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      ConcreteMixedTypeAttribute[] attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      TargetClassDefinition definition = attributes[0].GetTargetClassDefinition (TargetClassDefinitionCache.Current);
      Assert.AreSame (definition, TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)));
    }

    [Test]
    public void GeneratedTypeHasTypeInitializer ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsNotNull (generatedType.GetConstructor (BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated ()
    {
      MockRepository repository = new MockRepository ();
      INameProvider nameProviderMock = repository.StrictMock<INameProvider> ();
      ConcreteTypeBuilder.Current.TypeNameProvider = nameProviderMock;

      TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      Expect.Call (nameProviderMock.GetNewTypeName (definition)).Return ("Foo");

      repository.ReplayAll ();

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.AreEqual ("Foo", generatedType.FullName);

      repository.VerifyAll ();
    }

    [Test]
    public void NamesOfNestedTypesAreFlattened ()
    {
      MockRepository repository = new MockRepository ();
      INameProvider nameProviderMock = repository.StrictMock<INameProvider> ();
      ConcreteTypeBuilder.Current.TypeNameProvider = nameProviderMock;

      TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      Expect.Call (nameProviderMock.GetNewTypeName (definition)).Return ("Foo+Bar");

      repository.ReplayAll ();

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.AreEqual ("Foo/Bar", generatedType.FullName);

      repository.VerifyAll ();
    }

    [Test]
    public void AbstractBaseTypesLeadToAbstractConcreteTypes ()
    {
      Type concreteType = CreateMixedType (typeof (AbstractBaseType), typeof (MixinOverridingClassMethod));
      Assert.IsNotNull (concreteType);
      Assert.IsTrue (concreteType.IsAbstract);
      MethodInfo[] abstractMethods = Array.FindAll (concreteType.GetMethods (), delegate (MethodInfo method) { return method.IsAbstract; });
      string[] abstractMethodNames = Array.ConvertAll<MethodInfo, string> (abstractMethods, delegate (MethodInfo method) { return method.Name; });
      Assert.That (abstractMethodNames, Is.EquivalentTo (new string[] { "VirtualMethod", "get_VirtualProperty", "set_VirtualProperty",
          "add_VirtualEvent", "remove_VirtualEvent" }));
    }

    [Test]
    public void DeserializationConstructorGeneratedEvenIfBaseNotISerializable ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1));
      Assert.IsNull (typeof (BaseType1).GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, new Type[] {typeof (SerializationInfo), typeof (StreamingContext)}, null));
      Assert.IsNotNull (concreteType.GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, new Type[] { typeof (SerializationInfo), typeof (StreamingContext) }, null));
    }

    [Test]
    public void CopiedAttributesAreNotReplicated ()
    {
      Type concreteType = CreateMixedType (typeof (ClassWithCopyCustomAttributes));
      Assert.AreNotSame (typeof (ClassWithCopyCustomAttributes), concreteType);
      Assert.IsEmpty (concreteType.GetCustomAttributes (typeof (SampleCopyTemplateAttribute), true));
    }

    [Test]
    public void ValueTypeMixin ()
    {
      CreateMixedType (typeof (BaseType1), typeof (ValueTypeMixin));
    }

    [Test]
    public void ExtensionsFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (concreteType.GetField ("__extensions", BindingFlags.NonPublic | BindingFlags.Instance).Attributes, Is.EqualTo (FieldAttributes.Private));
    }

    [Test]
    public void FirstFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (concreteType.GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).Attributes, Is.EqualTo (FieldAttributes.Private));
    }

    [Test]
    public void ConfigurationFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (concreteType.GetField ("__configuration", BindingFlags.NonPublic | BindingFlags.Static).Attributes, Is.EqualTo (FieldAttributes.Private | FieldAttributes.Static));
    }
  }
}
