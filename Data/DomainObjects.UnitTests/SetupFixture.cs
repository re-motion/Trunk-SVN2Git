using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingDatabaseAgent _standardMappingDatabaseAgent;

    [SetUp]
    public void SetUp()
    {
      StandardConfiguration.Initialize();

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
      masterAgent.ExecuteBatch ("DataDomainObjects_CreateDB.sql", false);
      DatabaseAgent testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      testDomainAgent.ExecuteBatch ("DataDomainObjects_SetupDB.sql", true);

      _standardMappingDatabaseAgent = new StandardMappingDatabaseAgent (DatabaseTest.TestDomainConnectionString);
      _standardMappingDatabaseAgent.ExecuteBatch (StandardMappingTest.CreateTestDataFileName, true);
      _standardMappingDatabaseAgent.ExecuteBatch (TableInheritanceMappingTest.CreateTestDataFileName, true);
      _standardMappingDatabaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _standardMappingDatabaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}