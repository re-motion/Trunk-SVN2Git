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
  // TODO Review 3869: Class summary
  public abstract class SynonymBuilderBase : IEntityDefinitionVisitor
  {
    private readonly StringBuilder _createSynonymStringBuilder;
    private readonly StringBuilder _dropSynonymStringBuilder;
    // TODO Review 3869: Remove field (and ctor parameter)
    private readonly ISqlDialect _sqlDialect; 

    protected SynonymBuilderBase (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _createSynonymStringBuilder = new StringBuilder();
      _dropSynonymStringBuilder = new StringBuilder();
      _sqlDialect = sqlDialect;
    }

    public abstract void AddToCreateSynonymScript (EntityDefinitionBase entityDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropSynonymScript (EntityDefinitionBase entityDefinition, StringBuilder dropTableStringBuilder);

    public string GetCreateTableScript ()
    {
      return _createSynonymStringBuilder.ToString ();
    }

    public string GetDropTableScript ()
    {
      return _dropSynonymStringBuilder.ToString ();
    }

    public void AddSynonyms (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      entityDefinition.Accept (this);
    }

    // TODO Review 3869: Explicit

    public void VisitTableDefinition (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      AddToCreateSynonymScript (tableDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (tableDefinition, _dropSynonymStringBuilder);
    }

    public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      AddToCreateSynonymScript (unionViewDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (unionViewDefinition, _dropSynonymStringBuilder);
    }

    public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

      AddToCreateSynonymScript (filterViewDefinition, _createSynonymStringBuilder);
      AddToDropSynonymScript (filterViewDefinition, _dropSynonymStringBuilder);
    }

    public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

      //Nothing to do here
    }
  }
}