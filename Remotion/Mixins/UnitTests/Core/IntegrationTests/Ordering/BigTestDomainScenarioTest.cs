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
using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering
{
  [TestFixture]
  public class BigTestDomainScenarioTest
  {
    [Test]
    public void MixinDefinitionsAreSortedCorrectlySmall ()
    {
      TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      Assert.That (bt7.Mixins.Count, Is.EqualTo (7));
      // group 1
      Assert.That (bt7.Mixins[typeof (BT7Mixin0)].MixinIndex, Is.EqualTo (0));

      Assert.That (bt7.Mixins[typeof (BT7Mixin2)].MixinIndex, Is.EqualTo (1));
      Assert.That (bt7.Mixins[typeof (BT7Mixin3)].MixinIndex, Is.EqualTo (2));
      Assert.That (bt7.Mixins[typeof (BT7Mixin1)].MixinIndex, Is.EqualTo (3));

      // group 2
      Assert.That (bt7.Mixins[typeof (BT7Mixin10)].MixinIndex, Is.EqualTo (4));
      Assert.That (bt7.Mixins[typeof (BT7Mixin9)].MixinIndex, Is.EqualTo (5));

      // group 3
      Assert.That (bt7.Mixins[typeof (BT7Mixin5)].MixinIndex, Is.EqualTo (6));
    }

    [Test]
    public void MixinDefinitionsAreSortedCorrectlyGrand ()
    {
      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin0),
              typeof (BT7Mixin1),
              typeof (BT7Mixin2),
              typeof (BT7Mixin3),
              typeof (BT7Mixin4),
              typeof (BT7Mixin5),
              typeof (BT7Mixin6),
              typeof (BT7Mixin7),
              typeof (BT7Mixin8),
              typeof (BT7Mixin9),
              typeof (BT7Mixin10))
          .EnterScope())
      {
        CheckGrandOrdering();
      }

      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin10),
              typeof (BT7Mixin9),
              typeof (BT7Mixin8),
              typeof (BT7Mixin7),
              typeof (BT7Mixin6),
              typeof (BT7Mixin5),
              typeof (BT7Mixin4),
              typeof (BT7Mixin3),
              typeof (BT7Mixin2),
              typeof (BT7Mixin1),
              typeof (BT7Mixin0))
          .EnterScope())
      {
        CheckGrandOrdering();
      }
      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin5),
              typeof (BT7Mixin8),
              typeof (BT7Mixin9),
              typeof (BT7Mixin2),
              typeof (BT7Mixin1),
              typeof (BT7Mixin10),
              typeof (BT7Mixin4),
              typeof (BT7Mixin0),
              typeof (BT7Mixin6),
              typeof (BT7Mixin3),
              typeof (BT7Mixin7))
          .EnterScope())
      {
        CheckGrandOrdering();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The mixins applied to target class '.*BaseType7' cannot be ordered. The following mixins "
        + "require a clear base call ordering, but do not provide enough dependency information:\r\n"
        + "(('.*BT7Mixin0'(,\r\n)?)|('.*BT7Mixin4'(,\r\n)?)|('.*BT7Mixin6'(,\r\n)?)|('.*BT7Mixin7'(,\r\n)?)){4}\\.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfConnectedMixinsCannotBeSorted ()
    {
      using (
          MixinConfiguration
              .BuildFromActive ()
              .ForClass<BaseType7> ()
              .Clear ()
              .AddMixins (typeof (BT7Mixin0), typeof (BT7Mixin4), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin2), typeof (BT7Mixin5))
              .EnterScope ())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      }
    }

    private void CheckGrandOrdering ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType7> ()
          .EnsureMixin (typeof (BT7Mixin0)).WithDependency<IBT7Mixin7> ()
          .EnsureMixin (typeof (BT7Mixin7)).WithDependency<IBT7Mixin4> ()
          .EnsureMixin (typeof (BT7Mixin4)).WithDependency<IBT7Mixin6> ()
          .EnsureMixin (typeof (BT7Mixin6)).WithDependency<IBT7Mixin2> ()
          .EnsureMixin (typeof (BT7Mixin9)).WithDependency<IBT7Mixin8> ()
          .EnterScope ())
      {
        TargetClassDefinition bt7 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
        Assert.That (bt7.Mixins.Count, Is.EqualTo (11));
        // group 1
        Assert.That (bt7.Mixins[typeof (BT7Mixin0)].MixinIndex, Is.EqualTo (0)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin7)].MixinIndex, Is.EqualTo (1)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin4)].MixinIndex, Is.EqualTo (2)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin6)].MixinIndex, Is.EqualTo (3)); // u

        Assert.That (bt7.Mixins[typeof (BT7Mixin2)].MixinIndex, Is.EqualTo (4));
        Assert.That (bt7.Mixins[typeof (BT7Mixin3)].MixinIndex, Is.EqualTo (5));
        Assert.That (bt7.Mixins[typeof (BT7Mixin1)].MixinIndex, Is.EqualTo (6));

        // group 2
        Assert.That (bt7.Mixins[typeof (BT7Mixin10)].MixinIndex, Is.EqualTo (7));
        Assert.That (bt7.Mixins[typeof (BT7Mixin9)].MixinIndex, Is.EqualTo (8)); // u
        Assert.That (bt7.Mixins[typeof (BT7Mixin8)].MixinIndex, Is.EqualTo (9)); // u

        // group 3
        Assert.That (bt7.Mixins[typeof (BT7Mixin5)].MixinIndex, Is.EqualTo (10));
      }
    }
  }
}