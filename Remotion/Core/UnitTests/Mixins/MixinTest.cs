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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinTest
  {
    [Test]
    public void MixinGet_FindsMixinInstanceInTarget ()
    {
      var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      var mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.That (mixin, Is.Not.Null);
    }

    [Test]
    public void MixinGet_ReturnsNullIfMixinNotFound ()
    {
      var mixin = Mixin.Get<BT3Mixin2> (new object());
      Assert.That (mixin, Is.Null);
    }

    [Test]
    public void MixinGet_FindsMixinWithAssignableMatch ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var mixin = Mixin.Get<IBT1Mixin1> (bt1);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin, Is.InstanceOfType (typeof (BT1Mixin1)));
    }

    [Test]
    public void MixinGet_FindsMixinWithGenericMatch ()
    {
      BaseType3 bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.That (mixin, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Both mixins 'Remotion.UnitTests.Mixins.SampleTypes."
        + "DerivedDerivedNullMixin' and 'Remotion.UnitTests.Mixins.SampleTypes.DerivedNullMixin' match the given type 'NullMixin'.")]
    public void MixinGet_AssignableMatchAmbiguity ()
    {
      using (MixinConfiguration.BuildNew().ForClass<NullTarget>().AddMixin<DerivedNullMixin>().AddMixin<DerivedDerivedNullMixin>().EnterScope())
      {
        var instance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Mixin.Get<NullMixin> (instance);
      }
    }
  }
}