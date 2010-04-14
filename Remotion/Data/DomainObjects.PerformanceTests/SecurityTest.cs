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
using NUnit.Framework;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.Security.Configuration;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class SecurityTest
  {
    public const int TestRepititions = 100*1000;

    [SetUp]
    public void SetUp ()
    {
      SecurityConfiguration.Current.SecurityProvider = new StubSecurityProvider();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void BusinessObject_Property_IsAccessible ()
    {
      Console.WriteLine ("Expected average duration of SecurityTest for BusinessObject_Property_IsAccessible on reference system: ~6.5 µs (release build), ~10.5 µs (debug build)");

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
        var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

        Assert.That (property.IsAccessible (obj.BusinessObjectClass, obj));

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < TestRepititions; i++)
          property.IsAccessible (obj.BusinessObjectClass, obj);
        stopwatch.Stop();

        double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
        Console.WriteLine (
            "BusinessObject_Property_IsAccessible (executed {0}x): Average duration: {1} µs",
            TestRepititions,
            averageMilliSeconds.ToString ("N"));
      }
    }
  }
}