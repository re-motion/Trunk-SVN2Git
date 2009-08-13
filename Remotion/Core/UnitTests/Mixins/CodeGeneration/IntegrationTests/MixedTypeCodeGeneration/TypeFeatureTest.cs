// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
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

      Assert.IsNotNull (ObjectFactory.Create<BaseType1> (ParamList.Empty));
      Assert.IsNotNull (ObjectFactory.Create<BaseType2> (ParamList.Empty));
      Assert.IsNotNull (ObjectFactory.Create<BaseType3> (ParamList.Empty));
      Assert.IsNotNull (ObjectFactory.Create<BaseType4> (ParamList.Empty));
      Assert.IsNotNull (ObjectFactory.Create<BaseType5> (ParamList.Empty));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with the following signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated1 ()
    {
      ObjectFactory.Create<ClassWithCtors> (ParamList.Empty);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with the following signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated2 ()
    {
      ObjectFactory.Create<ClassWithCtors> (ParamList.Create (2.0));
    }

    [Test]
    public void ConstructorsAreReplicated3 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create (3));
      Assert.AreEqual (3, c.O);
    }

    [Test]
    public void ConstructorsAreReplicated4 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create ("a"));
      Assert.AreEqual ("a", c.O);
    }

    [Test]
    public void ConstructorsAreReplicated5 ()
    {
      var nullMixin = new NullMixin ();
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create ("a"), nullMixin);
      Assert.AreEqual ("a", c.O);
      Assert.AreSame (nullMixin, Mixin.Get<NullMixin> (c));
    }

    [Test]
    public void GeneratedTypeHasMixedTypeAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixedTypeAttribute), false));

      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void GeneratedTypeHasDebuggerDisplayAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (generatedType.IsDefined (typeof (DebuggerDisplayAttribute), false));

      var attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void DebuggerDisplayAttribute_NotAddedIfExistsViaMixin ()
    {
      Type generatedType = CreateMixedType (typeof (NullTarget), typeof (MixinAddingDebuggerDisplay));
      var attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.AreEqual (1, attributes.Length);
      Assert.AreEqual ("Y", attributes[0].Value);
    }

    [Test]
    public void MixedTypeAttribute_GetsClassContext ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      ClassContext context = attributes[0].GetClassContext ();
      Assert.AreEqual (context, TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).ConfigurationContext);
    }

    [Test]
    public void MixedTypeAttribute_GetsOrderedMixinTypes ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType7));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);

      // see MixinDependencySortTest.MixinDefinitionsAreSortedCorrectlySmall
      Assert.That (attributes[0].OrderedMixinTypes, Is.EqualTo (new[] { 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5) }));
    }

    [Test]
    public void MixedTypeAttribute_GetsClosedGenericMixinTypes ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);

      Assert.That (attributes[0].OrderedMixinTypes, List.Contains (typeof (BT3Mixin3<BaseType3, IBaseType33>)));
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
      var builder = new ConcreteTypeBuilder {Scope = SavedTypeBuilder.Scope};
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.TypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

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
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.TypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

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
      MethodInfo[] abstractMethods = Array.FindAll (concreteType.GetMethods (), method => method.IsAbstract);
      string[] abstractMethodNames = Array.ConvertAll (abstractMethods, method => method.Name);
      Assert.That (abstractMethodNames, Is.EquivalentTo (new[] { "VirtualMethod", "get_VirtualProperty", "set_VirtualProperty",
          "add_VirtualEvent", "remove_VirtualEvent" }));
    }

    [Test]
    public void DeserializationConstructorGeneratedEvenIfBaseNotISerializable ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1));
      Assert.IsNull (typeof (BaseType1).GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, new[] {typeof (SerializationInfo), typeof (StreamingContext)}, null));
      Assert.IsNotNull (concreteType.GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, new[] { typeof (SerializationInfo), typeof (StreamingContext) }, null));
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

    [Test]
    public void IMixinTarget ()
    {
      var mixinTarget = (IMixinTarget) ObjectFactory.Create<BaseType1> (ParamList.Empty);

      Assert.That (mixinTarget.Configuration, Is.Not.Null);
      Assert.That (mixinTarget.Configuration, Is.SameAs (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1))));

      object[] mixins = mixinTarget.Mixins;
      Assert.That (mixins, Is.Not.Null);
      Assert.That (mixins.Length, Is.EqualTo (mixinTarget.Configuration.Mixins.Count));
      Assert.That (mixins[0], Is.InstanceOfType (mixinTarget.Configuration.Mixins[0].Type));
    }
  }
}
