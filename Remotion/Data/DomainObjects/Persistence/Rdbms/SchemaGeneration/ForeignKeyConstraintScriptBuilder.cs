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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="ForeignKeyConstraintScriptBuilder"/> contains database-independent code to generate scripts to create and drop foreign constraints in 
  /// a relational database.
  /// </summary>
  public class ForeignKeyConstraintScriptBuilder : IScriptBuilder
  {
    private readonly IForeignKeyConstraintScriptElementFactory _foreignKeyConstraintElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private readonly ForeignKeyConstraintScriptBuilder _builder;

      public EntityDefinitionVisitor (ForeignKeyConstraintScriptBuilder builder)
      {
        _builder = builder;
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        _builder.AddTableDefinition (tableDefinition);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        //Nothing to do
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        //Nothing to do
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do
      }
    }

    public ForeignKeyConstraintScriptBuilder (
        IForeignKeyConstraintScriptElementFactory foreignKeyConstraintElementFactory)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintElementFactory", foreignKeyConstraintElementFactory);

      _foreignKeyConstraintElementFactory = foreignKeyConstraintElementFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement (new ScriptStatement ("-- Create foreign key constraints for tables that were created above"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement (new ScriptStatement ("-- Drop foreign keys of all tables"));
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
      var foreignKeyConstraints = tableDefinition.Constraints.OfType<ForeignKeyConstraintDefinition>();
      foreach (var foreignKeyConstraint in foreignKeyConstraints)
        AddForeignKeyConstraintDefinition (foreignKeyConstraint, tableDefinition.TableName);
    }

    private void AddForeignKeyConstraintDefinition (ForeignKeyConstraintDefinition foreignKeyConstraint, EntityNameDefinition tableName)
    {
      _createScriptElements.AddElement (_foreignKeyConstraintElementFactory.GetCreateElement (foreignKeyConstraint, tableName));
      _dropScriptElements.AddElement (_foreignKeyConstraintElementFactory.GetDropElement (foreignKeyConstraint, tableName));
    }
  }
}