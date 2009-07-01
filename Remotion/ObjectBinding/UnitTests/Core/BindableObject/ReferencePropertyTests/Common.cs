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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void CreateIfNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.CreateIfNull, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Create method is not supported by 'Remotion.ObjectBinding.BindableObject.Properties.ReferenceProperty'.")]
    public void Create ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      property.Create (null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = 
        "Argument concreteType is a Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithAllDataTypes, "
        +"which cannot be assigned to type Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass."
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
          false);
      new ReferenceProperty (parameters);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument parameters.ConcreteType is a Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, "
        + "which cannot be assigned to type Remotion.ObjectBinding.IBusinessObject."
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
          false);
      new ReferenceProperty (parameters);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), propertyName), _businessObjectProvider));
    }
  }
}