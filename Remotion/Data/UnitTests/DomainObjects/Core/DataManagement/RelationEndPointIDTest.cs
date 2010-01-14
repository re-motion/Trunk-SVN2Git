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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private string _propertyName;
    private RelationEndPointID _endPointID;
    private RelationEndPointID _nullEndPointID;
    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID = DomainObjectIDs.Order1;
      _propertyName = "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket";
      _endPointDefinition = _objectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (_propertyName);

      _endPointID = new RelationEndPointID (_objectID, _propertyName);
      _nullEndPointID = new RelationEndPointID (null, _endPointID.Definition);
    }

    [Test]
    public void Initialize_WithDefinition ()
    {
      var endPointID = new RelationEndPointID (_objectID, _endPointDefinition);
      
      Assert.That (endPointID.Definition, Is.EqualTo (_endPointDefinition));
      Assert.That (endPointID.ObjectID, Is.EqualTo (_objectID));
    }

    [Test]
    public void Initialize_WithPropertyName ()
    {
      var endPointID = new RelationEndPointID (_objectID, _propertyName);
      Assert.That (endPointID.Definition, Is.EqualTo (_endPointDefinition));
      Assert.That (endPointID.ObjectID, Is.EqualTo (_objectID));
    }

    [Test]
    public void Initialize_WithShortPropertyName ()
    {
      var endPointID = new RelationEndPointID (_objectID, typeof (Order), "OrderTicket");
      Assert.That (endPointID.Definition, Is.EqualTo (_endPointDefinition));
      Assert.That (endPointID.ObjectID, Is.EqualTo (_objectID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidPropertyName ()
    {
      new RelationEndPointID (_objectID, (string) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidObjectID ()
    {
      new RelationEndPointID (null, _propertyName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InitializeWithInvalidClassID ()
    {
      var objectIDWithInvalidClass = new ObjectID ("InvalidClassID", Guid.NewGuid());

      new RelationEndPointID (objectIDWithInvalidClass, "PropertyName");
    }

    [Test]
    public void HashCode ()
    {
      int expectedHashCode = _objectID.GetHashCode() ^ _propertyName.GetHashCode();
      Assert.That (_endPointID.GetHashCode (), Is.EqualTo (expectedHashCode));
    }

    [Test]
    public void HashCode_NullID ()
    {
      int expectedHashCode = _propertyName.GetHashCode();
      Assert.That (_nullEndPointID.GetHashCode (), Is.EqualTo (expectedHashCode));
    }

    [Test]
    public void Equals ()
    {
      var endPointID2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.That (_endPointID.Equals (endPointID2), Is.True);
    }

    [Test]
    public void EqualsForObjectID ()
    {
      var endPointID2 = new RelationEndPointID (ObjectID.Parse (_objectID.ToString()), _propertyName);
      var endPointID3 = new RelationEndPointID (DomainObjectIDs.Order2, _propertyName);

      Assert.That (_endPointID.Equals (endPointID2), Is.True);
      Assert.That (endPointID2.Equals (_endPointID), Is.True);
      Assert.That (_endPointID.Equals (endPointID3), Is.False);
      Assert.That (endPointID3.Equals (_endPointID), Is.False);
      Assert.That (endPointID2.Equals (endPointID3), Is.False);
      Assert.That (endPointID3.Equals (endPointID2), Is.False);
    }

    [Test]
    public void EqualsWithOtherType ()
    {
      Assert.That (_endPointID.Equals (new RelationEndPointIDTest ()), Is.False);
    }

    [Test]
    public void EqualsWithNull ()
    {
      Assert.That (_endPointID.Equals (null), Is.False);
    }

    [Test]
    public new void ToString ()
    {
      string expected = _objectID + "/" + _propertyName;
      Assert.That (_endPointID.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_WithNull ()
    {
      string expected = "null/" + _propertyName;
      Assert.That (_nullEndPointID.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void GetAllRelationEndPointIDs ()
    {
      var expectedPropertyNames = new[]
                                       {
                                           "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer",
                                           "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket",
                                           "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems",
                                           "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"
                                       };

      DataContainer existingDataContainer = DataContainer.CreateForExisting (
          new ObjectID ("Order", Guid.NewGuid()),
          null,
          definition => definition.DefaultValue);

      RelationEndPointID[] endPointIDs = RelationEndPointID.GetAllRelationEndPointIDs (existingDataContainer);

      Assert.That (endPointIDs.Length, Is.EqualTo (4));
      Assert.That (endPointIDs[0].ObjectID, Is.SameAs (existingDataContainer.ID));
      Assert.That (endPointIDs[1].ObjectID, Is.SameAs (existingDataContainer.ID));
      Assert.That (endPointIDs[2].ObjectID, Is.SameAs (existingDataContainer.ID));
      Assert.That (endPointIDs[3].ObjectID, Is.SameAs (existingDataContainer.ID));
      Assert.That (Array.IndexOf (expectedPropertyNames, endPointIDs[0].Definition.PropertyName) >= 0, Is.True);
      Assert.That (Array.IndexOf (expectedPropertyNames, endPointIDs[1].Definition.PropertyName) >= 0, Is.True);
      Assert.That (Array.IndexOf (expectedPropertyNames, endPointIDs[2].Definition.PropertyName) >= 0, Is.True);
      Assert.That (Array.IndexOf (expectedPropertyNames, endPointIDs[3].Definition.PropertyName) >= 0, Is.True);
    }

    [Test]
    public void StaticEquals ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.That (RelationEndPointID.Equals (id1, id2), Is.True);
    }

    [Test]
    public void StaticNotEquals ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.That (RelationEndPointID.Equals (id1, id2), Is.False);
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.That (id1 == id2, Is.False);
      Assert.That (id1 != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = id1;

      Assert.That (id1 == id2, Is.True);
      Assert.That (id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
      var nullID1 = (RelationEndPointID) null;
      var nullID2 = (RelationEndPointID) null;
      Assert.That (nullID1 == nullID2, Is.True);
      Assert.That (nullID1 != nullID2, Is.False);
    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      var id2 = new RelationEndPointID (
          DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.That (null == id2, Is.False);
      Assert.That (null != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);

      Assert.That (id1 == null, Is.False);
      Assert.That (id1 != null, Is.True);
    }
  }
}