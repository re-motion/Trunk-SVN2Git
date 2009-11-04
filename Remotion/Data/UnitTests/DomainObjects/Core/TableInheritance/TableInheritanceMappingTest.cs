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
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class TableInheritanceMappingTest: DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTableInheritanceTestData.sql";
    public const string TableInheritanceTestDomainProviderID = "TableInheritanceTestDomain";

    private ClientTransactionScope _transactionScope;

    public TableInheritanceMappingTest()
      : base (new DatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetMappingConfiguration ());
      ConfigurationWrapper.SetCurrent (null);
      _transactionScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      _transactionScope.Leave ();
      base.TearDown ();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TableInheritanceConfiguration.Instance.GetDomainObjectIDs (); }
    }
  }
}
