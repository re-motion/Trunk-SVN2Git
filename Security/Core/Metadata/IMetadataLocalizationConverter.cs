using System;
using System.Globalization;

namespace Remotion.Security.Metadata
{
  public interface IMetadataLocalizationConverter
  {
    void ConvertAndSave (LocalizedName[] localizedNames, CultureInfo culture, string filename);
  }
}
