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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Order = Remotion.Data.UnitTests.DomainObjects.TestDomain.Order;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
  {
    [Test]
    public void Loading ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      var collection = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Customer1);

      Assert.IsNotNull (collection);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
    }

    [Test]
    public void LoadOverInheritedProperty ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Partner), "ContactPerson");

      var collection = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Person6);

      Assert.AreEqual (1, collection.Count);
      Assert.AreEqual (DomainObjectIDs.Distributor2, collection[0].ID);
    }

    [Test]
    public void LoadWithOrderBy ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      var orderNumberProperty = GetPropertyDefinition (typeof (Order), "OrderNumber");
      var sortExpression = new SortExpressionDefinition (new[] { new SortedPropertySpecification (orderNumberProperty, SortOrder.Ascending) });

      var orderContainers = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          sortExpression,
          DomainObjectIDs.Customer1);

      Assert.AreEqual (2, orderContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, orderContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orderContainers[1].ID);
    }

    [Test]
    public void LoadDataContainersByRelatedIDOfDifferentStorageProvider ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "Official");

      var orderContainers = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Official1);
      Assert.IsNotNull (orderContainers);
      Assert.AreEqual (5, orderContainers.Count);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithStorageClassTransactionProperty ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Computer), "EmployeeTransactionProperty");

      var result = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Employee1);
      Assert.IsNotNull (result);
      Assert.AreEqual (0, result.Count);
    }
  }
}