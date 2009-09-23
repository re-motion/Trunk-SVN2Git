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
using Remotion.Mixins.Context;
using System.Linq;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class DefaultMixinInheritancePolicyTest
  {
    private DefaultMixinInheritancePolicy _policy;

    [SetUp]
    public void SetUp ()
    {
      _policy = DefaultMixinInheritancePolicy.Instance;
    }

    [Test]
    public void GetTypesToInheritFrom_None ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (object)).ToArray (), Is.Empty);
    }

    [Test]
    public void GetTypesToInheritFrom_Base ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (string)).ToArray (), List.Contains (typeof (object)));
    }

    [Test]
    public void GetTypesToInheritFrom_Interfaces ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (string)).ToArray (), 
          List.Contains (typeof (IEnumerable<char>)));
    }

    [Test]
    public void GetTypesToInheritFrom_GenericTypeDef ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (List<int>)).ToArray (), List.Contains (typeof (List<>)));
    }

    [Test]
    public void GetTypesToInheritFrom_NoGenericTypeDef_ForOpenGenericType ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (List<>)).ToArray (), List.Not.Contains (typeof (List<>)));
    }

    [Test]
    public void GetClassContextsToInheritFrom ()
    {
      var fakeClassContext = new ClassContext (typeof (object));
      var result = _policy.GetClassContextsToInheritFrom (typeof (BaseType1), t => fakeClassContext);

      Assert.That (result.ToArray (), Is.EqualTo (new[] { fakeClassContext }));
    }
  }
}
