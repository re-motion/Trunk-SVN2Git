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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  /// <summary>
  /// This class is the root to execute a single test in the profiler.
  /// Switch the project type to a ConsoleApplication in order to use it.
  /// </summary>
  public class Root
  {
    private Root ()
    {
    }

    [STAThread]
    public static void Main (string[] args)
    {
      LogManager.Initialize();

      var setUpFixture = new SetUpFixture();
      setUpFixture.SetUp();

      // Have all xml files loaded, so if the code is instrumented by a profiler, 
      // the loading does not falsify the method run times during the first call of GetObject.
      Dev.Null = MappingConfiguration.Current;
      Dev.Null = DomainObjectsConfiguration.Current.Query;

      //RunLoadObjectsTest();

      //RunSerializationTest();

      //RunHasRelationChangedTest();

      BindableObjectWithSecurityTest();
      BindableObjectWithoutSecurityTest();

      setUpFixture.TearDown();

      Console.WriteLine ("Test compelte");
      //Console.ReadLine();
    }

    private static void RunHasRelationChangedTest ()
    {
      var test = new HasRelationChangedTest();
      test.TestFixtureSetUp();

      test.SetUp();
      test.AskChanged();
      test.TearDown();

      test.TestFixtureTearDown();
    }

    private static void RunLoadObjectsTest ()
    {
      var test = new LoadObjectsTest();
      test.TestFixtureSetUp();

      test.SetUp();
      test.LoadObjectsOverRelationTest();
      test.TearDown();

      test.SetUp();
      test.LoadObjectsOverRelationWithAbstractBaseClass();
      test.TearDown();

      test.TestFixtureTearDown();
    }

    private static void RunSerializationTest ()
    {
      var test = new SerializationTest();
      test.TestFixtureSetUp();

      var testMethods = from m in test.GetType().GetMethods (BindingFlags.Public | BindingFlags.Instance)
                        where m.IsDefined (typeof (TestAttribute), true) && !m.IsDefined (typeof (IgnoreAttribute), true)
                        orderby m.Name
                        select m;
      foreach (MethodInfo testMethod in testMethods)
      {
        test.SetUp();
        testMethod.Invoke (test, new object[0]);
        test.TearDown();
      }

      test.TestFixtureTearDown();
    }

    private static void BindableObjectWithSecurityTest ()
    {
      var test = new BindableObjectWithSecurityTest();

      test.SetUp();
      test.BusinessObject_Property_IsAccessible();
      test.BusinessObject_GetProperty();
      test.Reflection_GetProperty ();
      test.DomainObject_GetProperty ();
      test.TearDown();
    }

    private static void BindableObjectWithoutSecurityTest ()
    {
      var test = new BindableObjectWithoutSecurityTest();

      test.SetUp();
      test.BusinessObject_Property_IsAccessible();
      test.BusinessObject_GetProperty();
      test.Reflection_GetProperty ();
      test.DomainObject_GetProperty ();
      test.TearDown();
    }
  }
}