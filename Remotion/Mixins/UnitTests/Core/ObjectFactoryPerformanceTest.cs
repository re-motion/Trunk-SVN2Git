// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain.GeneratedTypes;
using Remotion.Reflection;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  [Explicit ("Performance tests")]
  public class ObjectFactoryPerformanceTest
  {
    [Test]
    public void New_WithMixedMixin ()
    {
      const int iterations = 2000000;

      int acc = 0;
      var stopwatch = Stopwatch.StartNew ();
      for (int i = 0; i < iterations; ++i)
      {
        var instance = new ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation();
        acc ^= instance.GetHashCode ();
      }
      stopwatch.Stop();

      Console.WriteLine (acc);
 
      var elapsed = stopwatch.Elapsed;
      Console.WriteLine ("New_WithMixedMixin: {0} µs", elapsed.TotalMilliseconds * 1000.0 / iterations);
    }

    [Test]
    public void ObjectFactoryCreate_WithMixedMixin ()
    {
      const int iterations = 200000;

      ObjectFactory.Create<ClassWithMixedMixin> (ParamList.Empty);

      int acc = 0;
      var stopwatch = Stopwatch.StartNew ();
      for (int i = 0; i < iterations; ++i)
      {
        var instance = ObjectFactory.Create<ClassWithMixedMixin> (ParamList.Empty);
        acc ^= instance.GetHashCode ();
      }
      stopwatch.Stop ();

      Console.WriteLine (acc);

      var elapsed = stopwatch.Elapsed;
      Console.WriteLine ("ObjectFactoryCreate_WithMixedMixin: {0} µs", elapsed.TotalMilliseconds * 1000.0 / iterations);
    }
  }
}