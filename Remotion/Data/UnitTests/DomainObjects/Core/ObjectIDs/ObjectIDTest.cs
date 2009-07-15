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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
    public void EscapedDelimiterPlaceholderInValue ()
    {
      new ObjectID ("Official", "Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
    }

    [Test]
    public void SerializeGuidValue ()
    {
      var id = new ObjectID ("Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.That (id.ToString (), Is.EqualTo ("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid"));
    }

    [Test]
    public void DeserializeGuidValue ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectID id = ObjectID.Parse (idString);

      Assert.That (id.StorageProviderID, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value.GetType (), Is.EqualTo (typeof (Guid)));
      Assert.That (id.Value, Is.EqualTo (new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }
    
    [Test]
    public void HashCode ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("Official", 42);
      var id3 = new ObjectID ("Official", 41);

      Assert.That (id1.GetHashCode () == id2.GetHashCode (), Is.True);
      Assert.That (id1.GetHashCode () == id3.GetHashCode (), Is.False);
      Assert.That (id2.GetHashCode () == id3.GetHashCode (), Is.False);
    }

    [Test]
    public void TestEqualsForClassID ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("Official", 42);
      var id3 = new ObjectID ("SpecialOfficial", 42);

      Assert.That (id1.Equals (id2), Is.True);
      Assert.That (id1.Equals (id3), Is.False);
      Assert.That (id2.Equals (id3), Is.False);
      Assert.That (id2.Equals (id1), Is.True);
      Assert.That (id3.Equals (id1), Is.False);
      Assert.That (id3.Equals (id2), Is.False);
    }

    [Test]
    public void TestEqualsForValue ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("Official", 42);
      var id3 = new ObjectID ("Official", 41);

      Assert.That (id1.Equals (id2), Is.True);
      Assert.That (id1.Equals (id3), Is.False);
      Assert.That (id2.Equals (id3), Is.False);
      Assert.That (id2.Equals (id1), Is.True);
      Assert.That (id3.Equals (id1), Is.False);
      Assert.That (id3.Equals (id2), Is.False);
    }

    [Test]
    public void EqualsWithOtherType ()
    {
      var id = new ObjectID ("Official", 42);
      Assert.That (id.Equals (new ObjectIDTest ()), Is.False);
      Assert.That (id.Equals (42), Is.False);
    }

    [Test]
    public void EqualsWithNull ()
    {
      var id = new ObjectID ("Official", 42);
      Assert.That (id.Equals (null), Is.False);
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("Official", 42);

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("SpecialOfficial", 1);

      Assert.That (id1 == id2, Is.False);
      Assert.That (id1 != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      var id1 = new ObjectID ("Official", 42);
      ObjectID id2 = id1;

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
// ReSharper disable RedundantCast
      Assert.That ((ObjectID) null == (ObjectID) null, Is.True);
      Assert.That ((ObjectID) null != (ObjectID) null, Is.False);
// ReSharper restore RedundantCast
    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      var id2 = new ObjectID ("Official", 42);

      Assert.That (null == id2, Is.False);
      Assert.That (null != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      var id1 = new ObjectID ("Official", 42);

      Assert.That (id1 == null, Is.False);
      Assert.That (id1 != null, Is.True);
    }

    [Test]
    public void StaticEquals ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("Official", 42);

      Assert.That (ObjectID.Equals (id1, id2), Is.True);
    }

    [Test]
    public void StaticNotEquals ()
    {
      var id1 = new ObjectID ("Official", 42);
      var id2 = new ObjectID ("SpecialOfficial", 1);

      Assert.That (ObjectID.Equals (id1, id2), Is.False);
    }

    [Test]
    public void InitializeWithClassID ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      var id = new ObjectID ("Order", value);

      Assert.That (id.StorageProviderID, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    public void InitializeWithClassType ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      var id = new ObjectID (typeof (Order), value);

      Assert.That (id.StorageProviderID, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    public void InitializeWithClassDefinition ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      var id = new ObjectID (orderDefinition, value);

      Assert.That (id.StorageProviderID, Is.EqualTo (orderDefinition.StorageProviderID));
      Assert.That (id.ClassID, Is.EqualTo (orderDefinition.ID));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InitializeWithUnknownClassDefinitionID ()
    {
      ReflectionBasedClassDefinition unknownDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("UnknownClass", "UnknownTable", "TestDomain", typeof (Order), false);
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      new ObjectID (unknownDefinition, value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The ClassID 'Order' and the ClassType 'Remotion.Data.UnitTests.DomainObjects.Core.ObjectIDs.ObjectIDTest+InvalidDomainObject'"
        + " do not refer to the same ClassDefinition in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithUnknownClassDefinitionType ()
    {
      ReflectionBasedClassDefinition unknownDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (InvalidDomainObject), false);
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      new ObjectID (unknownDefinition, value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void InitializeWithEmptyGuid ()
    {
      new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], Guid.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void InitializeWithEmptyString ()
    {
      new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], string.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The ClassID 'Order' and the ClassType 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer'"
        + " do not refer to the same ClassDefinition in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithInvalidClassDefinition ()
    {
      ReflectionBasedClassDefinition invalidDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Customer), false);
      new ObjectID (invalidDefinition, Guid.NewGuid ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provided ClassDefinition 'Order' is not the same reference as the ClassDefinition found in the mapping configuration.\r\nParameter name: classDefinition")]
    public void InitializeWithClassDefinitionNotPartOfMappingConfiguration ()
    {
      ReflectionBasedClassDefinition invalidDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      new ObjectID (invalidDefinition, Guid.NewGuid ());
    }

    [Test]
    [ExpectedException (typeof (IdentityTypeNotSupportedException))]
    public void InitializeWithInvalidIdentityType ()
    {
      new ObjectID ("Order", 1);
    }

    [Test]
    public void InitializeWithGuid ()
    {
      Guid idValue = Guid.NewGuid ();
      var id = new ObjectID ("Official", idValue);

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo (idValue));
    }

    [Test]
    public void InitializeWithInt32 ()
    {
      var id = new ObjectID ("Official", 1);

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo (1));
    }

    [Test]
    public void InitializeWithString ()
    {
      var id = new ObjectID ("Official", "StringValue");

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo ("StringValue"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Remotion.Data.DomainObjects.ObjectID does not support values of type 'System.Byte'.\r\nParameter name: value")]
    public void InitializeWithInvalidType ()
    {
      new ObjectID ("Official", (byte) 1);
    }
  }
}
