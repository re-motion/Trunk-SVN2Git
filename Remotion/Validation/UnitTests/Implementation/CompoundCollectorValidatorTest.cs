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
using Remotion.Validation.Implementation;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundCollectorValidatorTest
  {
    private ICollectorValidator _collectorValidator1;
    private ICollectorValidator _collectorValidator2;
    private CompoundCollectorValidator _compoundCollectorValidator;
    private IComponentValidationCollector _collector;

    [SetUp]
    public void SetUp ()
    {
      _collectorValidator1 = MockRepository.GenerateStub<ICollectorValidator>();
      _collectorValidator2 = MockRepository.GenerateStub<ICollectorValidator>();

      _collector = MockRepository.GenerateStub<IComponentValidationCollector>();

      _compoundCollectorValidator = new CompoundCollectorValidator (new[] { _collectorValidator1, _collectorValidator2 });
    }

    [Test]
    public void IsValid_AllValid_ReturnsTrue ()
    {
      _collectorValidator1.Stub (stub => stub.IsValid (_collector)).Return (true);
      _collectorValidator2.Stub (stub => stub.IsValid (_collector)).Return (true);

      var result = _compoundCollectorValidator.IsValid (_collector);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsValid_NoneValid_ReturnsFalse ()
    {
      _collectorValidator1.Stub (stub => stub.IsValid (_collector)).Return (false);
      _collectorValidator2.Stub (stub => stub.IsValid (_collector)).Return (false);

      var result = _compoundCollectorValidator.IsValid (_collector);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsValid_OneValid_ReturnsTrue ()
    {
      _collectorValidator1.Stub (stub => stub.IsValid (_collector)).Return (false);
      _collectorValidator2.Stub (stub => stub.IsValid (_collector)).Return (true);

      var result = _compoundCollectorValidator.IsValid (_collector);

      Assert.That (result, Is.False);
    }
  }
}