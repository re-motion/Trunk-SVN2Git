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
