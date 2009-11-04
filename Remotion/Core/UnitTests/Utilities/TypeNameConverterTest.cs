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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class TypeNameConverterTest
  {
    // types

    // static members

    // member fields

    TypeNameConverter _converter;

    // construction and disposing

    public TypeNameConverterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _converter = new TypeNameConverter ();
    }

    [Test]
    public void CanConvertToString ()
    {
      Assert.IsTrue (_converter.CanConvertTo (typeof (string)));
    }

    [Test]
    public void CanConvertFromString ()
    {
      Assert.IsTrue (_converter.CanConvertFrom (typeof (string)));
    }

    [Test]
    public void ConvertToString ()
    {
      Type destinationType = typeof (string);

      Assert.AreEqual ("", _converter.ConvertTo (null, null, null, destinationType));
      Assert.AreEqual (
          "Remotion.UnitTests.Utilities.TypeNameConverterTest, Remotion.UnitTests", 
          (string) _converter.ConvertTo (null, null, typeof (TypeNameConverterTest), destinationType));
    }

    [Test]
    public void ConvertFromString ()
    {
      Assert.AreEqual (null, _converter.ConvertFrom (null, null, ""));
      Assert.AreEqual (
          typeof (TypeNameConverterTest),
          _converter.ConvertFrom (null, null, "Remotion.UnitTests.Utilities.TypeNameConverterTest, Remotion.UnitTests"));
      Assert.AreEqual (
          typeof (TypeNameConverterTest),
          _converter.ConvertFrom (null, null, "Remotion.UnitTests::Utilities.TypeNameConverterTest"));
    }
  }
}
