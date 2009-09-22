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
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixinGenericArgumentFinderTest
  {
    [Test]
    public void ThisArgumentFinder_Find ()
    {
      var thisArgument = MixinGenericArgumentFinder.ThisArgumentFinder.FindGenericArgument (typeof (BT3Mixin4));
      Assert.That (thisArgument, Is.SameAs (typeof (BaseType3)));
    }

    [Test]
    public void ThisArgumentFinder_Find_NoMixinBase ()
    {
      var thisArgument = MixinGenericArgumentFinder.ThisArgumentFinder.FindGenericArgument (typeof (object));
      Assert.That (thisArgument, Is.Null);
    }

    [Test]
    public void BaseArgumentFinder_Find ()
    {
      var baseArgument = MixinGenericArgumentFinder.BaseArgumentFinder.FindGenericArgument (typeof (BT3Mixin4));
      Assert.That (baseArgument, Is.SameAs (typeof (IBaseType34)));
    }

    [Test]
    public void BaseArgumentFinder_Find_NoMixinBase ()
    {
      var baseArgument = MixinGenericArgumentFinder.BaseArgumentFinder.FindGenericArgument (typeof (object));
      Assert.That (baseArgument, Is.Null);
    }

    [Test]
    public void BaseArgumentFinder_Find_NoBaseArgument ()
    {
      var baseArgument = MixinGenericArgumentFinder.BaseArgumentFinder.FindGenericArgument (typeof (BT3Mixin2));
      Assert.That (baseArgument, Is.Null);
    }
  }
}