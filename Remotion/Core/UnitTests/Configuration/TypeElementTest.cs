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
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using TypeNameConverter=Remotion.Utilities.TypeNameConverter;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElementTest
  {
    [Test]
    public void Initialize()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection) PrivateInvoke.GetNonPublicProperty (typeElement, "Properties");
      Assert.IsNotNull (properties);
      ConfigurationProperty property = properties["type"];
      Assert.IsNotNull (property);
      Assert.IsNull (property.DefaultValue);
      Assert.IsInstanceOfType (typeof (TypeNameConverter), property.Converter);
      Assert.IsInstanceOfType (typeof (SubclassTypeValidator), property.Validator);
      Assert.IsTrue (property.IsRequired);
    }

    [Test]
    public void GetAndSetType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      typeElement.Type = typeof (DerivedSampleType);
      Assert.AreEqual (typeof (DerivedSampleType), typeElement.Type);
    }

    [Test]
    public void GetType_WithTypeNull()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      Assert.IsNull (typeElement.Type);
    }

    [Test]
    public void CreateInstance_WithType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();
      typeElement.Type = typeof (DerivedSampleType);

      Assert.IsInstanceOfType (typeof (DerivedSampleType), typeElement.CreateInstance());
    }

    [Test]
    public void CreateInstance_WithoutType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      Assert.IsNull (typeElement.CreateInstance());
    }

    [Test]
    public void Deserialize_WithValidType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      string xmlFragment = @"<theElement type=""Remotion.UnitTests::Configuration.SampleType"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Assert.AreEqual (typeof (SampleType), typeElement.Type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Deserialize_WithInvalidType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      string xmlFragment = @"<theElement type=""System.Object, mscorlib"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Dev.Null = typeElement.Type;
    }
  }
}
