using System;
using Remotion.Xml;

namespace Remotion.Security.Metadata
{
  public class SecurityMetadataSchema : SchemaLoaderBase
  {
    protected override string SchemaFile
    {
      get { return "SecurityMetadata.xsd"; }
    }

    public override string SchemaUri
    {
      get { return "http://www.re-motion.org/Security/Metadata/1.0"; }
    }
  }
}
