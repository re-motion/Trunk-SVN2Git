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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Contains database-independent code for generating synonyms for the persistence model.
  /// </summary>
  public abstract class SynonymScriptBuilderBase : IScriptBuilder, IEntityDefinitionVisitor
  {
    private readonly StringBuilder _createSynonymStringBuilder;
    private readonly StringBuilder _dropSynonymStringBuilder;
    
    protected SynonymScriptBuilderBase ()
    {
      _createSynonymStringBuilder = new StringBuilder();
      _dropSynonymStringBuilder = new StringBuilder();
    }

    public abstract void AddToCreateSynonymScript (TableDefinition tableDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToCreateSynonymScript (FilterViewDefinition filterViewDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToCreateSynonymScript (UnionViewDefinition unionViewDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropSynonymScript (EntityDefinitionBase entityDefinition, StringBuilder dropTableStringBuilder);

    public string GetCreateScript ()
    {
      return _createSynonymStringBuilder.ToString ();
    }

    public string GetDropScript ()
    {
      return _dropSynonymStringBuilder.ToString ();
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      entityDefinition.Accept (this);
    }

    void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      AddToCreateSynonymScript (tableDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (tableDefinition, _dropSynonymStringBuilder);
    }

    void IEntityDefinitionVisitor.VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      AddToCreateSynonymScript (unionViewDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (unionViewDefinition, _dropSynonymStringBuilder);
    }

    void IEntityDefinitionVisitor.VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

      AddToCreateSynonymScript (filterViewDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (filterViewDefinition, _dropSynonymStringBuilder);
    }

    void IEntityDefinitionVisitor.VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

      //Nothing to do here
    }
  }
}