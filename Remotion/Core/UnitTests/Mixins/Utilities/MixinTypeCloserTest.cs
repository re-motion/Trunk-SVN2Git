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
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixinTypeCloserTest
  {
    [Test]
    public void GetClosedMixinType_NonGenericMixin()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (object));
      Assert.AreEqual (typeof (object), t);
    }

    [Test]
    public void GetClosedMixinType_BindToConstraints ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin6<,>));
      Assert.AreEqual (typeof (BT3Mixin6<IBT3Mixin6ThisDependencies, IBT3Mixin6BaseDependencies>), t);
    }

    [Test]
    public void GetClosedMixinType_BindToTargetType ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin3<,>));
      Assert.AreEqual (typeof (BT3Mixin3<BaseType3, IBaseType33>), t);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The generic mixin "
        + "'Remotion.UnitTests.Mixins.SampleTypes.GenericMixinWithUnsatisfiableConstraints`1[T]' applied to class "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BaseType3' cannot be automatically closed because the constraints of its type parameter 'T' cannot "
        + "be unified into a single type. The generic type parameter has inconclusive constraints 'System.ICloneable' "
        + "and 'System.Runtime.Serialization.ISerializable', which cannot be unified into a single type.")]
    public void GetClosedMixinType_MixinWithUnsatisfiableConstraintsThrows ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithUnsatisfiableConstraints<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The generic mixin "
        + "'Remotion.UnitTests.Mixins.SampleTypes.GenericMixinWithoutBindingInformation`1[T]' applied to class "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BaseType3' cannot be automatically closed because its type parameter 'T' does not have any binding "
        + "information. Apply the BindToTargetTypeAttribute or the BindToConstraintsAttribute to the type parameter or specify the parameter's "
        + "instantiation when configuring the mixin for the target class.")]
    public void GetClosedMixinType_MixinWithoutBindingInformationThrows ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithoutBindingInformation<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Cannot close the generic mixin type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BT3Mixin3`2[TThis,TBase]' applied to class 'Remotion.UnitTests.Mixins.SampleTypes.BaseType1' - "
        + "the inferred type arguments violate the generic parameter constraints. Specify the arguments manually, modify the parameter binding "
        + "specification, or relax the constraints. GenericArguments[0], 'Remotion.UnitTests.Mixins.SampleTypes.BaseType1', on "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BT3Mixin3`2[TThis,TBase]' violates the constraint of type 'TThis'.")]
    public void GetClosedMixinType_BindToInvalidTargetType ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (BT3Mixin3<,>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Type parameter 'T' of mixin "
        + "'Remotion.UnitTests.Mixins.SampleTypes.GenericMixinWithDoubleBindingInformation`1[T]' has more than one binding specification.")]
    public void GetClosedMixinType_BindToTargetAndConstraints ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithDoubleBindingInformation<>));
    }
  }
}
