using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance
{
  [SetUpFixture]
  public class TableInheritanceSetUpFixture
  {
    private DatabaseAgent _databaseAgent;

    [SetUp]
    public void SetUp()
    {
      TableInheritanceConfiguration.Initialize();

      _databaseAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      _databaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      _databaseAgent.ExecuteBatch (TableInheritanceMappingTest.CreateTestDataFileName, true);
      _databaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }
  }
}