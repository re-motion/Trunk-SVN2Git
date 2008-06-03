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
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class MetadataLocalizationToXmlConverter : IMetadataLocalizationConverter
  {
    private SecurityMetadataLocalizationSchema _schema;
    private LocalizationFileNameStrategy _fileNameStrategy;

    public MetadataLocalizationToXmlConverter ()
    {
      _schema = new SecurityMetadataLocalizationSchema ();
      _fileNameStrategy = new LocalizationFileNameStrategy ();
    }

    public void ConvertAndSave (LocalizedName[] localizedNames, CultureInfo culture, string filename)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

      XmlDocument document = Convert (localizedNames, culture.Name);
      document.Save (_fileNameStrategy.GetLocalizationFileName (filename, culture));
    }

    public XmlDocument Convert (LocalizedName[] localizedNames, string culture)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("localizedNames", localizedNames);
      ArgumentUtility.CheckNotNull ("culture", culture);

      XmlDocument document = new XmlDocument ();

      if (!document.Schemas.Contains (_schema.SchemaUri))
        document.Schemas.Add (_schema.LoadSchemaSet ());

      XmlDeclaration declaration = document.CreateXmlDeclaration ("1.0", string.Empty, string.Empty);
      document.AppendChild (declaration);
      
      XmlElement rootElement = document.CreateElement ("localizedNames", _schema.SchemaUri);
      XmlAttribute cultureAttribute = document.CreateAttribute ("culture");
      cultureAttribute.Value = culture;
      rootElement.Attributes.Append (cultureAttribute);

      foreach (LocalizedName localizedName in localizedNames)
      {
        XmlElement localizedNameElement = document.CreateElement ("localizedName", _schema.SchemaUri);
        
        XmlAttribute refAttribute = document.CreateAttribute ("ref");
        refAttribute.Value = localizedName.ReferencedObjectID;
        XmlAttribute commentAttribute = document.CreateAttribute ("comment");
        commentAttribute.Value = localizedName.Comment;
        XmlText text = document.CreateTextNode ("\r\n    " + localizedName.Text + "\r\n  ");

        localizedNameElement.Attributes.Append (refAttribute);
        localizedNameElement.Attributes.Append (commentAttribute);
        localizedNameElement.AppendChild (text);

        rootElement.AppendChild (localizedNameElement);
      }

      document.AppendChild (rootElement);

      return document;
    }
  }
}
