// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
