// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextCombinerTest
  {
    private ClassContextCombiner _combiner;
    private ClassContext _context1;
    private ClassContext _context2;

    [SetUp]
    public void SetUp ()
    {
      _combiner = new ClassContextCombiner ();
      _context1 = new ClassContext (typeof (object), new MixinContext[0], new Type[] { typeof (int), typeof (float) });
      _context2 = new ClassContext (typeof (string), new Type[] { typeof (double), typeof (int) });
    }

    [Test]
    public void Add_NonNull ()
    {
      _combiner.Add (_context1);
      _combiner.Add (_context2);
      Assert.That (_combiner.CollectedContexts, Is.EquivalentTo (new object[] {_context1, _context2}));
    }

    [Test]
    public void Add_Null ()
    {
      _combiner.Add (null);
      Assert.That (_combiner.CollectedContexts, Is.EquivalentTo (new object[0]));
    }

    [Test]
    public void GetCombinedContexts_Null ()
    {
      Assert.IsNull (_combiner.GetCombinedContexts(typeof (int)));
    }

    [Test]
    public void GetCombinedContexts_One ()
    {
      _combiner.Add (_context1);
      ClassContext result = _combiner.GetCombinedContexts (typeof (int));
      Assert.AreEqual (typeof (int), result.Type);
      Assert.That (result.CompleteInterfaces, Is.EquivalentTo (_context1.CompleteInterfaces));
    }

    [Test]
    public void GetCombinedContexts_Many ()
    {
      _combiner.Add (_context1);
      _combiner.Add (_context2);

      ClassContext result = _combiner.GetCombinedContexts (typeof (int));
      Assert.AreEqual (typeof (int), result.Type);
      
      Set<Type> expectedInterfaces = new Set<Type> (_context1.CompleteInterfaces);
      expectedInterfaces.AddRange (_context2.CompleteInterfaces);
      Assert.That (result.CompleteInterfaces, Is.EquivalentTo (expectedInterfaces));
    }
  }
}
