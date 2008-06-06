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
using System.Threading;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionCacheTest
  {
    [SetUp]
    [TearDown]
    public void ResetCache ()
    {
      TargetClassDefinitionCache.SetCurrent (null);
    }

    [Test]
    public void IsCached()
    {
      Assert.IsFalse (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
      TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test (Description = "Checks whether the test fixture correctly resets the cache before running the test.")]
    public void IsCached2 ()
    {
      Assert.IsFalse (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test]
    public void GetTargetClassDefinitionReturnsValidClassDef ()
    {
      ClassContext context = new ClassContext (typeof (BaseType1));
      TargetClassDefinition def = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
      Assert.IsNotNull (def);
      Assert.AreSame (context, def.ConfigurationContext);
    }

    [Test]
    public void GetTargetClassDefinitionImplementsCaching ()
    {
      TargetClassDefinition def = TargetClassDefinitionCache.Current.GetTargetClassDefinition(new ClassContext (typeof (BaseType1)));
      TargetClassDefinition def2 = TargetClassDefinitionCache.Current.GetTargetClassDefinition(new ClassContext (typeof (BaseType1)));
      Assert.IsNotNull (def);
      Assert.AreSame (def, def2);
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void CacheValidatesWhenGeneratingDefinition()
    {
      ClassContext cc = new ClassContext (typeof (DateTime));
      TargetClassDefinitionCache.Current.GetTargetClassDefinition (cc);
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      TargetClassDefinitionCache newCache = new TargetClassDefinitionCache ();
      Assert.IsFalse (TargetClassDefinitionCache.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { TargetClassDefinitionCache.SetCurrent (newCache); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (TargetClassDefinitionCache.HasCurrent);
      Assert.AreSame (newCache, TargetClassDefinitionCache.Current);
    }
  }
}
