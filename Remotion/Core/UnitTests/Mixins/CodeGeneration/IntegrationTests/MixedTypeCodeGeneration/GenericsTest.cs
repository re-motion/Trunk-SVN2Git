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
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

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

      PropertyInfo thisProperty = mixin.GetType ().BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (thisProperty);

      Assert.IsNotNull (thisProperty.GetValue (mixin, null));
      Assert.AreSame (bt3, thisProperty.GetValue (mixin, null));
      Assert.AreEqual (typeof (BaseType3), thisProperty.PropertyType);

      PropertyInfo baseProperty = mixin.GetType ().BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (baseProperty);

      Assert.IsNotNull (baseProperty.GetValue (mixin, null));
      Assert.AreSame (bt3.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, baseProperty.GetValue (mixin, null).GetType ());
      Assert.AreEqual (typeof (IBaseType33), baseProperty.PropertyType);
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
