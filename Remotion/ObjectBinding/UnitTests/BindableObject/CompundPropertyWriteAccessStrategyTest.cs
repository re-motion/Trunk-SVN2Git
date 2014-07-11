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
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class CompundPropertyWriteAccessStrategyTest : TestBase
  {
    private MockRepository _mockRepository;
    private CompundBindablePropertyWriteAccessStrategy _strategy;
    private IBindablePropertyWriteAccessStrategy _innerStrategy1;
    private IBindablePropertyWriteAccessStrategy _innerStrategy2;
    private IBindablePropertyWriteAccessStrategy _innerStrategy3;
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();

      _innerStrategy1 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();
      _innerStrategy2 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();
      _innerStrategy3 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();

      _strategy = new CompundBindablePropertyWriteAccessStrategy (new[] { _innerStrategy1, _innerStrategy2, _innerStrategy3 });

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_strategy.BindablePropertyWriteAccessStrategies, Is.EqualTo (new[] { _innerStrategy1, _innerStrategy2, _innerStrategy3 }));
    }

    [Test]
    public void CanWrite_WithoutStrategies_ReturnsTrue ()
    {
      var property = new StubPropertyBase (GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), _businessObjectProvider));
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();

      var strategy = new CompundBindablePropertyWriteAccessStrategy (Enumerable.Empty<IBindablePropertyWriteAccessStrategy>());
      var result = strategy.CanWrite (property, businessObject);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanWrite_WithAllStrategiesReturingTrue_ReturnsTrue ()
    {
      var property = new StubPropertyBase (GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), _businessObjectProvider));
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();

      using (_mockRepository.Ordered())
      {
        _innerStrategy1.Expect (mock => mock.CanWrite (property, businessObject)).Return (true);
        _innerStrategy2.Expect (mock => mock.CanWrite (property, businessObject)).Return (true);
        _innerStrategy3.Expect (mock => mock.CanWrite (property, businessObject)).Return (true);
      }
      _mockRepository.ReplayAll();

      var result = _strategy.CanWrite (property, businessObject);

      Assert.That (result, Is.True);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CanWrite_WithOneStrategyReturingFalse_ReturnsFalse_AndAbortsChecks ()
    {
      var property = new StubPropertyBase (GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), _businessObjectProvider));
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();

      using (_mockRepository.Ordered())
      {
        _innerStrategy1.Expect (mock => mock.CanWrite (property, businessObject)).Return (true);
        _innerStrategy2.Expect (mock => mock.CanWrite (property, businessObject)).Return (false);
        _innerStrategy3.Expect (mock => mock.CanWrite (property, businessObject)).Repeat.Never();
      }
      _mockRepository.ReplayAll();

      var result = _strategy.CanWrite (property, businessObject);

      Assert.That (result, Is.False);

      _mockRepository.VerifyAll();
    }
  }
}
