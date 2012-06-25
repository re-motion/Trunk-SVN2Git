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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderTableInheritanceTest : TableInheritanceMappingTest
  {
    private RdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _provider = RdbmsProviderObjectMother.CreateForIntegrationTest (TableInheritanceTestDomainStorageProviderDefinition);
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }
    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = _provider.LoadDataContainer (DomainObjectIDs.Customer).LocatedObject;
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual ("UnitTests", customerContainer.GetValue (GetPropertyDefinition (typeof (TIDomainBase), "CreatedBy"), ValueAccess.Current));
      Assert.AreEqual (
          "Zaphod", customerContainer.GetValue (GetPropertyDefinition (typeof (TIPerson), "FirstName"), ValueAccess.Current));
      Assert.AreEqual (
          CustomerType.Premium,
          customerContainer.GetValue (GetPropertyDefinition (typeof (TICustomer), "CustomerType"), ValueAccess.Current));
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractBaseClass ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (TIDomainBase), "Client");
      var createdAtProperty = GetPropertyDefinition (typeof (TIDomainBase), "CreatedAt");
      var sortExpression = new SortExpressionDefinition (new[] { new SortedPropertySpecification (createdAtProperty, SortOrder.Ascending) });

      var loadedDataContainers = _provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          sortExpression,
          DomainObjectIDs.Client).ToList();

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (4, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, loadedDataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[3].ID);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractClassWithoutDerivations ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (AbstractClassWithoutDerivations), "DomainBase");

      var result = _provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Customer);
      Assert.IsNotNull (result);
      Assert.AreEqual (0, result.Count());
    }
  }
}