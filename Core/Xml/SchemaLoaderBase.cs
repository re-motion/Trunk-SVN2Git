using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Remotion.Xml
{
  public abstract class SchemaLoaderBase
  {
    protected abstract string SchemaFile { get; }
    public abstract string SchemaUri { get; }

    /// <summary> Gets an <see cref="XmlSchemaSet"/> for the schema specified with property <see cref="SchemaFile"/> that is embedded in the assembly. </summary>
    /// <remarks> Overwrite this method if the Schema includes other schemas that need to be loaded first. </remarks>
    /// <exception cref="ApplicationException"> Thrown if the schema file could not be loaded. </exception>
    public virtual XmlSchemaSet LoadSchemaSet ()
    {
      XmlSchemaSet xmlSchemaSet = new XmlSchemaSet ();
      xmlSchemaSet.Add (LoadSchema (SchemaFile));
      return xmlSchemaSet;
    }

    /// <summary> Gets an <see cref="XmlSchema"/> for a schema embedded in the assembly. </summary>
    /// <exception cref="ApplicationException"> Thrown if the schema file could not be loaded. </exception>
    protected XmlSchema LoadSchema (string schemaFileName)
    {
      Type type = GetType ();
      Assembly assembly = type.Assembly;

      using (Stream schemaStream = assembly.GetManifestResourceStream (type, schemaFileName))
      {
        if (schemaStream == null)
          throw new ApplicationException (string.Format ("Error loading schema resource '{0}' from assembly '{1}'.", schemaFileName, assembly.FullName));

        using (XmlReader xmlReader = XmlReader.Create (schemaStream))
        {
          return XmlSchema.Read (xmlReader, null);
        }
      }
    }
  }
}