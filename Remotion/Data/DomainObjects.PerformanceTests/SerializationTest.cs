// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Diagnostics;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  // Reference system: Dell Dimension 9200, Intel Core 2 @ 2,66 GHz, 3,93 GB RAM; run in Release mode

  [TestFixture]
  public class SerializationTest : DatabaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      WriteHeadings ();
    }

    [Test]
    public void SerializeRelationPropertyObjects00041 ()
    {
      PerformSerializationTests (1, "SerializeRelationPropertyObjects00041", 6, 9, 42387, CreateAndFillRelationPropertyObject);
    }

    [Test]
    public void SerializeRelationPropertyObjects00410 ()
    {
      PerformSerializationTests (10, "SerializeRelationPropertyObjects00410", 28, 59, 300489, CreateAndFillRelationPropertyObject);
    }

    [Test]
    public void SerializeRelationPropertyObjects01025 ()
    {
      PerformSerializationTests (25, "SerializeRelationPropertyObjects01025", 74, 158, 730678, CreateAndFillRelationPropertyObject);
    }

    [Test]
    public void SerializeRelationPropertyObjects10250 ()
    {
      PerformSerializationTests (250, "SerializeRelationPropertyObjects10205", 769, 2199,7183228, CreateAndFillRelationPropertyObject);
    }

    [Test]
    public void SerializeSmallValuePropertyObjects00050 ()
    {
      PerformSerializationTests (50, "SerializeSmallValuePropertyObjects00050", 4, 8, 53871, CreateAndFillSmallValuePropertyObject);
    }

    [Test]
    public void SerializeSmallValuePropertyObjects00500 ()
    {
      PerformSerializationTests (500, "SerializeSmallValuePropertyObjects00500", 45, 89, 453471, CreateAndFillSmallValuePropertyObject);
    }

    [Test]
    public void SerializeSmallValuePropertyObjects01025 ()
    {
      PerformSerializationTests (1025, "SerializeSmallValuePropertyObjects01025", 98, 175, 919671, CreateAndFillSmallValuePropertyObject);
    }

    [Test]
    public void SerializeSmallValuePropertyObjects10250 ()
    {
      PerformSerializationTests (10250, "SerializeSmallValuePropertyObjects10250", 909, 2699, 9111481, CreateAndFillSmallValuePropertyObject);
    }

    [Test]
    public void SerializeValuePropertyObjects00050 ()
    {
      PerformSerializationTests (50, "SerializeValuePropertyObjects00050", 7, 11, 81917, CreateAndFillValuePropertyObject);
    }

    [Test]
    public void SerializeValuePropertyObjects00500 ()
    {
      PerformSerializationTests (500, "SerializeValuePropertyObjects00500", 58, 125, 716417, CreateAndFillValuePropertyObject);
    }

    [Test]
    public void SerializeValuePropertyObjects01025 ()
    {
      PerformSerializationTests (1025, "SerializeValuePropertyObjects01025", 110, 282, 1456667, CreateAndFillValuePropertyObject);
    }

    [Test]
    public void SerializeValuePropertyObjects10250 ()
    {
      PerformSerializationTests (10250, "SerializeValuePropertyObjects10250", 1168, 4231, 14463936, CreateAndFillValuePropertyObject);
    }

    private void PerformSerializationTests (int count, string nameOfTest, int serExpectedMS, int deserExpectedMS, int expectedDataSize, Action objectCreator)
    {
      PerformSerializationTests (nameOfTest, serExpectedMS, deserExpectedMS, expectedDataSize, delegate
      {
        using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
        {
          for (int i = 0; i < count; ++i)
            objectCreator ();
          return ClientTransaction.Current;
        }
      });
    }

    public void WriteHeadings ()
    {
      Console.Write (
          "{0}\t{1}\t{2}\t{3}\t", "Test", "Expected serialization time (ms)", "Expected deserialization time (ms)", "Expected data size (bytes)");
      Console.WriteLine (
          "{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
          "Number of runs",
          "Serialization time (ms)",
          "Deserialization time (ms)",
          "Data size (bytes)",
          "Number of data containers",
          "Number of relation end points");
    }

    private void PerformSerializationTests (string nameOfTest, int expectedMSSerialization, int expectedMSDeserialization, int expectedDataSize,
        Func<ClientTransaction> transactionInitializer)
    {
      const int numberOfTests = 10;

      Console.Write ("{0}\t{1}\t{2}\t{3}\t", nameOfTest, expectedMSSerialization, expectedMSDeserialization, expectedDataSize.ToString ("n0"));

      Stopwatch serializationStopwatch = new Stopwatch ();
      Stopwatch deserializationStopwatch = new Stopwatch ();
      int dataContainers = 0;
      int relationEndPoints = 0;
      int dataSize = 0;

      for (int i = 0; i < numberOfTests; i++)
      {
        ClientTransaction transaction = transactionInitializer();
        DataManager dataManager = (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
        dataContainers += dataManager.DataContainerMap.Count;
        relationEndPoints += dataManager.RelationEndPointMap.Count;

        serializationStopwatch.Start ();
        byte[] data = Serializer.Serialize (transaction);
        serializationStopwatch.Stop ();

        deserializationStopwatch.Start ();
        Serializer.Deserialize (data);
        deserializationStopwatch.Stop ();
        
        dataSize += data.Length;
      }

      double serAverageMilliSeconds = (double)serializationStopwatch.ElapsedMilliseconds / numberOfTests;
      double deserAverageMilliSeconds = (double) deserializationStopwatch.ElapsedMilliseconds / numberOfTests;
      double averageDataContainers = ((double) dataContainers) / numberOfTests;
      double averageRelationEndPoints = ((double) relationEndPoints) / numberOfTests;
      double averageSize = ((double) dataSize) / numberOfTests;

      Console.WriteLine ("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", numberOfTests, serAverageMilliSeconds.ToString ("n"),
          deserAverageMilliSeconds.ToString ("n"), averageSize.ToString ("n0"), averageDataContainers.ToString ("n0"),
          averageRelationEndPoints.ToString ("n0"));
    }

    private void CreateAndFillValuePropertyObject ()
    {
      Random random = new Random();
      ClassWithValueProperties instance = ClassWithValueProperties.NewObject();

      instance.BoolProperty1 = random.Next () % 2 == 0;
      instance.BoolProperty2 = random.Next () % 2 == 0;
      instance.BoolProperty3 = random.Next () % 2 == 0;
      instance.BoolProperty4 = random.Next () % 2 == 0;
      instance.BoolProperty5 = random.Next () % 2 == 0;
      instance.BoolProperty6 = random.Next () % 2 == 0;
      instance.BoolProperty7 = random.Next () % 2 == 0;
      instance.BoolProperty8 = random.Next () % 2 == 0;
      instance.BoolProperty9 = random.Next () % 2 == 0;
      instance.BoolProperty10 = random.Next () % 2 == 0;

      instance.IntProperty1 = random.Next();
      instance.IntProperty2 = random.Next();
      instance.IntProperty3 = random.Next();
      instance.IntProperty4 = random.Next();
      instance.IntProperty5 = random.Next();
      instance.IntProperty6 = random.Next();
      instance.IntProperty7 = random.Next();
      instance.IntProperty8 = random.Next();
      instance.IntProperty9 = random.Next();
      instance.IntProperty10 = random.Next();

      instance.DateTimeProperty1 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty2 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty3 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty4 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty5 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty6 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty7 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty8 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty9 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty10 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);

      instance.StringProperty1 = Guid.NewGuid().ToString ();
      instance.StringProperty2 = Guid.NewGuid ().ToString ();
      instance.StringProperty3 = Guid.NewGuid ().ToString ();
      instance.StringProperty4 = Guid.NewGuid ().ToString ();
      instance.StringProperty5 = Guid.NewGuid ().ToString ();
      instance.StringProperty6 = Guid.NewGuid ().ToString ();
      instance.StringProperty7 = Guid.NewGuid ().ToString ();
      instance.StringProperty8 = Guid.NewGuid ().ToString ();
      instance.StringProperty9 = Guid.NewGuid ().ToString ();
      instance.StringProperty10 = Guid.NewGuid ().ToString ();
    }

    private void CreateAndFillSmallValuePropertyObject ()
    {
      Random random = new Random ();
      ClassWithFewValueProperties instance = ClassWithFewValueProperties.NewObject ();

      instance.BoolProperty1 = random.Next () % 2 == 0;
      instance.BoolProperty2 = random.Next () % 2 == 0;
      instance.BoolProperty3 = random.Next () % 2 == 0;
      instance.BoolProperty4 = random.Next () % 2 == 0;
      instance.BoolProperty5 = random.Next () % 2 == 0;

      instance.IntProperty1 = random.Next ();
      instance.IntProperty2 = random.Next ();
      instance.IntProperty3 = random.Next ();
      instance.IntProperty4 = random.Next ();
      instance.IntProperty5 = random.Next ();

      instance.DateTimeProperty1 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty2 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty3 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty4 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);
      instance.DateTimeProperty5 = new DateTime (random.Next () % 2000 + 1000, random.Next () % 12 + 1, random.Next () % 28 + 1);

      instance.StringProperty1 = Guid.NewGuid ().ToString ();
      instance.StringProperty2 = Guid.NewGuid ().ToString ();
      instance.StringProperty3 = Guid.NewGuid ().ToString ();
      instance.StringProperty4 = Guid.NewGuid ().ToString ();
      instance.StringProperty5 = Guid.NewGuid ().ToString ();
    }

    private void CreateAndFillRelationPropertyObject ()
    {
      ClassWithRelationProperties instance = ClassWithRelationProperties.NewObject();
      instance.Unary1 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary2 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary3 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary4 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary5 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary6 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary7 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary8 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary9 = OppositeClassWithAnonymousRelationProperties.NewObject ();
      instance.Unary10 = OppositeClassWithAnonymousRelationProperties.NewObject ();

      instance.Real1 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real2 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real3 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real4 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real5 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real6 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real7 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real8 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real9 = OppositeClassWithVirtualRelationProperties.NewObject ();
      instance.Real10 = OppositeClassWithVirtualRelationProperties.NewObject ();

      instance.Virtual1 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual2 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual3 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual4 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual5 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual6 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual7 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual8 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual9 = OppositeClassWithRealRelationProperties.NewObject ();
      instance.Virtual10 = OppositeClassWithRealRelationProperties.NewObject ();

      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject ());
    }
  }
}
