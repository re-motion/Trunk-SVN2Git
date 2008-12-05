// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      _mockRepository.StrictMock<IBusinessObject> ();
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
      IBusinessObject mockBusinessObject  =_mockRepository.StrictMock<IBusinessObject>();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.StrictMock<IBusinessObjectEnumerationProperty>();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value1, "Value1", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.False);
    }

    [Test]
    public void IsEnabled_WithTrue ()
    {
      IBusinessObject mockBusinessObject = _mockRepository.StrictMock<IBusinessObject> ();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.StrictMock<IBusinessObjectEnumerationProperty> ();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll ();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value2, "Value2", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }
  }
}
