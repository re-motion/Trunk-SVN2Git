using System;
using Remotion.Xml;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{

public class UrlMappingSchema: SchemaLoaderBase
{
  public UrlMappingSchema()
  {
  }

  protected override string SchemaFile
  {
    get { return "UrlMapping.xsd"; }
  }

  public override string SchemaUri
  {
    get { return UrlMappingConfiguration.SchemaUri; }
  }
}

}