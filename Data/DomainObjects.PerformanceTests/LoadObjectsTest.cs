using System;
using System.Diagnostics;
using NUnit.Framework;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class LoadObjectsTest: DatabaseTest
  {
    // types

    // static members and constants

    private ObjectID _clientID;
    private ObjectID _fileID;

    // member fields

    // construction and disposing

    public LoadObjectsTest()
    {
    }

    // methods and properties

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();

      _clientID = new ObjectID ("Client", new Guid ("6F20355F-FA99-4c4e-B432-02C41F7BD390"));
      _fileID = new ObjectID ("File", Guid.NewGuid());

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Client.NewObject();
        File.NewObject();
        Company.NewObject();
        Person.NewObject();
      }
    }

    [Test]
    public void LoadObjectsOverRelationTest()
    {
      const int numberOfTests = 10;

      Console.WriteLine ("Expected average duration of LoadObjectsOverRelationTest on reference system: ~230 ms");

      Stopwatch stopwatch = new Stopwatch ();
      for (int i = 0; i < numberOfTests; i++)
      {
        using (ClientTransaction.NewRootTransaction().EnterScope (AutoRollbackBehavior.None))
        {
          Client client = Client.GetObject (_clientID);

          stopwatch.Start();

          DomainObjectCollection files = client.Files;

          stopwatch.Stop();

          Assert.AreEqual (6000, files.Count);
        }
      }

      double averageMilliSeconds = stopwatch.ElapsedMilliseconds / numberOfTests;
      Console.WriteLine ("LoadObjectsOverRelationTest (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
    }

    [Test]
    public void LoadObjectsOverRelationWithAbstractBaseClass()
    {
      const int numberOfTests = 10;

      Console.WriteLine ("Expected average duration of LoadObjectsOverRelationWithAbstractBaseClass on reference system: ~710 ms");

      Stopwatch stopwatch = new Stopwatch ();
      for (int i = 0; i < numberOfTests; i++)
      {
        using (ClientTransaction.NewRootTransaction ().EnterScope (AutoRollbackBehavior.None))
        {
          Client client = Client.GetObject (_clientID);

          stopwatch.Start();

          DomainObjectCollection clientBoundBaseClasses = client.ClientBoundBaseClasses;

          stopwatch.Stop();
          Assert.AreEqual (4000, clientBoundBaseClasses.Count);
        }
      }

      double averageMilliSeconds = stopwatch.ElapsedMilliseconds / numberOfTests;
      Console.WriteLine (
          "LoadObjectsOverRelationWithAbstractBaseClass (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
    }
  }
}