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
