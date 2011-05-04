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
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using System.Linq;
using Remotion.Development.UnitTesting;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class RelationQuerySyncSpike : DatabaseTest
  {
    [Test]
    public void LinearSearch_100 ()
    {
      const int testSetSize = 100;
      const int testRepititions = 10000;

      TimeFindRelatedObjects(testSetSize, testRepititions);
    }

    [Test]
    public void LinearSearch_500 ()
    {
      const int testSetSize = 500;
      const int testRepititions = 10000;

      TimeFindRelatedObjects (testSetSize, testRepititions);
    }

    [Test]
    public void LinearSearch_10000 ()
    {
      const int testSetSize = 10000;
      const int testRepititions = 1000;

      TimeFindRelatedObjects (testSetSize, testRepititions);
    }

    [Test]
    public void LinearSearch_20000 ()
    {
      const int testSetSize = 20000;
      const int testRepititions = 1000;

      TimeFindRelatedObjects (testSetSize, testRepititions);
    }

    private void TimeFindRelatedObjects (int testSetSize, int testRepititions)
    {
      Console.WriteLine ("Expected average duration of LoadObjectsOverRelationTest on reference system: ~0.3-0.6 µs/object");

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var originatingObject = ClassWithRelationProperties.NewObject ();
        var objects = TestDomainObjectMother.PrepareLocalObjects (testSetSize, () => OppositeClassWithCollectionRelationProperties.NewObject());

        for (int i = 0; i < objects.Length; i += objects.Length / 50)
          originatingObject.Collection.Add (objects[i]);

        var dataContainers = objects
            .Select (obj => ((DataManager) PrivateInvoke.GetNonPublicProperty (ClientTransaction.Current, "DataManager")).DataContainers[obj.ID])
            .ToArray ();
        Assert.That (dataContainers.Length, Is.EqualTo (testSetSize));

        var endPointID = originatingObject.Collection.AssociatedEndPointID;

        int result = 0;

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < testRepititions; i++)
          result += FindRelatedObjects (dataContainers, endPointID).Length;
        stopwatch.Stop();

        Console.WriteLine (result / testRepititions);

        double averageMilliSeconds = stopwatch.Elapsed.TotalMilliseconds / testRepititions;
        Console.WriteLine (
            "FindRelatedObjects_{3} (executed {0}x ({2} objects)): Average duration: {1} ms; i.e., {4} µs/object",
            testRepititions,
            averageMilliSeconds.ToString ("n"),
            dataContainers.Length,
            testSetSize,
            averageMilliSeconds / testSetSize * 1000.0);
      }
    }

    private DomainObject[] FindRelatedObjects (IEnumerable<DataContainer> dataContainers, RelationEndPointID relationEndPointID)
    {
      var originatingObjectID = relationEndPointID.ObjectID;
      var oppositeEndPointDefinition = relationEndPointID.Definition.GetOppositeEndPointDefinition ();

      var originalRelatedObjects = new List<DomainObject> ();
      var currentRelatedObjects = new List<DomainObject> ();

      foreach (var dc in dataContainers)
      {
        if (oppositeEndPointDefinition.ClassDefinition.ClassType.IsAssignableFrom (dc.ID.ClassDefinition.ClassType))
        {
          var propertyValue = dc.PropertyValues[oppositeEndPointDefinition.PropertyName];
          
          if (originatingObjectID.Equals (propertyValue.GetValueWithoutEvents (ValueAccess.Current)))
            currentRelatedObjects.Add (dc.DomainObject);
          if (originatingObjectID.Equals (propertyValue.GetValueWithoutEvents (ValueAccess.Original)))
            originalRelatedObjects.Add (dc.DomainObject);
        }
      }

      return currentRelatedObjects.ToArray ();
    }
  }
}