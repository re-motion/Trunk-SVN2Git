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

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class BindableObjectTestBase
  {
    public const int TestRepititions = 100 * 1000;

    public virtual void BusinessObject_Property_IsAccessible ()
    {
        var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
        var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

        bool value = true;

        Assert.That (property.IsAccessible (obj.BusinessObjectClass, obj));

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < TestRepititions; i++)
          value ^= property.IsAccessible (obj.BusinessObjectClass, obj);
        stopwatch.Stop();

        Console.WriteLine (value);

        double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
        Console.WriteLine (
            "BusinessObject_Property_IsAccessible (executed {0}x): Average duration: {1} µs",
            TestRepititions,
            averageMilliSeconds.ToString ("N"));
    }

    public virtual void BusinessObject_GetProperty ()
    {
      var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
        ((ObjectWithSecurity) obj).TheProperty = "value";
        var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

        bool value = false;

        Assert.That (property.IsAccessible (obj.BusinessObjectClass, obj));

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < TestRepititions; i++)
          value ^= obj.GetProperty (property) == null;
        stopwatch.Stop();

        Console.WriteLine (value);

        double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
        Console.WriteLine (
            "BusinessObject_GetProperty (executed {0}x): Average duration: {1} µs",
            TestRepititions,
            averageMilliSeconds.ToString ("N"));
    }
  }
}