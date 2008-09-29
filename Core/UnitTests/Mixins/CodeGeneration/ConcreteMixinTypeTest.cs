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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeTest
  {
    private ConcreteMixinType _concreteMixinType;
    private MethodInfo _method1;
    private MethodInfo _method2;

    private TargetClassDefinition _simpleClassDefinition;
    private MixinDefinition _simpleMixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _simpleClassDefinition = new TargetClassDefinition (new ClassContext (typeof (BaseType1), typeof (BT1Mixin1)));
      _simpleMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (BT1Mixin1), _simpleClassDefinition, false);

      _concreteMixinType = new ConcreteMixinType (_simpleMixinDefinition, typeof (object));
      _method1 = typeof (object).GetMethod ("ToString");
      _method2 = typeof (object).GetMethod ("Equals", BindingFlags.Instance | BindingFlags.Public);
    }

    [Test]
    public void AddMethodWrapper ()
    {
      _concreteMixinType.AddMethodWrapper (_method1, _method2);
      Assert.That (_concreteMixinType.GetMethodWrapper (_method1), Is.SameAs (_method2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A public wrapper for method 'System.Object.ToString' was already added.")]
    public void AddMethodWrapper_Twice ()
    {
      _concreteMixinType.AddMethodWrapper (_method1, _method2);
      _concreteMixinType.AddMethodWrapper (_method1, _method2);
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "No public wrapper was generated for method 'System.Object.ToString'.")]
    public void GetMethodWrapper_NotFound ()
    {
      _concreteMixinType.GetMethodWrapper (_method1);
    }
  }
}