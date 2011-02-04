// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GenericsTest : CodeGenerationBaseTest
  {
    [Test]
    public void GenericMixinsAreSpecialized ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>));
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.IsNotNull (mixin);

      PropertyInfo targetProperty = MixinReflector.GetTargetProperty (mixin.GetType ());
      Assert.IsNotNull (targetProperty);

      Assert.IsNotNull (targetProperty.GetValue (mixin, null));
      Assert.AreSame (bt3, targetProperty.GetValue (mixin, null));
      Assert.AreEqual (typeof (BaseType3), targetProperty.PropertyType);

      PropertyInfo nextProperty = MixinReflector.GetNextProperty (mixin.GetType ());
      Assert.IsNotNull (nextProperty);

      Assert.IsNotNull (nextProperty.GetValue (mixin, null));
      Assert.AreSame (bt3.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, nextProperty.GetValue (mixin, null).GetType ());
      Assert.AreEqual (typeof (IBaseType33), nextProperty.PropertyType);
    }

    [Test]
    public void MuchGenericityWithoutOverriding ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (VeryGenericMixin<,>), typeof (BT3Mixin4));
      var m = bt3 as IVeryGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
    }

    [Test]
    public void MuchGenericityWithOverriding ()
    {
      var cougs = CreateMixedObject<ClassOverridingUltraGenericStuff> (typeof (AbstractDerivedUltraGenericMixin<,>), typeof (BT3Mixin4));
      var m = cougs as IUltraGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("String-IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
    }

  }
}
