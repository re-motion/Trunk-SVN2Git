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
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class ClassDefinitionTest : DatabaseTest
  {
    public const int TestRepititions = 1000000;

    [Test]
    public void GetOppositeClassDefinition ()
    {
      Console.WriteLine ("Expected average duration of GetOppositeClassDefinition on reference system: ~0,25 탎");

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithRelationProperties));
      var endPoints = classDefinition.GetRelationEndPointDefinitions().ToList();

      bool found = true;
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        found &= classDefinition.GetMandatoryOppositeClassDefinition (endPoints[i % endPoints.Count].PropertyName) != null;
      stopwatch.Stop();

      Console.WriteLine (found);

      double averageMicroseconds = stopwatch.Elapsed.TotalMilliseconds * 1000.0 / TestRepititions;
      Console.WriteLine (
          "GetOppositeClassDefinition (executed {0}x ({2} end points)): Average duration: {1} 탎",
          TestRepititions,
          averageMicroseconds.ToString ("n"),
          endPoints.Count);
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition ()
    {
      Console.WriteLine ("Expected average duration of GetMandatoryOppositeClassDefinition on reference system: ~0,25 탎");

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithRelationProperties));
      var endPoints = classDefinition.GetRelationEndPointDefinitions().ToList();

      bool found = true;
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        found &= classDefinition.GetMandatoryOppositeClassDefinition (endPoints[i % endPoints.Count].PropertyName) != null;
      stopwatch.Stop();

      Console.WriteLine (found);

      double averageMicroseconds = stopwatch.Elapsed.TotalMilliseconds * 1000.0 / TestRepititions;
      Console.WriteLine (
          "GetMandatoryOppositeClassDefinition (executed {0}x ({2} end points)): Average duration: {1} 탎",
          TestRepititions,
          averageMicroseconds.ToString ("n"),
          endPoints.Count);
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Console.WriteLine ("Expected average duration of GetOppositeEndPointDefinition on reference system: ~0,25 탎");

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithRelationProperties));
      var endPoints = classDefinition.GetRelationEndPointDefinitions().ToList();

      bool found = true;
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        found &= classDefinition.GetOppositeEndPointDefinition (endPoints[i % endPoints.Count].PropertyName) != null;
      stopwatch.Stop();

      Console.WriteLine (found);

      double averageMicroseconds = stopwatch.Elapsed.TotalMilliseconds * 1000.0 / TestRepititions;
      Console.WriteLine (
          "GetOppositeEndPointDefinition (executed {0}x ({2} end points)): Average duration: {1} 탎",
          TestRepititions,
          averageMicroseconds.ToString ("n"),
          endPoints.Count);
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition ()
    {
      Console.WriteLine ("Expected average duration of GetMandatoryOppositeEndPointDefinition on reference system: ~0,25 탎");

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithRelationProperties));
      var endPoints = classDefinition.GetRelationEndPointDefinitions().ToList();

      bool found = true;
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        found &= classDefinition.GetMandatoryOppositeEndPointDefinition (endPoints[i % endPoints.Count].PropertyName) != null;
      stopwatch.Stop();

      Console.WriteLine (found);

      double averageMicroseconds = stopwatch.Elapsed.TotalMilliseconds * 1000.0 / TestRepititions;
      Console.WriteLine (
          "GetMandatoryOppositeEndPointDefinition (executed {0}x ({2} end points)): Average duration: {1} 탎",
          TestRepititions,
          averageMicroseconds.ToString ("n"),
          endPoints.Count);
    }

    [Test]
    public void SetStorageEntity ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithRelationProperties));
      var tableDefinition = new TableDefinition (
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new EntityNameDefinition (null, "TestView"),
          new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true),
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false),
          new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);

      Assert.That (classDefinition.StorageEntityDefinition, Is.Not.SameAs (tableDefinition));
      classDefinition.SetStorageEntity (tableDefinition);
      Assert.That (classDefinition.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }
  }
}