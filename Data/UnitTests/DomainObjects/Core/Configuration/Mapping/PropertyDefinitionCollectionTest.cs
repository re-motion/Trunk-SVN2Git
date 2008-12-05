// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : StandardMappingTest
  {
    private PropertyDefinitionCollection _collection;
    private PropertyDefinition _propertyDefinition1;
    private PropertyDefinition _propertyDefinition2;
    private PropertyDefinition _propertyDefinitionNonPersisted;
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);
      _propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (_classDefinition, "Name", "Name", typeof (string), 100);
      _propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (_classDefinition, "Name2", "Name", typeof (string), 100);
      _propertyDefinitionNonPersisted = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (_classDefinition, "Name3", "Name", typeof (string), null, 100, StorageClass.Transaction);
      _collection = new PropertyDefinitionCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_propertyDefinition1);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void AddEvents ()
    {
      PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
          _collection, false);

      _collection.Add (_propertyDefinition1);

      Assert.AreSame (_propertyDefinition1, eventReceiver.AddingPropertyDefinition);
      Assert.AreSame (_propertyDefinition1, eventReceiver.AddedPropertyDefinition);
    }

    [Test]
    public void CancelAdd ()
    {
      PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Add (_propertyDefinition1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (_propertyDefinition1, eventReceiver.AddingPropertyDefinition);
        Assert.AreSame (null, eventReceiver.AddedPropertyDefinition);
      }
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

      PropertyDefinition copy = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition((ReflectionBasedClassDefinition) _propertyDefinition1.ClassDefinition, _propertyDefinition1.PropertyName, _propertyDefinition1.StorageSpecificName, _propertyDefinition1.PropertyType, _propertyDefinition1.IsNullable, _propertyDefinition1.MaxLength, _propertyDefinition1.StorageClass);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_propertyDefinition1);

      PropertyDefinitionCollection copiedCollection = new PropertyDefinitionCollection (_collection, false);

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
    public void ContainsColumName ()
    {
      _collection.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (int)));

      Assert.IsTrue (_collection.ContainsColumnName ("ColumnName"));
    }

    [Test]
    public void InitializeWithClassDefinition ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      PropertyDefinitionCollection collection = new PropertyDefinitionCollection (orderDefinition);
      Assert.AreSame (orderDefinition, collection.ClassDefinition);
    }

    [Test]
    public void GetAllPersistent ()
    {
      _collection.Add (_propertyDefinition1);
      _collection.Add (_propertyDefinition2);
      _collection.Add (_propertyDefinitionNonPersisted);

      Assert.That (_collection.GetAllPersistent().ToArray(), Is.EqualTo(new[] {_propertyDefinition1, _propertyDefinition2}));
    }
  }
}
