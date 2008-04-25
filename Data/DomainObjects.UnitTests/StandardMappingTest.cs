using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class StandardMappingTest: DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTestData.sql";

    protected StandardMappingTest()
      : base (new StandardMappingDatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);      
      TestMappingConfiguration.Reset();
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return StandardConfiguration.Instance.GetDomainObjectIDs(); }
    }
  }
}