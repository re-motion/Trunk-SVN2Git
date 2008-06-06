/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextCollectorTest
  {
    private ClassContextCollector _collector;
    private ClassContext _context1;
    private ClassContext _context2;

    [SetUp]
    public void SetUp ()
    {
      _collector = new ClassContextCollector ();
      _context1 = new ClassContext (typeof (object), new MixinContext[0], new Type[] { typeof (int), typeof (float) });
      _context2 = new ClassContext (typeof (string), new Type[] { typeof (double), typeof (int) });
    }

    [Test]
    public void Add_NonNull ()
    {
      _collector.Add (_context1);
      _collector.Add (_context2);
      Assert.That (_collector.CollectedContexts, Is.EquivalentTo (new object[] {_context1, _context2}));
    }

    [Test]
    public void Add_Null ()
    {
      _collector.Add (null);
      Assert.That (_collector.CollectedContexts, Is.EquivalentTo (new object[0]));
    }

    [Test]
    public void GetCombinedContexts_Null ()
    {
      Assert.IsNull (_collector.GetCombinedContexts(typeof (int)));
    }

    [Test]
    public void GetCombinedContexts_One ()
    {
      _collector.Add (_context1);
      ClassContext result = _collector.GetCombinedContexts (typeof (int));
      Assert.AreEqual (typeof (int), result.Type);
      Assert.That (result.CompleteInterfaces, Is.EquivalentTo (_context1.CompleteInterfaces));
    }

    [Test]
    public void GetCombinedContexts_Many ()
    {
      _collector.Add (_context1);
      _collector.Add (_context2);

      ClassContext result = _collector.GetCombinedContexts (typeof (int));
      Assert.AreEqual (typeof (int), result.Type);
      
      Set<Type> expectedInterfaces = new Set<Type> (_context1.CompleteInterfaces);
      expectedInterfaces.AddRange (_context2.CompleteInterfaces);
      Assert.That (result.CompleteInterfaces, Is.EquivalentTo (expectedInterfaces));
    }
  }
}
