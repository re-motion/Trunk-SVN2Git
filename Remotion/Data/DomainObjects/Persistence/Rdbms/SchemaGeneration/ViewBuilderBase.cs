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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Contains database-independent code for generating views for the persistence model.
  /// </summary>
  public abstract class ViewBuilderBase : IEntityDefinitionVisitor
  { 
    private readonly StringBuilder _createViewStringBuilder;
    private readonly StringBuilder _dropViewStringBuilder;
    private readonly ISqlDialect _sqlDialect;

    protected ViewBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
      _createViewStringBuilder = new StringBuilder ();
      _dropViewStringBuilder = new StringBuilder ();
    }

    public abstract void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddTableViewToCreateViewScript (TableDefinition tableDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddUnionViewToCreateViewScript (UnionViewDefinition unionViewDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddToDropViewScript (IEntityDefinition entityDefinition, StringBuilder dropViewStringBuilder);
    
    public abstract string CreateViewSeparator { get; }

    public string GetCreateViewScript ()
    {
      return _createViewStringBuilder.ToString ();
    }

    public string GetDropViewScript ()
    {
      return _dropViewStringBuilder.ToString ();
    }

    public void AddViews (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      foreach (ClassDefinition classDefinition in classDefinitions)
        AddView (classDefinition);
    }

    public void AddView (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var entityDefinition = classDefinition.StorageEntityDefinition as IEntityDefinition;
      if (entityDefinition != null)
        entityDefinition.Accept (this);
    }

    protected string GetColumnList (IEnumerable<IColumnDefinition> columnDefinitions, bool allowNulls)
    {
      var visitor = new NameListColumnDefinitionVisitor (allowNulls, true, _sqlDialect);

      foreach (var columnDefinition in columnDefinitions)
        columnDefinition.Accept (visitor);

      return visitor.GetNameList ();
    }

    protected string GetClassIDList (IEnumerable<string> classIDs)
    {
      return SeparatedStringBuilder.Build (", ", classIDs, id => "'" + id + "'");
    }

    private void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition)
    {
      Assertion.IsNotNull (filterViewDefinition.ViewName, "Change implementation when null views are allowed");

      AppendCreateViewSeparator ();
      AddFilterViewToCreateViewScript (filterViewDefinition, _createViewStringBuilder);
    }

    private void AddTableViewToCreateViewScript (TableDefinition tableDefinition)
    {
      Assertion.IsNotNull (tableDefinition.ViewName, "Change implementation when null views are allowed");

      AppendCreateViewSeparator ();
      AddTableViewToCreateViewScript (tableDefinition, _createViewStringBuilder);
    }

    private void AddUnionViewToCreateViewScript (UnionViewDefinition unionViewDefinition)
    {
      Assertion.IsNotNull (unionViewDefinition.ViewName, "Change implementation when null views are allowed");

      AppendCreateViewSeparator ();
      AddUnionViewToCreateViewScript (unionViewDefinition, _createViewStringBuilder);
    }

    private void AppendCreateViewSeparator ()
    {
      if (_createViewStringBuilder.Length != 0)
        _createViewStringBuilder.Append (CreateViewSeparator);
    }

    private void AddToDropViewScript (IEntityDefinition entityDefinition)
    {
      Assertion.IsNotNull (entityDefinition.ViewName, "Change implementation when null views are allowed");

      if (_dropViewStringBuilder.Length != 0)
        _dropViewStringBuilder.Append ("\r\n");

      AddToDropViewScript (entityDefinition, _dropViewStringBuilder);
    }

    void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      AddTableViewToCreateViewScript (tableDefinition);
      AddToDropViewScript (tableDefinition);
    }

    void IEntityDefinitionVisitor.VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      AddUnionViewToCreateViewScript (unionViewDefinition);
      AddToDropViewScript (unionViewDefinition);
    }

    void IEntityDefinitionVisitor.VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

      AddFilterViewToCreateViewScript (filterViewDefinition);
      AddToDropViewScript (filterViewDefinition);
    }

    void IEntityDefinitionVisitor.VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

      //Nothing to do here
    }
  }
}
