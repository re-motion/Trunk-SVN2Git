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
