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

namespace Remotion.Security.Metadata
{
  public class LocalizationFileNameStrategy
  {
    public string GetLocalizationFileName (string metadataFilename, CultureInfo culture)
    {
      string baseFilename = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string baseFilePath = Path.Combine (basePath, baseFilename);

      if (string.IsNullOrEmpty (culture.Name))
        return baseFilePath + ".Localization.xml";

      return baseFilePath + ".Localization." + culture.Name + ".xml";
    }

    public string[] GetLocalizationFileNames (string metadataFilename)
    {
      string baseFileName = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string searchPattern = baseFileName + ".Localization.*xml";

      if (basePath == string.Empty)
        basePath = ".";

      return Directory.GetFiles (basePath, searchPattern, SearchOption.TopDirectoryOnly);
    }
  }
}
