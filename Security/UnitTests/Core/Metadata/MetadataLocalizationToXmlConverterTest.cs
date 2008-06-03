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
using System.Globalization;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.XmlAsserter;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class MetadataLocalizationToXmlConverterTest
  {
    [Test]
    public void Convert_Empty ()
    {
      LocalizedName[] localizedNames = new LocalizedName[0];
      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter ();

      XmlDocument document = converter.Convert (localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"" />
          ";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void Convert_OneLocalizedName ()
    {
      LocalizedName[] localizedNames = new LocalizedName[1];
      localizedNames[0] = new LocalizedName ("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter ();

      XmlDocument document = converter.Convert (localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void Convert_TwoLocalizedNames ()
    {
      LocalizedName[] localizedNames = new LocalizedName[2];
      localizedNames[0] = new LocalizedName ("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");
      localizedNames[1] = new LocalizedName ("93969f13-65d7-49f4-a456-a1686a4de3de", "Confidentiality", "Vertraulichkeit");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter ();

      XmlDocument document = converter.Convert (localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
    Vertraulichkeit
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void Convert_OneLocalizedNameForInvariantCulture ()
    {
      LocalizedName[] localizedNames = new LocalizedName[1];
      localizedNames[0] = new LocalizedName ("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter ();

      XmlDocument document = converter.Convert (localizedNames, CultureInfo.InvariantCulture.Name);

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture="""">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual (expectedXml, document);
    }

    [Test]
    public void ConvertAndSave_NewFile ()
    {
      string testOutputPath = @"Core\Metadata\Testoutput";

      if (!Directory.Exists (testOutputPath))
        Directory.CreateDirectory (testOutputPath);

      string filename = Path.Combine (testOutputPath, "metadata.xml");
      string expectedFilename = Path.Combine (testOutputPath, "metadata.Localization.de.xml");

      if (File.Exists (expectedFilename))
        File.Delete (expectedFilename);

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter ();
      converter.ConvertAndSave (new LocalizedName[0], new CultureInfo("de"), filename);

      Assert.IsTrue (File.Exists (expectedFilename));
    }
  }
}
