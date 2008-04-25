using System;

namespace Remotion.Security.Metadata
{
  public interface IMetadataConverter
  {
    void ConvertAndSave (MetadataCache cache, string filename);
  }
}
