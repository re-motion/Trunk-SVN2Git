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
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DataManagement
{
  [TestFixture]
  public class PropertyValueCollectionTest : StandardMappingTest
  {
    private PropertyValueCollection _collection;

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new PropertyValueCollection ();
    }

    [Test]
    public void Events ()
    {
      PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", typeof (int), 42);
      PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", typeof (string), "Arthur Dent");
      PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", typeof (string), true, null);

      _collection.Add (propertyValue1);
      _collection.Add (propertyValue2);
      _collection.Add (propertyValue3);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (_collection, false);

      _collection["Property 2"].Value = "Zaphod Beeblebrox";

      Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (propertyValue2, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void CancelEvents ()
    {
      PropertyValue propertyValue1 = CreatePropertyValue ("Property 1", typeof (int), 42);
      PropertyValue propertyValue2 = CreatePropertyValue ("Property 2", typeof (string), "Arthur Dent");
      PropertyValue propertyValue3 = CreatePropertyValue ("Property 3", typeof (string), true, null);

      _collection.Add (propertyValue1);
      _collection.Add (propertyValue2);
      _collection.Add (propertyValue3);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (_collection, true);

      try
      {
        _collection["Property 2"].Value = "Zaphod Beeblebrox";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (propertyValue2, eventReceiver.ChangingPropertyValue);
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'DoesNotExist' does not exist.\r\nParameter name: propertyName")]
    public void NonExistingPropertyName ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName 1", typeof (int), 42));
      _collection.Add (CreatePropertyValue ("PropertyName 2", typeof (int), 43));

      PropertyValue propertyValue = _collection["DoesNotExist"];
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'PropertyName' already exists in collection.\r\nParameter name: value")]
    public void DuplicatePropertyNames ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName", typeof (int), 42));
      _collection.Add (CreatePropertyValue ("PropertyName", typeof (int), 43));
    }

    [Test]
    public void PropertyValueInTwoCollections ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", typeof (int), 42);
      PropertyValueCollection collection1 = new PropertyValueCollection ();
      PropertyValueCollection collection2 = new PropertyValueCollection ();

      collection1.Add (value);
      collection2.Add (value);

      PropertyValueContainerEventReceiver receiver1 = new PropertyValueContainerEventReceiver (collection1, false);
      PropertyValueContainerEventReceiver receiver2 = new PropertyValueContainerEventReceiver (collection2, false);

      value.Value = 43;

      Assert.IsNotNull (receiver1.ChangingPropertyValue);
      Assert.IsNotNull (receiver1.ChangedPropertyValue);

      Assert.IsNotNull (receiver2.ChangingPropertyValue);
      Assert.IsNotNull (receiver2.ChangedPropertyValue);
    }

    [Test]
    public void ContainsPropertyValueTrue ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", typeof (int), 42);

      _collection.Add (value);

      Assert.IsTrue (_collection.Contains (value));
    }

    [Test]
    public void ContainsPropertyValueFalse ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName", typeof (int), 42);
      _collection.Add (value);

      PropertyValue copy = CreatePropertyValue ("PropertyName", typeof (int), 42);

      Assert.IsFalse (_collection.Contains (copy));

    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullPropertyValue ()
    {
      _collection.Contains ((PropertyValue) null);
    }

    private PropertyValue CreatePropertyValue (string name, Type propertyType, object value)
    {
      return CreatePropertyValue (name, propertyType, null, value);
    }

    private PropertyValue CreatePropertyValue (string name, Type propertyType, bool? isNullable, object value)
    {
      int? maxLength = (propertyType == typeof (string)) ? (int?) 100 : null;

      ReflectionBasedClassDefinition classDefinition =
          new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false, new List<Type> ());
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, name, name, propertyType, isNullable, maxLength, StorageClass.Persistent);
      return new PropertyValue (definition, value);
    }
  }
}
