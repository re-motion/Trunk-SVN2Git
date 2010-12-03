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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionTest : MappingReflectionTestBase
  {
    private RelationEndPointDefinitionCollection _collection;
    private ReflectionBasedClassDefinition _classDefinition;
    private RelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;
    private RelationEndPointDefinition _endPoint3;
    private RelationEndPointDefinition _endPoint4;
    private ReflectionBasedPropertyDefinition _propertyDefinition1;
    private ReflectionBasedPropertyDefinition _propertyDefinition2;
    private ReflectionBasedPropertyDefinition _propertyDefinition3;
    private ReflectionBasedPropertyDefinition _propertyDefinition4;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      _propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property1", "Property1", typeof (ObjectID), true, null, StorageClass.Persistent);
      _propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property2", "Property2", typeof(ObjectID),true,null,StorageClass.Persistent);
      _propertyDefinition3 = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property3", "Property3", typeof (ObjectID), true, null, StorageClass.Persistent);
      _propertyDefinition4 = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property4", "Property4", typeof (ObjectID), true, null, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (
          new PropertyDefinitionCollection (new[] { _propertyDefinition1, _propertyDefinition2, _propertyDefinition3, _propertyDefinition4 }, true));
      _endPoint1 = new RelationEndPointDefinition (_classDefinition, "Property1", false);
      _endPoint2 = new RelationEndPointDefinition (_classDefinition, "Property2", false);
      _endPoint3 = new RelationEndPointDefinition (_classDefinition, "Property3", false);
      _endPoint4 = new RelationEndPointDefinition (_classDefinition, "Property4", false);
      _collection = new RelationEndPointDefinitionCollection();
    }

    [Test]
    public void CreateForAllRelationDefinitions_ClassDefinitionWithoutBaseClassDefinition ()
    {
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "Customer", UnitTestDomainStorageProviderDefinition, typeof (Customer), false);
      var relationDefinition = new RelationDefinition ("Test", _endPoint1, _endPoint2);

      _classDefinition.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition }, true));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelations (_classDefinition).ToArray();

      Assert.That (endPoints.Length, Is.EqualTo (2));
      Assert.That (endPoints[0], Is.SameAs (_endPoint1));
      Assert.That (endPoints[1], Is.SameAs (_endPoint2));
    }

    [Test]
    public void CreateForAllRelationDefinitions_ClassDefinitionWithBaseClassDefinition ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Partner", "Partner", UnitTestDomainStorageProviderDefinition, typeof (Partner), false, baseClassDefinition, new Type[0]);

      var relationDefinition1 = new RelationDefinition ("Test", _endPoint1, _endPoint2);
      var relationDefinition2 = new RelationDefinition ("Test", _endPoint3, _endPoint4);

      _classDefinition.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition1 }, true));
      baseClassDefinition.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition2 }, true));
      
      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelations (_classDefinition).ToArray ();

      Assert.That (endPoints.Length, Is.EqualTo (4));
      Assert.That (endPoints[0], Is.SameAs (_endPoint1));
      Assert.That (endPoints[1], Is.SameAs (_endPoint2));
      Assert.That (endPoints[2], Is.SameAs (_endPoint3));
      Assert.That (endPoints[3], Is.SameAs (_endPoint4));
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection.Count, Is.EqualTo (1));
    }

    [Test]
    public void PropertyNameIndexer ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection["Property1"], Is.SameAs (_endPoint1));
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection[0], Is.SameAs (_endPoint1));
    }

    [Test]
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection.Contains ("Property1"), Is.True);
    }

    [Test]
    public void ContainsPropertyNameFalse ()
    {
      Assert.That (_collection.Contains ("UndefinedPropertyName"), Is.False);
    }

    [Test]
    public void ContainsRelationEndPointDefinitionTrue ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection.Contains (_endPoint1), Is.True);
    }

    [Test]
    public void ContainsRelationEndPointDefinitionFalse ()
    {
      _collection.Add (_endPoint1);

      Assert.That (_collection.Contains (_endPoint2), Is.False);
    }

    [Test]
    public void CopyConstructor ()
    {
      var copiedCollection = new RelationEndPointDefinitionCollection (new[] { _endPoint1 }, false);

      Assert.That (copiedCollection.Count, Is.EqualTo (1));
      Assert.That (copiedCollection[0], Is.SameAs (_endPoint1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullRelationEndPointDefinitions ()
    {
      _collection.Contains ((IRelationEndPointDefinition) null);
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That (_collection.IsReadOnly, Is.False);

      _collection.SetReadOnly ();

      Assert.That (_collection.IsReadOnly, Is.True);
    }

    [Test]
    public void GetEnumerator ()
    {
      _collection.Add (_endPoint1);

      IEnumerator<IRelationEndPointDefinition> enumerator = _collection.GetEnumerator ();

      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_endPoint1));
      Assert.That (enumerator.MoveNext (), Is.False);
    }
  }
}