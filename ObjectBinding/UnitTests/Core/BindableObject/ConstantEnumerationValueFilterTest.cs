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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ConstantEnumerationValueFilterTest : EnumerationTestBase
  {
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _mockRepository.CreateMock<IBusinessObject> ();
    }
    
    [Test]
    public void Initialize ()
    {
      Enum[] expected = new Enum[] {TestEnum.Value1, TestEnum.Value2};
      ConstantEnumerationValueFilter filter = new ConstantEnumerationValueFilter (expected);

      Assert.That (filter.DisabledEnumValues, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException))]
    public void Initialize_WithMismatchedEnumValues ()
    {
      new ConstantEnumerationValueFilter (new Enum[] {TestEnum.Value1, EnumWithUndefinedValue.Value2});
    }

    [Test]
    public void IsEnabled_WithFalse ()
    {
      IBusinessObject mockBusinessObject  =_mockRepository.CreateMock<IBusinessObject>();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.CreateMock<IBusinessObjectEnumerationProperty>();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value1, "Value1", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.False);
    }

    [Test]
    public void IsEnabled_WithTrue ()
    {
      IBusinessObject mockBusinessObject = _mockRepository.CreateMock<IBusinessObject> ();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.CreateMock<IBusinessObjectEnumerationProperty> ();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll ();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value2, "Value2", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }
  }
}
