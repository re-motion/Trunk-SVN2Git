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
