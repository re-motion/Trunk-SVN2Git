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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class HasRelationChangedTest : DatabaseTest
  {
    public const int TestSetSize = 1000;
    public const int TestRepititions = 10;

    [Test]
    public void AskChanged ()
    {
      Console.WriteLine ("Expected average duration of HasRelationChangedTest on reference system: ~20 ms");

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithRelationProperties[] objects = PrepareData();
        bool changed = ClientTransaction.Current.HasChanged();

        Assert.That (changed, Is.False);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < TestRepititions; i++)
          changed ^= ClientTransaction.Current.HasChanged();
        stopwatch.Stop();

        Console.WriteLine (changed);

        double averageMilliSeconds = stopwatch.ElapsedMilliseconds / TestRepititions;
        Console.WriteLine (
            "HasRelationChangedTest (executed {0} x ClientTransaction.Current.HasChanged ({2} objects)): Average duration: {1} ms",
            TestRepititions,
            averageMilliSeconds.ToString ("n"),
            objects.Length);
      }
    }

    private ClassWithRelationProperties[] PrepareData ()
    {
      ClassWithRelationProperties[] objects = QueryFactory.CreateLinqQuery<ClassWithRelationProperties>().Select (o => o).ToArray();
      if (objects.Length < TestSetSize)
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          for (int i = 0; i < TestSetSize - objects.Length; i++)
            CreateAndFillRelationPropertyObject();
          ClientTransaction.Current.Commit();
        }
        objects = QueryFactory.CreateLinqQuery<ClassWithRelationProperties>().Select (o => o).ToArray();
      }
      return objects;
    }

    private void CreateAndFillRelationPropertyObject ()
    {
      ClassWithRelationProperties instance = ClassWithRelationProperties.NewObject();
      instance.Unary1 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary2 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary3 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary4 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary5 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary6 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary7 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary8 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary9 = OppositeClassWithAnonymousRelationProperties.NewObject();
      instance.Unary10 = OppositeClassWithAnonymousRelationProperties.NewObject();

      instance.Real1 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real2 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real3 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real4 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real5 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real6 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real7 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real8 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real9 = OppositeClassWithVirtualRelationProperties.NewObject();
      instance.Real10 = OppositeClassWithVirtualRelationProperties.NewObject();

      instance.Virtual1 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual2 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual3 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual4 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual5 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual6 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual7 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual8 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual9 = OppositeClassWithRealRelationProperties.NewObject();
      instance.Virtual10 = OppositeClassWithRealRelationProperties.NewObject();

      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
      instance.Collection.Add (OppositeClassWithCollectionRelationProperties.NewObject());
    }
  }
}