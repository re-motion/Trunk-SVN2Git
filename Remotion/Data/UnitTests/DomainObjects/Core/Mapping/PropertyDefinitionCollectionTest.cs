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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : MappingReflectionTestBase
  {
    private PropertyDefinitionCollection _collection;
    private PropertyDefinition _propertyDefinition1;
    private PropertyDefinition _propertyDefinition2;
    private PropertyDefinition _propertyDefinitionNonPersisted;
    private ClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      _propertyDefinition1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _classDefinition, "Name", "Name", StorageClass.Persistent);
      _propertyDefinition2 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _classDefinition, "Name2", "Name", StorageClass.Persistent);
      _propertyDefinitionNonPersisted = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _classDefinition, "Name3", "Name", StorageClass.Transaction);
      _collection = new PropertyDefinitionCollection();
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsFalse ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classDefinition, "Test", "Test", StorageClass.Persistent);

      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      
      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties (_classDefinition, false);

      Assert.That (propertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (propertyDefinitions.IsReadOnly, Is.False);
      Assert.That (propertyDefinitions[0], Is.SameAs (propertyDefinition));
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsTrue ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classDefinition, "Test", "Test", StorageClass.Persistent);

      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));

      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties (_classDefinition, true);

      Assert.That (propertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (propertyDefinitions.IsReadOnly, Is.True);
      Assert.That (propertyDefinitions[0], Is.SameAs (propertyDefinition));
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithBaseClassDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Company));
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Partner), baseClassDefinition);

      var propertyDefinitionInBaseClass = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          baseClassDefinition, "Property1", "Property1", StorageClass.Persistent);
      var propertyDefinitionInDerivedClass = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          classDefinition, "Property2", "Property2", StorageClass.Persistent);

      baseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinitionInBaseClass }, true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinitionInDerivedClass }, true));
      
      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties (classDefinition, false);

      Assert.That (propertyDefinitions.Count, Is.EqualTo (2));
      Assert.That (propertyDefinitions[0], Is.SameAs (propertyDefinitionInDerivedClass));
      Assert.That (propertyDefinitions[1], Is.SameAs (propertyDefinitionInBaseClass));
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void PropertyNameIndexer ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.AreSame (_propertyDefinition1, _collection["Name"]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.AreSame (_propertyDefinition1, _collection[0]);
    }

    [Test]
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.IsTrue (_collection.Contains ("Name"));
    }

    [Test]
    public void ContainsPropertyNameFalse ()
    {
      Assert.IsFalse (_collection.Contains ("UndefinedPropertyName"));
    }

    [Test]
    public void ContainsPropertyDefinitionTrue ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.IsTrue (_collection.Contains (_propertyDefinition1));
    }

    [Test]
    public void ContainsPropertyDefinitionFalse ()
    {
      _collection.Add (_propertyDefinition1);

      var copy =
          new PropertyDefinition (
              _propertyDefinition1.ClassDefinition,
              _propertyDefinition1.PropertyInfo,
              _propertyDefinition1.PropertyName,
              _propertyDefinition1.IsObjectID,
              _propertyDefinition1.IsNullable,
              _propertyDefinition1.MaxLength,
              _propertyDefinition1.StorageClass);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      var copiedCollection = new PropertyDefinitionCollection (new[] { _propertyDefinition1 }, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_propertyDefinition1, copiedCollection[0]);
    }

    [Test]
    public void ContainsPropertyDefinition ()
    {
      _collection.Add (_propertyDefinition1);

      Assert.IsTrue (_collection.Contains (_propertyDefinition1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullPropertyDefinition ()
    {
      _collection.Contains ((PropertyDefinition) null);
    }

    [Test]
    public void GetAllPersistent ()
    {
      _collection.Add (_propertyDefinition1);
      _collection.Add (_propertyDefinition2);
      _collection.Add (_propertyDefinitionNonPersisted);

      Assert.That (_collection.GetAllPersistent().ToArray(), Is.EqualTo (new[] { _propertyDefinition1, _propertyDefinition2 }));
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That (_collection.IsReadOnly, Is.False);

      _collection.SetReadOnly();

      Assert.That (_collection.IsReadOnly, Is.True);
    }

    [Test]
    public void GetEnumerator ()
    {
      _collection.Add (_propertyDefinition1);

      IEnumerator<PropertyDefinition> enumerator = _collection.GetEnumerator();
      
      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_propertyDefinition1));
      Assert.That (enumerator.MoveNext(), Is.False);
    }
  }
}