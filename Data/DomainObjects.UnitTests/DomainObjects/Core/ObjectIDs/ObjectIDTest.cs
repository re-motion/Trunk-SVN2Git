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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.ObjectIDs
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [DBTable]
    private class InvalidDomainObject : DomainObject
    {
    }

    [Test]
    public void SerializeStringValue ()
    {
      ObjectID id = new ObjectID ("Official", "Arthur Dent");
      Assert.AreEqual ("Official|Arthur Dent|System.String", id.ToString ());
    }

    [Test]
    public void SerializeInt32Value ()
    {
      ObjectID id = new ObjectID ("Official", 42);
      Assert.AreEqual ("Official|42|System.Int32", id.ToString ());
    }

    [Test]
    public void SerializeGuidValue ()
    {
      ObjectID id = new ObjectID ("Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.AreEqual ("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid", id.ToString ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
    public void EscapedDelimiterPlaceholderInValue ()
    {
      ObjectID id = new ObjectID ("Official", "Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
    }

    [Test]
    public void DeserializeStringValue ()
    {
      string idString = "Official|Arthur Dent|System.String";
      ObjectID id = ObjectID.Parse (idString);

      Assert.AreEqual ("UnitTestStorageProviderStub", id.StorageProviderID);
      Assert.AreEqual ("Official", id.ClassID);
      Assert.AreEqual (typeof (string), id.Value.GetType ());
      Assert.AreEqual ("Arthur Dent", id.Value);
    }

    [Test]
    public void DeserializeInt32Value ()
    {
      string idString = "Official|42|System.Int32";
      ObjectID id = ObjectID.Parse (idString);

      Assert.AreEqual ("UnitTestStorageProviderStub", id.StorageProviderID);
      Assert.AreEqual ("Official", id.ClassID);
      Assert.AreEqual (typeof (int), id.Value.GetType ());
      Assert.AreEqual (42, id.Value);
    }

    [Test]
    public void DeserializeGuidValue ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectID id = ObjectID.Parse (idString);

      Assert.AreEqual ("TestDomain", id.StorageProviderID);
      Assert.AreEqual ("Order", id.ClassID);
      Assert.AreEqual (typeof (Guid), id.Value.GetType ());
      Assert.AreEqual (new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"), id.Value);
    }

    [Test]
    [ExpectedException (typeof (FormatException),
        ExpectedMessage = "Serialized ObjectID 'Order|5d09030c-25"
         + "c2-4735-b514-46333bd28ac8|System.Guid|Zaphod' is not correctly formatted.")]
    public void ObjectIDStringWithTooManyParts ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid|Zaphod";
      ObjectID id = ObjectID.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Type 'System.Double' is not supported.")]
    public void ObjectIDStringWithInvalidValueType ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";
      ObjectID id = ObjectID.Parse (idString);
    }

    [Test]
    public void HashCode ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("Official", 42);
      ObjectID id3 = new ObjectID ("Official", 41);

      Assert.IsTrue (id1.GetHashCode () == id2.GetHashCode ());
      Assert.IsFalse (id1.GetHashCode () == id3.GetHashCode ());
      Assert.IsFalse (id2.GetHashCode () == id3.GetHashCode ());
    }

    [Test]
    public void TestEqualsForClassID ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("Official", 42);
      ObjectID id3 = new ObjectID ("SpecialOfficial", 42);

      Assert.IsTrue (id1.Equals (id2));
      Assert.IsFalse (id1.Equals (id3));
      Assert.IsFalse (id2.Equals (id3));
      Assert.IsTrue (id2.Equals (id1));
      Assert.IsFalse (id3.Equals (id1));
      Assert.IsFalse (id3.Equals (id2));
    }

    [Test]
    public void TestEqualsForValue ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("Official", 42);
      ObjectID id3 = new ObjectID ("Official", 41);

      Assert.IsTrue (id1.Equals (id2));
      Assert.IsFalse (id1.Equals (id3));
      Assert.IsFalse (id2.Equals (id3));
      Assert.IsTrue (id2.Equals (id1));
      Assert.IsFalse (id3.Equals (id1));
      Assert.IsFalse (id3.Equals (id2));
    }

    [Test]
    public void EqualsWithOtherType ()
    {
      ObjectID id = new ObjectID ("Official", 42);
      Assert.IsFalse (id.Equals (new ObjectIDTest ()));
      Assert.IsFalse (id.Equals (42));
    }

    [Test]
    public void EqualsWithNull ()
    {
      ObjectID id = new ObjectID ("Official", 42);
      Assert.IsFalse (id.Equals (null));
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("Official", 42);

      Assert.IsTrue (id1 == id2);
      Assert.IsFalse (id1 != id2);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("SpecialOfficial", 1);

      Assert.IsFalse (id1 == id2);
      Assert.IsTrue (id1 != id2);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = id1;

      Assert.IsTrue (id1 == id2);
      Assert.IsFalse (id1 != id2);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
      Assert.IsTrue ((ObjectID) null == (ObjectID) null);
      Assert.IsFalse ((ObjectID) null != (ObjectID) null);

    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      ObjectID id2 = new ObjectID ("Official", 42);

      Assert.IsFalse (null == id2);
      Assert.IsTrue (null != id2);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);

      Assert.IsFalse (id1 == null);
      Assert.IsTrue (id1 != null);
    }

    [Test]
    public void StaticEquals ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("Official", 42);

      Assert.IsTrue (ObjectID.Equals (id1, id2));
    }

    [Test]
    public void StaticNotEquals ()
    {
      ObjectID id1 = new ObjectID ("Official", 42);
      ObjectID id2 = new ObjectID ("SpecialOfficial", 1);

      Assert.IsFalse (ObjectID.Equals (id1, id2));
    }

    [Test]
    public void InitializeWithClassID ()
    {
      Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      ObjectID id = new ObjectID ("Order", value);

      Assert.AreEqual ("TestDomain", id.StorageProviderID);
      Assert.AreEqual ("Order", id.ClassID);
      Assert.AreEqual (value, id.Value);
    }

    [Test]
    public void InitializeWithClassType ()
    {
      Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      ObjectID id = new ObjectID (typeof (Order), value);

      Assert.AreEqual ("TestDomain", id.StorageProviderID);
      Assert.AreEqual ("Order", id.ClassID);
      Assert.AreEqual (value, id.Value);
    }

    [Test]
    public void InitializeWithClassDefinition ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      ObjectID id = new ObjectID (orderDefinition, value);

      Assert.AreEqual (orderDefinition.StorageProviderID, id.StorageProviderID);
      Assert.AreEqual (orderDefinition.ID, id.ClassID);
      Assert.AreEqual (value, id.Value);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InitializeWithUnknownClassDefinitionID ()
    {
      ReflectionBasedClassDefinition unknownDefinition = new ReflectionBasedClassDefinition ("UnknownClass", "UnknownTable", "TestDomain", typeof (Order), false, new List<Type>());
      Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      ObjectID id = new ObjectID (unknownDefinition, value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The ClassID 'Order' and the ClassType 'Remotion.Data.UnitTests.DomainObjects.Core.ObjectIDs.ObjectIDTest+InvalidDomainObject'"
        + " do not refer to the same ClassDefinition in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithUnknownClassDefinitionType ()
    {
      ReflectionBasedClassDefinition unknownDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (InvalidDomainObject), false, new List<Type>());
      Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      ObjectID id = new ObjectID (unknownDefinition, value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void InitializeWithEmptyGuid ()
    {
      ObjectID id = new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], Guid.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void InitializeWithEmptyString ()
    {
      ObjectID id = new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], string.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The ClassID 'Order' and the ClassType 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer'"
        + " do not refer to the same ClassDefinition in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithInvalidClassDefinition ()
    {
      ReflectionBasedClassDefinition invalidDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Customer), false, new List<Type>());
      ObjectID id = new ObjectID (invalidDefinition, Guid.NewGuid ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provided ClassDefinition 'Order' is not the same reference as the ClassDefinition found in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithClassDefinitionNotPartOfMappingConfiguration ()
    {
      ReflectionBasedClassDefinition invalidDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false, new List<Type>());
      ObjectID id = new ObjectID (invalidDefinition, Guid.NewGuid ());
    }

    [Test]
    [ExpectedException (typeof (IdentityTypeNotSupportedException))]
    public void InitializeWithInvalidIdentityType ()
    {
      ObjectID id = new ObjectID ("Order", 1);
    }

    [Test]
    public void InitializeWithGuid ()
    {
      Guid idValue = Guid.NewGuid ();
      ObjectID id = new ObjectID ("Official", idValue);

      Assert.AreEqual ("Official", id.ClassID);
      Assert.AreEqual (idValue, id.Value);
    }

    [Test]
    public void InitializeWithInt32 ()
    {
      ObjectID id = new ObjectID ("Official", 1);

      Assert.AreEqual ("Official", id.ClassID);
      Assert.AreEqual (1, id.Value);
    }

    [Test]
    public void InitializeWithString ()
    {
      ObjectID id = new ObjectID ("Official", "StringValue");

      Assert.AreEqual ("Official", id.ClassID);
      Assert.AreEqual ("StringValue", id.Value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Remotion.Data.DomainObjects.ObjectID does not support values of type 'System.Byte'.\r\nParameter name: value")]
    public void InitializeWithInvalidType ()
    {
      ObjectID id = new ObjectID ("Official", (byte) 1);
    }
  }
}
