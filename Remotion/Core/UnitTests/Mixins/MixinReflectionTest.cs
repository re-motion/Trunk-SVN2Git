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
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinReflectionTest
  {
    [Test]
    public void IMixinTarget ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly());

      using (context.EnterScope())
      {
        var mixinTarget = (IMixinTarget) ObjectFactory.Create<BaseType1> (ParamList.Empty);

        TargetClassDefinition configuration = mixinTarget.Configuration;
        Assert.That (configuration, Is.Not.Null);

        Assert.That (configuration, Is.SameAs (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1))));

        object[] mixins = mixinTarget.Mixins;
        Assert.That (mixins, Is.Not.Null);
        Assert.That (mixins.Length, Is.EqualTo (configuration.Mixins.Count));
      }
    }

    [Test]
    public void GetTargetProperty ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (MixinReflector.GetTargetProperty (m1.Type), Is.Null);

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.That (MixinReflector.GetTargetProperty (m2.Type), Is.Not.Null);
      Assert.That (
          MixinReflector.GetTargetProperty (m2.Type),
          Is.EqualTo (typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.That (MixinReflector.GetTargetProperty (m3.Type), Is.Not.Null);
      Assert.That (
          MixinReflector.GetTargetProperty (m3.Type),
          Is.EqualTo (typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.That (MixinReflector.GetTargetProperty (m4.Type), Is.Not.Null);
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));
      Assert.That (
          MixinReflector.GetTargetProperty (m4.Type),
          Is.EqualTo (m4.Type.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (
          MixinReflector.GetTargetProperty (m4.Type),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetBaseProperty ()
    {
      MixinDefinition m1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (MixinReflector.GetBaseProperty (m1.Type), Is.Null);

      MixinDefinition m2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.That (MixinReflector.GetBaseProperty (m2.Type), Is.Not.Null);
      Assert.That (
          MixinReflector.GetBaseProperty (m2.Type),
          Is.EqualTo (
              typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));

      MixinDefinition m3 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.That (MixinReflector.GetBaseProperty (m3.Type), Is.Null);

      MixinDefinition m4 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.That (MixinReflector.GetBaseProperty (m4.Type), Is.Not.Null);
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m4.Type));
      Assert.That (
          MixinReflector.GetBaseProperty (m4.Type),
          Is.EqualTo (
              m4.Type.BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));
      Assert.That (
          MixinReflector.GetBaseProperty (m4.Type),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetMixinBaseCallProxyType ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Type bcpt = MixinReflector.GetBaseCallProxyType (bt1);
      Assert.That (bcpt, Is.Not.Null);
      Assert.That (bcpt, Is.EqualTo (bt1.GetType().GetNestedType ("BaseCallProxy")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not a mixin target", MatchType = MessageMatch.Contains)]
    public void GetMixinBaseCallProxyTypeThrowsIfWrongType1 ()
    {
      MixinReflector.GetBaseCallProxyType (new object());
    }
  }
}
