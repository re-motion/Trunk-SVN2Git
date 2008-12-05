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
using Remotion.Mixins;
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
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
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
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithMissmatchedConcreteType ()
    {
      new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"), _businessObjectProvider),
          TypeFactory.GetConcreteType (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithConcreteTypeNotImplementingIBusinessObject ()
    {
      new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"), _businessObjectProvider),
          typeof (SimpleBusinessObjectClass));
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), propertyName), _businessObjectProvider),
          TypeFactory.GetConcreteType (typeof (SimpleBusinessObjectClass)));
    }
  }
}
