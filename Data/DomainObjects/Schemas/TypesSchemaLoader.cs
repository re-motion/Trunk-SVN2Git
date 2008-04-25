using System;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;
using Remotion.Xml;

namespace Remotion.Data.DomainObjects.Schemas
{
  public sealed class TypesSchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public static readonly TypesSchemaLoader Instance = new TypesSchemaLoader();

    // member fields

    private readonly string _schemaFile = "Types.xsd";
    private readonly string _schemaUri = "http://www.re-motion.org/Data/DomainObjects/Types";

    // construction and disposing

    private TypesSchemaLoader ()
    {
    }

    // methods and properties

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
