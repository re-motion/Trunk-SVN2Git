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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionTest : MappingReflectionTestBase
  {
    private RelationEndPointDefinitionCollection _collection;
    private ClassDefinition _classDefinition;
    private RelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;
    private PropertyDefinition _propertyDefinition1;
    private PropertyDefinition _propertyDefinition2;
    private PropertyDefinition _propertyDefinition3;
    private PropertyDefinition _propertyDefinition4;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      _propertyDefinition1 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property1", "Property1", typeof (DomainObject), StorageClass.Persistent);
      _propertyDefinition2 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property2", "Property2", typeof (DomainObject), StorageClass.Persistent);
      _propertyDefinition3 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property3", "Property3", typeof (DomainObject), StorageClass.Persistent);
      _propertyDefinition4 = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Property4", "Property4", typeof (DomainObject), StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (
          new PropertyDefinitionCollection (new[] { _propertyDefinition1, _propertyDefinition2, _propertyDefinition3, _propertyDefinition4 }, true));
      _endPoint1 = new RelationEndPointDefinition (_propertyDefinition1, false);
      _endPoint2 = new RelationEndPointDefinition (_propertyDefinition2, false);
      _collection = new RelationEndPointDefinitionCollection();
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsFalse ()
    {
      _classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { _endPoint1, _endPoint2 }, true));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints (_classDefinition, false);

      Assert.That (endPoints.Count, Is.EqualTo (2));
      Assert.That (endPoints.IsReadOnly, Is.False);
      Assert.That (endPoints[0], Is.SameAs (_endPoint1));
      Assert.That (endPoints[1], Is.SameAs (_endPoint2));
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsTrue ()
    {
      _classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { _endPoint1, _endPoint2 }, false));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints (_classDefinition, true);

      Assert.That (endPoints.Count, Is.EqualTo (2));
      Assert.That (endPoints.IsReadOnly, Is.True);
      Assert.That (endPoints[0], Is.SameAs (_endPoint1));
      Assert.That (endPoints[1], Is.SameAs (_endPoint2));
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithBaseClassDefinition ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
     var basedPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          baseClassDefinition, "Property1", "Property1", typeof (DomainObject), StorageClass.Persistent);
     baseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { basedPropertyDefinition }, true));
      var derivedClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Partner", "Partner", UnitTestDomainStorageProviderDefinition, typeof (Partner), false, baseClassDefinition, new Type[0]);
     var derivedPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          derivedClassDefinition, "Property2", "Property2", typeof (DomainObject),  StorageClass.Persistent);
      derivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { derivedPropertyDefinition }, true));

      var endPoint1 = new RelationEndPointDefinition (basedPropertyDefinition, false);
      var endPoint2 = new RelationEndPointDefinition (derivedPropertyDefinition, false);

      baseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPoint1 }, true));
      derivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPoint2 }, true));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints (derivedClassDefinition, true);
       
      Assert.That (endPoints.Count, Is.EqualTo (2));
      Assert.That (endPoints[0], Is.SameAs (endPoint2));
      Assert.That (endPoints[1], Is.SameAs (endPoint1));
    }
    
    [Test]
    public void Add ()
    {
      _collection.Add (_endPoint1);
      Assert.That (_collection.Count, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "End points without property name cannot be added to this collection.")]
    public void Add_PropertyNameIsNull ()
    {
      var endPoint = new AnonymousRelationEndPointDefinition (_classDefinition);
      _collection.Add (endPoint);
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