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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class CustomProjectionIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void SequenceOfDomainObjectProperties ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>() select o.OrderNumber;

      Assert.That (result, Is.EquivalentTo (new[] { 1, 2, 3, 4, 5, 6 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type 'ObjectID' ist not supported by this storage provider.\n"+
      "Please select the ID and ClassID values separately (ID.Value, ID.ClassID), then create an ObjectID with it in memory.")]
    public void SequenceOfDomainObjectIDs ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>() select o.ID;

      result.ToArray();
    }

    [Test]
    [Ignore("TODO 4871: enable")]
    public void SequenceOfObjectIDs_ConstructedInMemory ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber < 3 select new ObjectID (o.ID.ClassID, o.ID.Value)).ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem }));
    }

    [Test]
    public void ComplexProjection ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order>()
                    where o.OrderNumber == 1
                    select new { o.OrderNumber, Property = new { o.OrderTicket.FileName, Count = o.OrderItems.Count() } }).Single();

      Assert.That (result.OrderNumber, Is.EqualTo (1));
      Assert.That (result.Property.FileName, Is.EqualTo (@"C:\order1.png")); 
      Assert.That (result.Property.Count, Is.EqualTo (2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
      "This LINQ provider does not support queries with complex projections that include DomainObjects."+
      "Either change the query to return just a sequence of DomainObjects ('from o in QueryFactory.CreateLinqQuery<Order>() select o') "
      +"or change the complex projection to contain no DomainObjects ('from o in QueryFactory.CreateLinqQuery<Order>() select new { o.OrderNumber, o.OrderDate }').")]
    public void ComplexProjection_ContainingDomainObject ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order> () select new { o.OrderNumber, o });

      result.ToArray();
    }

  }
}