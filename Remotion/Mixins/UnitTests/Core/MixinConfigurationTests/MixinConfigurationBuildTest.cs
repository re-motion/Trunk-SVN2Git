// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using NUnit.Framework;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationBuildTest
  {
    [Test]
    public void BuildNew ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      using (MixinConfiguration.BuildNew ().ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsNotNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType7)).Mixins.ContainsKey (typeof (BT7Mixin0)));
        Assert.IsNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFrom ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass <BaseType2>().AddMixin (typeof (BT2Mixin1)).BuildConfiguration();
      
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      using (MixinConfiguration.BuildFrom (parentConfiguration).ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.AreNotSame (parentConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsNotNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType7)).Mixins.ContainsKey (typeof (BT7Mixin0)));
        Assert.IsNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1)));
        Assert.IsNotNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType2)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType4> ().AddMixin<BT4Mixin1> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsNotNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1)).Mixins.ContainsKey (typeof (BT1Mixin1)));
        Assert.IsNotNull (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType4)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType4)).Mixins.ContainsKey (typeof (BT4Mixin1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive_CausesDefaultConfigurationToBeAnalyzed ()
    {
      MixinConfiguration.SetActiveConfiguration (null);
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));
      }
      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
    }

  }
}
