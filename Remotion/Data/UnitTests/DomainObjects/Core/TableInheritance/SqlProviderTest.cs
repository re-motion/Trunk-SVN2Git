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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions;
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
      Assert.AreEqual ("UnitTests", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.CreatedBy"));
      Assert.AreEqual ("Zaphod", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Person.FirstName"));
      Assert.AreEqual (CustomerType.Premium, customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Customer.CustomerType"));
    }

    [Test]
    public void LoadDataContainersByRelatedIDWithAbstractBaseClass ()
    {
      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      DataContainerCollection loadedDataContainers = Provider.LoadDataContainersByRelatedID (domainBaseClass, "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client", DomainObjectIDs.Client);

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (4, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, loadedDataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[3].ID);
    }

    [Test]
    public void GetColumnsFromSortExpression ()
    {
      var complexSortExpression =
          SortExpressionDefinitionObjectMother.ParseSortExpression (typeof (Order), "Number asc, OrderDate desc, Customer asc");

      Assert.That (Provider.GetColumnsFromSortExpression (complexSortExpression), Is.EqualTo ("[Number], [OrderDate], [CustomerID]"));
      Assert.That (Provider.GetColumnsFromSortExpression (null), Is.Null);
      Assert.That (Provider.GetColumnsFromSortExpression (SortExpressionDefinitionObjectMother.CreateEmptySortExpression()), Is.EqualTo (string.Empty));
    }
  }
}
