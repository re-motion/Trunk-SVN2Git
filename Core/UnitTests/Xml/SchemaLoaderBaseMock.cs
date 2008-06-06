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
using Remotion.Utilities;
using Remotion.Xml;

namespace Remotion.UnitTests.Xml
{
  public class SchemaLoaderBaseMock : SchemaLoaderBase
  {
    // types

    // static members and constants

    // member fields

    private string _schemaUri;

    // construction and disposing

    public SchemaLoaderBaseMock (string schemaUri)
    {
      ArgumentUtility.CheckNotNull ("schemaUri", schemaUri);

      _schemaUri = schemaUri;
    }

    // methods and properties

    protected override string SchemaFile
    {
      get { return "SchemaLoaderBaseMock.xsd"; }
    }

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    public new XmlSchema LoadSchema (string schemaFileName)
    {
      return base.LoadSchema (schemaFileName);
    }
  }
}
