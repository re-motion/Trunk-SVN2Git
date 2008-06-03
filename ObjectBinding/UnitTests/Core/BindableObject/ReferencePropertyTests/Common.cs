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
