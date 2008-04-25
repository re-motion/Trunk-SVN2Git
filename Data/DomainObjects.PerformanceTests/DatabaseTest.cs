using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.PerformanceTests.Database;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class DatabaseTest
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      StandardConfiguration.Initialize();
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
      using (TestDataLoader loader = new TestDataLoader (StandardConfiguration.ConnectionString))
      {
        loader.Load();
      }
    }

    [TearDown]
    public virtual void TearDown()
    {
    }
  }
}