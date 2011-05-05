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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="ViewScriptBuilder"/> contains database-independent code to generate scripts to create and drop views in a relational database.
  /// </summary>
  public class ViewScriptBuilder : IScriptBuilder
  {
    private readonly IViewScriptElementFactory<TableDefinition> _tableViewElementFactory;
    private readonly IViewScriptElementFactory<UnionViewDefinition> _unionViewElementFactory;
    private readonly IViewScriptElementFactory<FilterViewDefinition> _filterViewElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private readonly ViewScriptBuilder _builder;

      public EntityDefinitionVisitor (ViewScriptBuilder builder)
      {
        _builder = builder;
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        _builder.AddTableDefinition (tableDefinition);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        _builder.AddUnionViewDefinition (unionViewDefinition);
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        _builder.AddFilterViewDefinition (filterViewDefinition);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do
      }
    }

    public ViewScriptBuilder (
        IViewScriptElementFactory<TableDefinition> tableViewElementFactory,
        IViewScriptElementFactory<UnionViewDefinition> unionViewElementFactory,
        IViewScriptElementFactory<FilterViewDefinition> filterViewElementFactory,
        ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull ("tableViewElementFactory", tableViewElementFactory);
      ArgumentUtility.CheckNotNull ("unionViewElementFactory", unionViewElementFactory);
      ArgumentUtility.CheckNotNull ("filterViewElementFactory", filterViewElementFactory);
      ArgumentUtility.CheckNotNull ("commentFactory", commentFactory);
      
      _tableViewElementFactory = tableViewElementFactory;
      _unionViewElementFactory = unionViewElementFactory;
      _filterViewElementFactory = filterViewElementFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement (commentFactory.GetCommentElement("Create a view for every class"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement (commentFactory.GetCommentElement("Drop all views"));
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
      AddElements (_tableViewElementFactory.GetCreateElement (tableDefinition), _tableViewElementFactory.GetDropElement (tableDefinition));
    }

    private void AddUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      AddElements (_unionViewElementFactory.GetCreateElement (unionViewDefinition), _unionViewElementFactory.GetDropElement (unionViewDefinition));
    }

    private void AddFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      AddElements (_filterViewElementFactory.GetCreateElement (filterViewDefinition), _filterViewElementFactory.GetDropElement (filterViewDefinition));
    }

    private void AddElements (IScriptElement createElement, IScriptElement dropElement)
    {
      _createScriptElements.AddElement (createElement);
      _dropScriptElements.AddElement (dropElement);
    }
  }
}