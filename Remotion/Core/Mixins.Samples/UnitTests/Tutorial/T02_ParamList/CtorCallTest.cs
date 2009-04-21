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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Samples.Tutorial.T02_ParamList;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.Tutorial.T02_ParamList
{
  [TestFixture]
  public class CtorCallTest
  {
    [Test]
    [ExpectedException (typeof (MissingMethodException))] // looks for a ctor with five string arguments
    public void Activator_BrokenCalls_ArrayPassed ()
    {
      var theObject1 = (TheClass) Activator.CreateInstance (typeof (TheClass), new[] { "my", "home", "is", "my", "castle" });
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("Many strings: my, home, is, my, castle"));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))] // looks for a default ctor
    public void Activator_BrokenCalls_SingleNullPassed ()
    {
      var theObject1 = (TheClass) Activator.CreateInstance (typeof (TheClass), null);
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("Not even one string."));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException))]
    public void Activator_BrokenCalls_AmbiguousNullPassed ()
    {
      var theObject1 = (TheClass) Activator.CreateInstance (typeof (TheClass), new object[] { null });
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("One null string"));
    }

    [Test]
    public void ParamList_ArrayPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass> (ParamList.Create (new[] { "my", "home", "is", "my", "castle" }));
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("Many strings: my, home, is, my, castle"));
    }

    [Test]
    public void ParamList_SingleNullPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass> (ParamList.Create ((string) null));
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("Not even one string."));
    }

    [Test]
    public void ParamList_OtherNullPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass> (ParamList.Create ((string[]) null));
      Assert.That (theObject1.ConstructionInfo, Is.EqualTo ("Not many strings."));
    }

  }
}