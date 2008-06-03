/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
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
      _transactionScope = ClientTransaction.NewRootTransaction().EnterDiscardingScope();
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
