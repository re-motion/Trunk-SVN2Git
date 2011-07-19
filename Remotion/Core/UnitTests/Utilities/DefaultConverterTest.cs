// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class DefaultConverterTest
  {
    private DefaultConverter _converterWithNullableType;
    private ITypeDescriptorContext _typeDescriptorContext;
    private DefaultConverter _converterWithoutNullableType;

    [SetUp]
    public void SetUp ()
    {
      _converterWithNullableType = new DefaultConverter (typeof (string));
      _converterWithoutNullableType = new DefaultConverter (typeof (int));
      _typeDescriptorContext = MockRepository.GenerateStub<ITypeDescriptorContext>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_converterWithNullableType.Type, Is.SameAs(typeof (string)));
      Assert.That (_converterWithNullableType.IsNullableType, Is.True);
    }

    [Test]
    public void Initialization_ValuerType ()
    {
      Assert.That (_converterWithoutNullableType.Type, Is.SameAs(typeof (int)));
      Assert.That (_converterWithoutNullableType.IsNullableType, Is.False);
    }

    [Test]
    public void CanConvertFrom_True ()
    {
      var converter = new DefaultConverter (typeof (object));

      var result = converter.CanConvertFrom (_typeDescriptorContext, typeof (string));

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanConvertFrom_False ()
    {
      var result = _converterWithNullableType.CanConvertFrom (_typeDescriptorContext, typeof (int));

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanConvertTo_True ()
    {
      var result = _converterWithNullableType.CanConvertTo (_typeDescriptorContext, typeof (object));

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanConvertTo_False ()
    {
      var result = _converterWithNullableType.CanConvertTo (_typeDescriptorContext, typeof (int));

      Assert.That (result, Is.False);
    }

    [Ignore("TODO: RM-4167")]
    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Null cannot be converted to type 'Int32'")]
    public void ConvertFrom_ValueIsNullAndNoNullableType ()
    {
      _converterWithoutNullableType.ConvertFrom (_typeDescriptorContext, CultureInfo.CurrentCulture, null);
    }

    [Test]
    public void ConvertFrom_ValueIsNullAndNullableType ()
    {
      var result = _converterWithNullableType.ConvertFrom (_typeDescriptorContext, CultureInfo.CurrentCulture, null);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Value of type 'Object' cannot be connverted to type 'String'.")]
    public void ConvertFrom_ValueIsNotNullAndCannotConvertFromType ()
    {
      _converterWithNullableType.ConvertFrom (_typeDescriptorContext, CultureInfo.CurrentCulture, new object());
    }

    [Test]
    public void ConvertFrom_ValueIsNotNullAndCanConvertFromType ()
    {
      var converter = new DefaultConverter (typeof (object));

      var result = converter.ConvertFrom (_typeDescriptorContext, CultureInfo.CurrentCulture, "test");

      Assert.That (result, Is.EqualTo ("test"));
    }
    
    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Null cannot be converted by this TypeConverter.")]
    public void ConvertTo_ValueIsNullAndNoNullableType ()
    {
      _converterWithoutNullableType.ConvertTo (_typeDescriptorContext, CultureInfo.CurrentCulture, null, typeof(int));
    }

    [Test]
    public void ConvertTo_ValueIsNullAndNullableType ()
    {
      var result = _converterWithNullableType.ConvertTo (_typeDescriptorContext, CultureInfo.CurrentCulture, null, typeof(string));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Values of type 'Int32' cannot be converted by this TypeConverter.")]
    public void ConvertTo_ValueIsNotNullAndTypeNotAssignable ()
    {
      _converterWithNullableType.ConvertTo (_typeDescriptorContext, CultureInfo.CurrentCulture, 5, typeof (string));
    }

    [Test]
    public void ConvertTo_ValueIsNotNullAndCannotConvertToDestinationType ()
    {
      var result = _converterWithNullableType.ConvertTo (_typeDescriptorContext, CultureInfo.CurrentCulture, "test", typeof (object));

      Assert.That (result, Is.EqualTo ("test"));
    }
  }
}