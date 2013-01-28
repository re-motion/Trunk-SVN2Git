// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.ObjectIDs
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    private ClassDefinition _orderClassDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderClassDefinition = MappingConfiguration.Current.GetClassDefinition ("Order");
    }

    [Test]
    public void Create_WithAbstractType ()
    {
      try
      {
        ObjectID.Create(typeof (TIDomainBase), Guid.NewGuid ());
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        string expectedMessage = string.Format (
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.\r\nParameter name: classDefinition",
            typeof (TIDomainBase).AssemblyQualifiedName, "TI_DomainBase");

        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
    public void Create_EscapedDelimiterPlaceholderInValue ()
    {
      ObjectID.Create("Official", "Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
    }

    [Test]
    public void AsObjectID ()
    {
      var id = (IObjectID<DomainObject>) ObjectID.Create (_orderClassDefinition, new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.That (id.AsObjectID(), Is.SameAs (id));
    }

    [Test]
    public void AsObjectID_Generic_CanUpcast ()
    {
      var id = ObjectID.Create (_orderClassDefinition, new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));

      var result = id.AsObjectID<Order>();

      Assert.That (result, Is.SameAs (id));
      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (IObjectID<Order>)));
    }

    [Test]
    public void AsObjectID_Generic_CanDowncast ()
    {
      var id = ObjectID.Create (_orderClassDefinition, new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));

      var result = id.AsObjectID<TestDomainBase> ();

      Assert.That (result, Is.SameAs (id));
      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (IObjectID<TestDomainBase>)));
    }

    [Test]
    public void AsObjectID_Generic_ThrowsOnUnsupportedCast ()
    {
      var id = ObjectID.Create (_orderClassDefinition, new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));

      Assert.That (
          () => id.AsObjectID<OrderItem>(),
          Throws.TypeOf<InvalidCastException>().With.Message.EqualTo (
              "The ObjectID 'Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid' cannot be represented as an "
              + "IObjectID<Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem>."));
    }

    [Test]
    public void SerializeGuidValue ()
    {
      var id = ObjectID.Create("Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.That (id.ToString (), Is.EqualTo ("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid"));
    }

    [Test]
    public void DeserializeGuidValue ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectID id = ObjectID.Parse (idString);

      Assert.That (id, Is.TypeOf<ObjectID<Order>>());
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value.GetType (), Is.EqualTo (typeof (Guid)));
      Assert.That (id.Value, Is.EqualTo (new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }
    
    [Test]
    public void HashCode ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("Official", 42);
      var id3 = ObjectID.Create("Official", 41);

      Assert.That (id1.GetHashCode () == id2.GetHashCode (), Is.True);
      Assert.That (id1.GetHashCode () == id3.GetHashCode (), Is.False);
      Assert.That (id2.GetHashCode () == id3.GetHashCode (), Is.False);
    }

    [Test]
    public void TestEqualsForClassID ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("Official", 42);
      var id3 = ObjectID.Create("SpecialOfficial", 42);

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
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("Official", 42);
      var id3 = ObjectID.Create("Official", 41);

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
      var id = ObjectID.Create("Official", 42);
      Assert.That (id.Equals (new ObjectIDTest ()), Is.False);
      Assert.That (id.Equals (42), Is.False);
    }

    [Test]
    public void EqualsWithNull ()
    {
      var id = ObjectID.Create("Official", 42);
      Assert.That (id.Equals (null), Is.False);
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("Official", 42);

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("SpecialOfficial", 1);

      Assert.That (id1 == id2, Is.False);
      Assert.That (id1 != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      var id1 = ObjectID.Create("Official", 42);
      ObjectID id2 = id1;

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
// ReSharper disable RedundantCast
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
      Assert.That ((ObjectID) null == (ObjectID) null, Is.True);
      Assert.That ((ObjectID) null != (ObjectID) null, Is.False);
// ReSharper restore ConditionIsAlwaysTrueOrFalse
// ReSharper restore EqualExpressionComparison
// ReSharper restore RedundantCast
    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      var id2 = ObjectID.Create("Official", 42);

      Assert.That (null == id2, Is.False);
      Assert.That (null != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      var id1 = ObjectID.Create("Official", 42);

      Assert.That (id1 == null, Is.False);
      Assert.That (id1 != null, Is.True);
    }

    [Test]
    public void StaticEquals ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("Official", 42);

      Assert.That (ObjectID.Equals (id1, id2), Is.True);
    }

    [Test]
    public void StaticNotEquals ()
    {
      var id1 = ObjectID.Create("Official", 42);
      var id2 = ObjectID.Create("SpecialOfficial", 1);

      Assert.That (ObjectID.Equals (id1, id2), Is.False);
    }

    [Test]
    public void Create_WithClassID ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      var id = ObjectID.Create("Order", value);

      Assert.That (id, Is.TypeOf<ObjectID<Order>>());
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassDefinition, Is.SameAs (_orderClassDefinition));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    public void Create_WithClassType ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      var id = ObjectID.Create(typeof (Order), value);

      Assert.That (id, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassDefinition, Is.SameAs (_orderClassDefinition));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    public void Create_WithGenericType_CreatesGenericObjectIDWithCorrectClassDefinition ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
      var id = ObjectID.Create<Order> (value);

      Assert.That (id, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassDefinition, Is.SameAs (_orderClassDefinition));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    public void Create_WithGenericType_WithUnmappedType_Throws ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      Assert.That (
          () => ObjectID.Create<UnmappedDomainObject> (value),
          Throws.TypeOf<MappingException>()
                .With.Message.EqualTo (
                    "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.ObjectIDs.ObjectIDTest+UnmappedDomainObject'."));
    }

    [Test]
    public void Create_WithGenericType_WithNullValue_Throws ()
    {
      Assert.That (() => ObjectID.Create<Order> (null), Throws.TypeOf<ArgumentNullException> ());
    }

    [Test]
    public void Create_WithClassDefinition ()
    {
      var value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

      var id = ObjectID.Create(_orderClassDefinition, value);

      Assert.That (id, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo (_orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name));
      Assert.That (id.ClassDefinition, Is.SameAs (_orderClassDefinition));
      Assert.That (id.Value, Is.EqualTo (value));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void Create_WithEmptyGuid ()
    {
      ObjectID.Create(MappingConfiguration.Current.GetClassDefinition ("Order"), Guid.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void Create_WithEmptyString ()
    {
      ObjectID.Create(MappingConfiguration.Current.GetClassDefinition ("Order"), string.Empty);
    }

    [Test]
    [ExpectedException (typeof (IdentityTypeNotSupportedException))]
    public void Create_WithInvalidIdentityType ()
    {
      ObjectID.Create("Order", 1);
    }

    [Test]
    public void Create_WithGuid ()
    {
      Guid idValue = Guid.NewGuid ();
      var id = ObjectID.Create("Official", idValue);

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo (idValue));
    }

    [Test]
    public void Create_WithInt32 ()
    {
      var id = ObjectID.Create("Official", 1);

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo (1));
    }

    [Test]
    public void Create_WithString ()
    {
      var id = ObjectID.Create("Official", "StringValue");

      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value, Is.EqualTo ("StringValue"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Remotion.Data.DomainObjects.ObjectID does not support values of type 'System.Byte'.\r\nParameter name: value")]
    public void Create_WithInvalidType ()
    {
      ObjectID.Create("Official", (byte) 1);
    }

    [Test]
    public void CompareTo_String ()
    {
      var id1 = ObjectID.Create("Official", "aaa");
      var id2 = ObjectID.Create("Official", "bbb");
      var id3 = ObjectID.Create("Official", "aaa");

      Assert.That (id1.CompareTo (id2), Is.EqualTo (-1));
      Assert.That (id2.CompareTo (id1), Is.EqualTo (1));
      Assert.That (id1.CompareTo (id3), Is.EqualTo (0));
    }

    [Test]
    public void CompareTo_Guid ()
    {
      var id1 = ObjectID.Create(typeof (Order), new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));
      var id2 = ObjectID.Create(typeof (Order), new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C37}"));
      var id3 = ObjectID.Create(typeof (Order), new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));

      Assert.That (id1.CompareTo (id2), Is.EqualTo (-1));
      Assert.That (id2.CompareTo (id1), Is.EqualTo (1));
      Assert.That (id1.CompareTo (id3), Is.EqualTo (0));
    }

    [Test]
    public void CompareTo_DifferentValueTypes ()
    {
      var id1 = ObjectID.Create(typeof (Order), new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));
      var id2 = ObjectID.Create("Official", "test");

      Assert.That (id1.CompareTo (id2), Is.EqualTo (1));
      Assert.That (id2.CompareTo (id1), Is.EqualTo (-1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument must be of type ObjectID.\r\nParameter name: obj")]
    public void CompareTo_InvalidArgument ()
    {
      var id = ObjectID.Create("Official", "aaa");

      id.CompareTo ("test");
    }

    [IgnoreForMappingConfiguration]
    private class UnmappedDomainObject : DomainObject
    {
    }
  }
}
