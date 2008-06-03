/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class ConvertFromNativePropertyType : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();

      _mockRepository = new MockRepository();
      _mockRepository.CreateMock<IBusinessObject>();
    }

    [Test]
    public void ValidValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (TestEnum.Value1), Is.EqualTo (TestEnum.Value1));
    }

    [Test]
    public void Null ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (null), Is.Null);
    }

    [Test]
    public void UndefinedValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (EnumWithUndefinedValue.UndefinedValue), Is.Null);
    }

    [Test]
    public void InvalidEnumValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType ((TestEnum) (-1)), Is.EqualTo ((TestEnum) (-1)));
    }

    [Test]
    public void EnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.ConvertFromNativePropertyType (EnumWithUndefinedValue.Value1);
      Assert.That (property.ConvertFromNativePropertyType (EnumWithUndefinedValue.Value1), Is.EqualTo (EnumWithUndefinedValue.Value1));
    }
    
    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}
