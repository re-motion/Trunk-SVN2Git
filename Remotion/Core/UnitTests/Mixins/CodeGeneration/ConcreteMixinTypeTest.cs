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
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      _method1 = typeof (object).GetMethod ("ToString");
      _method2 = typeof (object).GetMethod ("Equals", BindingFlags.Instance | BindingFlags.Public);
      _concreteMixinType = new ConcreteMixinType (
          identifier, 
          typeof (object), 
          typeof (IServiceProvider), 
          new Dictionary<MethodInfo, MethodInfo> { { _method1, _method2 } },
          new Dictionary<MethodInfo, MethodInfo> { { _method1, _method2 } });
    }

    [Test]
    public void GetMethodWrapper ()
    {
      Assert.That (_concreteMixinType.GetMethodWrapper (_method1), Is.SameAs (_method2));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "No public wrapper was generated for method 'System.Object.Equals'.")]
    public void GetMethodWrapper_NotFound ()
    {
      _concreteMixinType.GetMethodWrapper (_method2);
    }

    [Test]
    public void GetOverrideInterfaceMethod ()
    {
      Assert.That (_concreteMixinType.GetOverrideInterfaceMethod (_method1), Is.SameAs (_method2));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "No override interface method was generated for method 'System.Object.Equals'.")]
    public void GetOverrideInterfaceMethod_NotFound ()
    {
      _concreteMixinType.GetOverrideInterfaceMethod (_method2);
    }

  }
}
