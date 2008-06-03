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
using System.Collections.Generic;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class MetadataConverterBuilder
  {
    private bool _convertMetadataToXml = false;
    private List<CultureInfo> _cultures;

    public MetadataConverterBuilder ()
    {
      _cultures = new List<CultureInfo> ();
    }

    public bool ConvertMetadataToXml
    {
      get { return _convertMetadataToXml; }
      set { _convertMetadataToXml = value; }
    }

    public IMetadataConverter Create ()
    {
      if (_cultures.Count == 0 && !_convertMetadataToXml)
        throw new InvalidOperationException ("You must specify at least a localization or a metadata converter.");

      if (_cultures.Count == 0)
        return new MetadataToXmlConverter ();

      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (new MetadataLocalizationToXmlConverter (), _cultures.ToArray ());

      if (_convertMetadataToXml)
        converter.MetadataConverter = new MetadataToXmlConverter ();

      return converter;
    }

    public void AddLocalization (string cultureName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("cultureName", cultureName);

      AddLocalization (new CultureInfo (cultureName.Trim ()));
    }

    public void AddLocalization (CultureInfo cultureInfo)
    {
      ArgumentUtility.CheckNotNull ("cultureInfo", cultureInfo);

      _cultures.Add (cultureInfo);
    }
  }
}
