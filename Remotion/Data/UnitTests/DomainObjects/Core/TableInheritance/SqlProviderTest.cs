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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class SqlProviderTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = Provider.LoadDataContainer (DomainObjectIDs.Customer);
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual (
          "UnitTests", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.CreatedBy"));
      Assert.AreEqual (
          "Zaphod", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Person.FirstName"));
      Assert.AreEqual (
          CustomerType.Premium,
          customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Customer.CustomerType"));
    }

    [Test]
    public void LoadDataContainersByRelatedIDWithAbstractBaseClass ()
    {
      var relationEndPointDefinition =
          MappingConfiguration.Current.GetTypeDefinition (typeof (DomainBase)).GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client");

      var loadedDataContainers = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          ((VirtualRelationEndPointDefinition)relationEndPointDefinition.GetOppositeEndPointDefinition()).GetSortExpression(),
          DomainObjectIDs.Client);

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (4, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, loadedDataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[3].ID);
    }
  }
}