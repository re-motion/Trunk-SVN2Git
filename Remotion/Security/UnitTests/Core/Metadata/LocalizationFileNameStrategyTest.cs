// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.IO;
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class LocalizationFileNameStrategyTest
  {
    [Test]
    public void GetLocalizationFileNames_NoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Core\Metadata\LocalizationFiles\notexisting.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (0, localizationFileNames.Length);
    }

    [Test]
    public void GetLocalizationFileNames_OneLocalizationFile ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Core\Metadata\LocalizationFiles\OneLocalizationFile.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (1, localizationFileNames.Length);
      Assert.Contains (@"Core\Metadata\LocalizationFiles\OneLocalizationFile.Localization.de.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Core\Metadata\LocalizationFiles\TwoLocalizationFiles.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@"Core\Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.de.xml", localizationFileNames);
      Assert.Contains (@"Core\Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.en.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFilesIncludingInvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Core\Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@"Core\Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml", localizationFileNames);
      Assert.Contains (@"Core\Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_WithoutBaseDirectory ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string wd = Directory.GetCurrentDirectory ();
      Directory.SetCurrentDirectory (@"Core\Metadata\LocalizationFiles");
      string metadataFileName = "TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Directory.SetCurrentDirectory (wd);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml", localizationFileNames);
      Assert.Contains (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileName_GermanLanguage ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, new CultureInfo ("de"));

      Assert.AreEqual ("metadata.Localization.de.xml", localizationFilename);
    }

    [Test]
    public void GetLocalizationFileName_InvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, CultureInfo.InvariantCulture);

      Assert.AreEqual ("metadata.Localization.xml", localizationFilename);
    }
  }
}
