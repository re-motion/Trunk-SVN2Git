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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="IndexScriptBuilder"/> contains database-independent code to generate scripts to create and drop indexes in a relational database.
  /// </summary>
  public class IndexScriptBuilder : IScriptBuilder2
  {
    private readonly IIndexScriptElementFactory _indexScriptElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private readonly IndexScriptBuilder _builder;

      public EntityDefinitionVisitor (IndexScriptBuilder builder)
      {
        _builder = builder;
      }

      void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
      {
        _builder.AddTableDefinition (tableDefinition);
      }

      void IEntityDefinitionVisitor.VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        _builder.AddUnionViewDefinition (unionViewDefinition);
      }

      void IEntityDefinitionVisitor.VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        _builder.AddFilterViewDefinition (filterViewDefinition);
      }

      void IEntityDefinitionVisitor.VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do
      }
    }

    public IndexScriptBuilder (IIndexScriptElementFactory indexScriptElementFactory)
    {
      ArgumentUtility.CheckNotNull ("indexScriptElementFactory", indexScriptElementFactory);

      _indexScriptElementFactory = indexScriptElementFactory;
      _createScriptElements = new ScriptElementCollection();
      _dropScriptElements = new ScriptElementCollection();
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      var visitor = new EntityDefinitionVisitor (this);
      entityDefinition.Accept (visitor);
    }

    public ScriptElementCollection GetCreateScript ()
    {
      return _createScriptElements;
    }

    public ScriptElementCollection GetDropScript ()
    {
      return _dropScriptElements;
    }

    private void AddTableDefinition (TableDefinition tableDefinition)
    {
      AddIndexes (tableDefinition.Indexes);
    }

    private void AddUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      AddIndexes (unionViewDefinition.Indexes);
    }

    private void AddFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      AddIndexes (filterViewDefinition.Indexes);
    }

    private void AddIndexes (IEnumerable<IIndexDefinition> indexes)
    {
      foreach (var index in indexes)
      {
        _createScriptElements.AddElement (_indexScriptElementFactory.GetCreateElement (index));
        _dropScriptElements.AddElement (_indexScriptElementFactory.GetDropElement (index));
      }
    }
  }
}