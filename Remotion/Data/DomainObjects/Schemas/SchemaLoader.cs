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
using System.Xml.Schema;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;
using Remotion.Xml;

namespace Remotion.Data.DomainObjects.Schemas
{
  public class SchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public static readonly SchemaLoader Queries = new SchemaLoader ("Queries.xsd", PrefixNamespace.QueryConfigurationNamespace.Uri);

    // member fields

    private readonly string _schemaFile;
    private readonly string _schemaUri;

    // construction and disposing

    protected SchemaLoader (string schemaFile, string schemaUri)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("schemaFile", schemaFile);
      ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);

      _schemaFile = schemaFile;
      _schemaUri = schemaUri;
    }

    // methods and properties

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public override XmlSchemaSet LoadSchemaSet ()
    {
      XmlSchemaSet schemaSet = base.LoadSchemaSet ();
      schemaSet.Add (TypesSchemaLoader.Instance.LoadSchemaSet());

      return schemaSet;
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
