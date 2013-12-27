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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Parameter 'concreteType' is a 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithAllDataTypes', "
        +"which cannot be assigned to type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass'."
        + "\r\nParameter name: concreteType")]
    public void Initialize_WithMissmatchedConcreteType ()
    {
      PropertyBase.Parameters parameters = new PropertyBase.Parameters (
          _businessObjectProvider,
          GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass),
          typeof (ClassWithAllDataTypes),
          null,
          false,
          false,
          new BindableObjectDefaultValueStrategy ());
      new ReferenceProperty (parameters);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'parameters.ConcreteType' is a 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass', "
        + "which cannot be assigned to type 'Remotion.ObjectBinding.IBusinessObject'."
        + "\r\nParameter name: parameters.ConcreteType")]
    public void Initialize_WithConcreteTypeNotImplementingIBusinessObject ()
    {
      PropertyBase.Parameters parameters = new PropertyBase.Parameters (
          _businessObjectProvider,
          GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass),
          typeof (SimpleBusinessObjectClass),
          null,
          false,
          false,
          new BindableObjectDefaultValueStrategy ());
      new ReferenceProperty (parameters);
    }
  }
}
