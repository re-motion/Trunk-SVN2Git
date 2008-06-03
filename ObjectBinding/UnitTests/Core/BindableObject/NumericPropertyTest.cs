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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class NumericPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void Initialize_ByteProperty ()
    {
      IBusinessObjectNumericProperty property = new ByteProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Byte)));
      Assert.That (property.AllowNegative, Is.False);
    }

    [Test]
    public void Initialize_Int16Property ()
    {
      IBusinessObjectNumericProperty property = new Int16Property (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int16"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Int16)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int32Property ()
    {
      IBusinessObjectNumericProperty property = new Int32Property (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int32"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Int32)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int64Property ()
    {
      IBusinessObjectNumericProperty property = new Int64Property (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int64"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Int64)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_SingleProperty ()
    {
      IBusinessObjectNumericProperty property = new SingleProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Single"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Single)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DoubleProperty ()
    {
      IBusinessObjectNumericProperty property = new DoubleProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Double"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Double)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DecimalProperty ()
    {
      IBusinessObjectNumericProperty property = new DecimalProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Decimal"), _businessObjectProvider));

      Assert.That (property.Type, Is.SameAs (typeof (Decimal)));
      Assert.That (property.AllowNegative, Is.True);
    }
  }
}
