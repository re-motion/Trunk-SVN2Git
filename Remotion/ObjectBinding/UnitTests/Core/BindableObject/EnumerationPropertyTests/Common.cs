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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class Common : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();

      _mockRepository = new MockRepository();
      _mockRepository.StrictMock<IBusinessObject>();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property 'NullableScalar' defined on type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithValueType`1[Remotion.ObjectBinding.UnitTests.Core.TestDomain.EnumWithUndefinedValue]'"
        + " must not be nullable since the property's type already defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute'.")]
    public void Initialize_NullableWithUndefinedValue ()
    {
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "NullableScalar");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property 'Scalar' defined on type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithValueType`1[Remotion.ObjectBinding.UnitTests.Core.TestDomain.TestFlags]'"
        + " is a flags-enum, which is not supported.")]
    public void Initialize_FlagsEnum ()
    {
      CreateProperty (typeof (ClassWithValueType<TestFlags>), "Scalar");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The enum type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.EnumWithUndefinedValueFromOtherType' "
        + "defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute' with an enum value that belongs to a different enum type.")]
    public void Initialize_WithUndefinedEnumValueFromOtherType ()
    {
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValueFromOtherType>), "Scalar");
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
          GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}
