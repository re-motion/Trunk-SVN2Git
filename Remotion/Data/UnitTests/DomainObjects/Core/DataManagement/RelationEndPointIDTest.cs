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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private string _propertyName;
    private RelationEndPointID _endPointID;
    private RelationEndPointID _nullEndPointID;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID = DomainObjectIDs.Order1;
      _propertyName = "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket";
      _endPointID = new RelationEndPointID (_objectID, _propertyName);
      _nullEndPointID = new RelationEndPointID (null, _endPointID.Definition);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (_propertyName, _endPointID.Definition.PropertyName);
      Assert.AreEqual (_objectID, _endPointID.ObjectID);
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
      Assert.AreEqual (expectedHashCode, _endPointID.GetHashCode());
    }

    [Test]
    public void HashCode_NullID ()
    {
      int expectedHashCode = _propertyName.GetHashCode();
      Assert.AreEqual (expectedHashCode, _nullEndPointID.GetHashCode());
    }

    [Test]
    public void Equals ()
    {
      var endPointID2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.IsTrue (_endPointID.Equals (endPointID2));
    }

    [Test]
    public void EqualsForObjectID ()
    {
      var endPointID2 = new RelationEndPointID (ObjectID.Parse (_objectID.ToString()), _propertyName);
      var endPointID3 = new RelationEndPointID (DomainObjectIDs.Order2, _propertyName);

      Assert.IsTrue (_endPointID.Equals (endPointID2));
      Assert.IsTrue (endPointID2.Equals (_endPointID));
      Assert.IsFalse (_endPointID.Equals (endPointID3));
      Assert.IsFalse (endPointID3.Equals (_endPointID));
      Assert.IsFalse (endPointID2.Equals (endPointID3));
      Assert.IsFalse (endPointID3.Equals (endPointID2));
    }

    [Test]
    public void EqualsWithOtherType ()
    {
      Assert.IsFalse (_endPointID.Equals (new RelationEndPointIDTest()));
    }

    [Test]
    public void EqualsWithNull ()
    {
      Assert.IsFalse (_endPointID.Equals (null));
    }

    [Test]
    public new void ToString ()
    {
      string expected = _objectID + "/" + _propertyName;
      Assert.AreEqual (expected, _endPointID.ToString());
    }

    [Test]
    public void ToString_WithNull ()
    {
      string expected = "null/" + _propertyName;
      Assert.AreEqual (expected, _nullEndPointID.ToString ());
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

      Assert.AreEqual (4, endPointIDs.Length);
      Assert.AreSame (existingDataContainer.ID, endPointIDs[0].ObjectID);
      Assert.AreSame (existingDataContainer.ID, endPointIDs[1].ObjectID);
      Assert.AreSame (existingDataContainer.ID, endPointIDs[2].ObjectID);
      Assert.AreSame (existingDataContainer.ID, endPointIDs[3].ObjectID);
      Assert.IsTrue (Array.IndexOf (expectedPropertyNames, endPointIDs[0].Definition.PropertyName) >= 0);
      Assert.IsTrue (Array.IndexOf (expectedPropertyNames, endPointIDs[1].Definition.PropertyName) >= 0);
      Assert.IsTrue (Array.IndexOf (expectedPropertyNames, endPointIDs[2].Definition.PropertyName) >= 0);
      Assert.IsTrue (Array.IndexOf (expectedPropertyNames, endPointIDs[3].Definition.PropertyName) >= 0);
    }

    [Test]
    public void StaticEquals ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.IsTrue (RelationEndPointID.Equals (id1, id2));
    }

    [Test]
    public void StaticNotEquals ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.IsFalse (RelationEndPointID.Equals (id1, id2));
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (_objectID, _propertyName);

      Assert.IsTrue (id1 == id2);
      Assert.IsFalse (id1 != id2);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.IsFalse (id1 == id2);
      Assert.IsTrue (id1 != id2);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);
      var id2 = id1;

      Assert.IsTrue (id1 == id2);
      Assert.IsFalse (id1 != id2);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
      var nullID1 = (RelationEndPointID) null;
      var nullID2 = (RelationEndPointID) null;
      Assert.IsTrue (nullID1 == nullID2);
      Assert.IsFalse (nullID1 != nullID2);
    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      var id2 = new RelationEndPointID (
          DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.IsFalse (null == id2);
      Assert.IsTrue (null != id2);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      var id1 = new RelationEndPointID (_objectID, _propertyName);

      Assert.IsFalse (id1 == null);
      Assert.IsTrue (id1 != null);
    }
  }
}