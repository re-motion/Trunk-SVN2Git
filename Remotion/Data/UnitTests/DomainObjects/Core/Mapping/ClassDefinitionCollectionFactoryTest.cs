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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionFactoryTest 
  {
    private ClassDefinitionCollection _classDefinitions;
    private ReflectionBasedNameResolver _nameResolver;

    [SetUp]
    public void SetUp ()
    {
      _classDefinitions = new ClassDefinitionCollection ();
      _nameResolver = new ReflectionBasedNameResolver();
    }

    [Test]
    public void GetClassDefinition_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithMixedProperties), _nameResolver);
      
      var result = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs(typeof (ClassWithMixedProperties)));
      Assert.That (result.BaseClass, Is.Null);
    }

    [Test]
    public void GetClassDefinition_ForBaseClass_WithoutStorageGroupAttribute ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithoutStorageGroupWithMixedProperties), _nameResolver);
      
      var result = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);
      
      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (ClassWithoutStorageGroupWithMixedProperties)));
      Assert.That (result.BaseClass, Is.Null);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _nameResolver);
      
      var result = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs(typeof (DerivedClassWithMixedProperties)));
      Assert.That (result.BaseClass, Is.Not.Null);
      Assert.That (result.BaseClass.ClassType, Is.SameAs(typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithoutStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties), _nameResolver);
      
      var result = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);
      
      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (DerivedClassWithoutStorageGroupWithMixedProperties)));
      Assert.That (result.BaseClass, Is.Not.Null);
      Assert.That (result.BaseClass.ClassType, Is.SameAs (typeof (ClassWithoutStorageGroupWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _nameResolver);
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition ();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _nameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();

      var expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ReflectionBasedClassDefinition actual = ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector.Type, classReflector.NameResolver, classReflector);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForClassesWithSameClassID ()
    {
      Type type1 = GetTypeFromDomainWithErrors ("ClassWithSameClassID");
      Type type2 = GetTypeFromDomainWithErrors ("OtherClassWithSameClassID");

      var classReflector1 = new ClassReflector (type1, _nameResolver);
      var classReflector2 = new ClassReflector (type2, _nameResolver);

      ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector1.Type, classReflector1.NameResolver, classReflector1);
      try
      {
        ClassDefinitionCollectionFactory.GetClassDefinition (_classDefinitions, classReflector2.Type, classReflector2.NameResolver, classReflector2);

        Assert.Fail ("exception expected");
      }
      catch (MappingException ex)
      {
        var expectedMessage = string.Format (
            "Class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithSameClassID' "
            + "and 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.OtherClassWithSameClassID' "
            + "both have the same class ID 'DefinedID'. Use the ClassIDAttribute to define unique IDs for these classes. "
            + "The assemblies involved are '{0}' and '{1}'.",
            GetType ().Assembly.GetName ().FullName,
            GetType ().Assembly.GetName ().FullName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }
    
    private ReflectionBasedClassDefinition CreateClassWithMixedPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithMixedProperties",
          "ClassWithMixedProperties",
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub)),
          typeof (ClassWithMixedProperties),
          false);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DerivedClassWithMixedProperties",
          "DerivedClassWithMixedProperties",
          new UnitTestStorageProviderStubDefinition("DefaultStorageProvider", typeof(UnitTestStorageObjectFactoryStub)),
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition ());
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      
      return classDefinition;
    }

    private static Type GetTypeFromDomainWithErrors (string typename)
    {
      var type = Assembly.GetExecutingAssembly ().GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors." + typename,
          true,
          false);

      Assert.That (type, Is.Not.Null, "Type '{0}' could not be found in domain with errors.", typename);

      return type;
    }

  }
}