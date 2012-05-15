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
    [Ignore ("TODO RM-4855: enable")]
    [Test]
    public void SequenceOfDomainObjectProperties ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>() select o.OrderNumber;

      Assert.That (result, Is.EquivalentTo (new[] { 1, 2, 3, 4, 5, 6 }));
    }

    [Ignore ("TODO RM-4855: enable")]
    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "ObjectID should be created in memory. TODO RM-4855: adapt message")]
    public void SequenceOfDomainObjectIDs ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>() select o.ID;

      result.ToArray();
    }

    [Ignore ("TODO RM-4855: enable")]
    [Test]
    public void SequenceOfObjectIDs_ConstructedInMemory ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber < 3 select new ObjectID (o.ID.ClassDefinition, o.ID.Value)).ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem }));
    }

    [Ignore ("TODO RM-4855: enable")]
    [Test]
    public void ComplexProjection ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order>()
                    where o.OrderNumber == 1
                    select new { o.OrderNumber, Property = new { o.OrderTicket.FileName, o.OrderItems.Count } }).Single();

      Assert.That (result.OrderNumber, Is.EqualTo (1));
      Assert.That (result.Property.FileName, Is.EqualTo (@"C:\order1.png")); 
      Assert.That (result.Property.Count, Is.EqualTo (2));
    }

    [Ignore ("TODO RM-4855: enable")]
    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A Domain Object cannot be included in a complex projection. TODO RM-4855: adapt message")]
    public void ComplexProjection_ContainingDomainObject ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order> () select new { o.OrderNumber, o });

      result.ToArray();
    }

  }
}