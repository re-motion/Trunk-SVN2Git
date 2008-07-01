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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : StandardMappingTest
  {
    private PropertyDefinitionCollection _collection;
    private PropertyDefinition _propertyDefinition;
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false, new List<Type> ());
      _propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "Name", "Name", typeof (string), 100);
      _collection = new PropertyDefinitionCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_propertyDefinition);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void AddEvents ()
    {
      PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
          _collection, false);

      _collection.Add (_propertyDefinition);

      Assert.AreSame (_propertyDefinition, eventReceiver.AddingPropertyDefinition);
      Assert.AreSame (_propertyDefinition, eventReceiver.AddedPropertyDefinition);
    }

    [Test]
    public void CancelAdd ()
    {
      PropertyDefinitionCollectionEventReceiver eventReceiver = new PropertyDefinitionCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Add (_propertyDefinition);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (_propertyDefinition, eventReceiver.AddingPropertyDefinition);
        Assert.AreSame (null, eventReceiver.AddedPropertyDefinition);
      }
    }

    [Test]
    public void PropertyNameIndexer ()
    {
      _collection.Add (_propertyDefinition);
      Assert.AreSame (_propertyDefinition, _collection["Name"]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_propertyDefinition);
      Assert.AreSame (_propertyDefinition, _collection[0]);
    }

    [Test]
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add (_propertyDefinition);
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
      _collection.Add (_propertyDefinition);
      Assert.IsTrue (_collection.Contains (_propertyDefinition));
    }

    [Test]
    public void ContainsPropertyDefinitionFalse ()
    {
      _collection.Add (_propertyDefinition);

      PropertyDefinition copy = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition((ReflectionBasedClassDefinition) _propertyDefinition.ClassDefinition, _propertyDefinition.PropertyName, _propertyDefinition.StorageSpecificName, _propertyDefinition.PropertyType, _propertyDefinition.IsNullable, _propertyDefinition.MaxLength, _propertyDefinition.StorageClass);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_propertyDefinition);

      PropertyDefinitionCollection copiedCollection = new PropertyDefinitionCollection (_collection, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_propertyDefinition, copiedCollection[0]);
    }

    [Test]
    public void ContainsPropertyDefinition ()
    {
      _collection.Add (_propertyDefinition);

      Assert.IsTrue (_collection.Contains (_propertyDefinition));
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
  }
}
