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
using System.Globalization;
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class MetadataConverterBuilderTest
  {
    private MetadataConverterBuilder _converterBuilder;

    [SetUp]
    public void SetUp ()
    {
      _converterBuilder = new MetadataConverterBuilder ();
    }

    [Test]
    public void Create_SimpleMetadataToXmlConverter ()
    {
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (MetadataToXmlConverter), converter);
    }

    [Test]
    public void Create_LocalizingConverterForOneLanguage ()
    {
      _converterBuilder.AddLocalization ("de");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForLanguageWithWhitespaces ()
    {
      _converterBuilder.AddLocalization (" de ");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForOneCultureInfo ()
    {
      _converterBuilder.AddLocalization (new CultureInfo("de"));

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForTwoLanguages ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization ("fr");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual ("fr", localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterWithMetadataToXmlConverter ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization ("fr");
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsInstanceOfType (typeof (MetadataToXmlConverter), localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual ("fr", localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterWithInvariantCulture ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization (CultureInfo.InvariantCulture);

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual (CultureInfo.InvariantCulture.TwoLetterISOLanguageName, localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "You must specify at least a localization or a metadata converter.")]
    public void Create_ExceptionWithoutLocalizationAndMetadataConverter ()
    {
      _converterBuilder.Create ();
    }
  }
}
