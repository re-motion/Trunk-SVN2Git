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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using System.Reflection;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeTest
  {
    private ConcreteMixinType _concreteMixinType;
    private MethodInfo _method1;
    private MethodInfo _method2;

    [SetUp]
    public void SetUp ()
    {
      _concreteMixinType = new ConcreteMixinType (typeof (object));
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
