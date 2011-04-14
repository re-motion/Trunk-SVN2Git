// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class PrimaryXmlIndexDefinition : SqlIndexDefinitionBase
  {
    private readonly EntityNameDefinition _indexName;
    private readonly EntityNameDefinition _objectName;
    private readonly IColumnDefinition _xmlColumn;

    /// <summary>
    /// <see cref="PrimaryXmlIndexDefinition"/> represents a priamry xml-column index in a relational database.
    /// </summary>
    public PrimaryXmlIndexDefinition (EntityNameDefinition indexName, EntityNameDefinition objectName, IColumnDefinition xmlColumn)
    {
      ArgumentUtility.CheckNotNull ("indexName", indexName);
      ArgumentUtility.CheckNotNull ("objectName", objectName);
      ArgumentUtility.CheckNotNull ("xmlColumn", xmlColumn);

      _indexName = indexName;
      _objectName = objectName;
      _xmlColumn = xmlColumn;
    }

    public override EntityNameDefinition IndexName
    {
      get { return _indexName; }
    } 

    public override EntityNameDefinition ObjectName
    {
      get { return _objectName; }
    }

    public IColumnDefinition XmlColumn
    {
      get { return _xmlColumn; }
    }

    public override void Accept (ISqlIndexDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitPrimaryXmlIndexDefinition (this);
    }
  }
}