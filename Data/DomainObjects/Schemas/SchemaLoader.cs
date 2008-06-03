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
using System.Xml.Schema;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;
using Remotion.Xml;

namespace Remotion.Data.DomainObjects.Schemas
{
  public class SchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public readonly static SchemaLoader Queries = new SchemaLoader ("Queries.xsd", PrefixNamespace.QueryConfigurationNamespace.Uri);

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
