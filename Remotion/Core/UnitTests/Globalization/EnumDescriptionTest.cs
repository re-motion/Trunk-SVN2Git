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
using System.Globalization;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.UnitTests.Globalization
{
  public enum EnumWithDescriptions
  {
    [EnumDescription ("Value One")]
    Value1 = 1,

    [EnumDescription ("Value 2")]
    Value2 = 2,

    [EnumDescription ("Value III")]
    Value3 = 3,

    Value4 = 4,
  }

  [EnumDescriptionResource ("Remotion.UnitTests.Resources.strings")]
  public enum EnumFromResource
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3,
    Value4 = 4,
  }

  [TestFixture]
  [Obsolete("Version 1.13.222.0")]
  public class EnumDescriptionTest
  {
    [Test]
    public void TestGetDescriptionForEnumWithDescriptions ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          Assert.That (EnumDescription.GetDescription (EnumWithDescriptions.Value1), Is.EqualTo ("Value One"));
          Assert.That (EnumDescription.GetDescription (EnumWithDescriptions.Value2), Is.EqualTo ("Value 2"));
          Assert.That (EnumDescription.GetDescription (EnumWithDescriptions.Value3), Is.EqualTo ("Value III"));
          Assert.That (EnumDescription.GetDescription (EnumWithDescriptions.Value4), Is.EqualTo ("Value4"));
          Assert.That (EnumDescription.GetDescription ((EnumWithDescriptions) 100), Is.EqualTo ("100"));
        }
      }
    }

    [Test]
    public void TestGetAvailableValuesForEnumWithDescriptions ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          EnumValue[] enumValuesInvariant = EnumDescription.GetAllValues (typeof (EnumWithDescriptions));
          Assert.That (enumValuesInvariant.Length, Is.EqualTo (4));
          Assert.That (enumValuesInvariant[0].Value, Is.EqualTo (EnumWithDescriptions.Value1));
          Assert.That (enumValuesInvariant[0].Description, Is.EqualTo ("Value One"));
          Assert.That (enumValuesInvariant[1].Value, Is.EqualTo (EnumWithDescriptions.Value2));
          Assert.That (enumValuesInvariant[1].Description, Is.EqualTo ("Value 2"));
          Assert.That (enumValuesInvariant[2].Value, Is.EqualTo (EnumWithDescriptions.Value3));
          Assert.That (enumValuesInvariant[2].Description, Is.EqualTo ("Value III"));
          Assert.That (enumValuesInvariant[3].Value, Is.EqualTo (EnumWithDescriptions.Value4));
          Assert.That (enumValuesInvariant[3].Description, Is.EqualTo ("Value4"));

          CultureInfo culture = new CultureInfo ("en-US");
          EnumValue[] enumValuesSpecific = EnumDescription.GetAllValues (typeof (EnumWithDescriptions), culture);
          Assert.That (enumValuesSpecific.Length, Is.EqualTo (4));
          Assert.That (enumValuesSpecific[0].Value, Is.EqualTo (EnumWithDescriptions.Value1));
          Assert.That (enumValuesSpecific[0].Description, Is.EqualTo ("Value One"));
          Assert.That (enumValuesSpecific[1].Value, Is.EqualTo (EnumWithDescriptions.Value2));
          Assert.That (enumValuesSpecific[1].Description, Is.EqualTo ("Value 2"));
          Assert.That (enumValuesSpecific[2].Value, Is.EqualTo (EnumWithDescriptions.Value3));
          Assert.That (enumValuesSpecific[2].Description, Is.EqualTo ("Value III"));
          Assert.That (enumValuesSpecific[3].Value, Is.EqualTo (EnumWithDescriptions.Value4));
          Assert.That (enumValuesSpecific[3].Description, Is.EqualTo ("Value4"));
        }
      }
    }

    [Test]
    public void TestGetDescriptionForEnumFromResource ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value1), Is.EqualTo ("Wert Eins"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value2), Is.EqualTo ("Wert 2"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value3), Is.EqualTo ("Wert III"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value4), Is.EqualTo ("Value4"));
        Assert.That (EnumDescription.GetDescription ((EnumFromResource) 100), Is.EqualTo ("100"));

        CultureInfo culture = new CultureInfo ("en-US");
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value1, culture), Is.EqualTo ("Val1"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value2, culture), Is.EqualTo ("Val2"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value3, culture), Is.EqualTo ("Val3"));
        Assert.That (EnumDescription.GetDescription (EnumFromResource.Value4, culture), Is.EqualTo ("Value4"));
        Assert.That (EnumDescription.GetDescription ((EnumFromResource) 100, culture), Is.EqualTo ("100"));
      }
    }

    [Test]
    public void TestGetAvailableValuesForEnumFromResource ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          EnumValue[] enumValues = EnumDescription.GetAllValues (typeof (EnumFromResource));
          Assert.That (enumValues.Length, Is.EqualTo (4));
          Assert.That (enumValues[0].Value, Is.EqualTo (EnumFromResource.Value1));
          Assert.That (enumValues[0].Description, Is.EqualTo ("Wert Eins"));
          Assert.That (enumValues[1].Value, Is.EqualTo (EnumFromResource.Value2));
          Assert.That (enumValues[1].Description, Is.EqualTo ("Wert 2"));
          Assert.That (enumValues[2].Value, Is.EqualTo (EnumFromResource.Value3));
          Assert.That (enumValues[2].Description, Is.EqualTo ("Wert III"));
          Assert.That (enumValues[3].Value, Is.EqualTo (EnumFromResource.Value4));
          Assert.That (enumValues[3].Description, Is.EqualTo ("Value4"));
        }
      }
    }
  }
}