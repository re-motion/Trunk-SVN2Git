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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

public enum EnumWithDescriptions
{
  [EnumDescription("Value One")]
  Value1 = 1,
  [EnumDescription("Value 2")]
  Value2 = 2,
  [EnumDescription("Value III")]
  Value3 = 3
}

[EnumDescriptionResource("Remotion.UnitTests.Resources.strings")]
public enum EnumFromResource
{
  Value1 = 1,
  Value2 = 2,
  Value3 = 3
}

[TestFixture]
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
        Assert.AreEqual ("Value One", EnumDescription.GetDescription (EnumWithDescriptions.Value1));
        Assert.AreEqual ("Value 2", EnumDescription.GetDescription (EnumWithDescriptions.Value2));
        Assert.AreEqual ("Value III", EnumDescription.GetDescription (EnumWithDescriptions.Value3));
      }
    }
  }

  [Test]
  public void TestGetAvailableValuesForEnumWithDescriptions()
  {
    using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
    {
      // try twice to test caching
      for (int i = 0; i < 2; ++i)
      {
        EnumValue[] enumValues = EnumDescription.GetAllValues (typeof (EnumWithDescriptions));
        Assert.AreEqual (3, enumValues.Length);
        Assert.AreEqual (EnumWithDescriptions.Value1, enumValues[0].Value);
        Assert.AreEqual ("Value One", enumValues[0].Description);
        Assert.AreEqual (EnumWithDescriptions.Value2, enumValues[1].Value);
        Assert.AreEqual ("Value 2", enumValues[1].Description);
        Assert.AreEqual (EnumWithDescriptions.Value3, enumValues[2].Value);
        Assert.AreEqual ("Value III", enumValues[2].Description);
      }
    }
  }

  [Test]
  public void TestGetDescriptionForEnumFromResource()
  {
    using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
    {
      Assert.AreEqual ("Wert Eins", EnumDescription.GetDescription (EnumFromResource.Value1));
      Assert.AreEqual ("Wert 2", EnumDescription.GetDescription (EnumFromResource.Value2));
      Assert.AreEqual ("Wert III", EnumDescription.GetDescription (EnumFromResource.Value3));

      CultureInfo culture = new CultureInfo ("en-US");
      Assert.AreEqual ("Val1", EnumDescription.GetDescription (EnumFromResource.Value1, culture));
      Assert.AreEqual ("Val2", EnumDescription.GetDescription (EnumFromResource.Value2, culture));
      Assert.AreEqual ("Val3", EnumDescription.GetDescription (EnumFromResource.Value3, culture));
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
        Assert.AreEqual (3, enumValues.Length);
        Assert.AreEqual (EnumFromResource.Value1, enumValues[0].Value);
        Assert.AreEqual ("Wert Eins", enumValues[0].Description);
        Assert.AreEqual (EnumFromResource.Value2, enumValues[1].Value);
        Assert.AreEqual ("Wert 2", enumValues[1].Description);
        Assert.AreEqual (EnumFromResource.Value3, enumValues[2].Value);
        Assert.AreEqual ("Wert III", enumValues[2].Description);
      }
    }
  }
}

}
