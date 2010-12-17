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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public abstract class ViewBuilderBase
  {
    private readonly StringBuilder _createViewStringBuilder;
    private readonly StringBuilder _dropViewStringBuilder;

    protected ViewBuilderBase ()
    {
      _createViewStringBuilder = new StringBuilder ();
      _dropViewStringBuilder = new StringBuilder ();
    }

    public abstract void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddTableViewToCreateViewScript (TableDefinition tableDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddUnionViewToCreateViewScript (UnionViewDefinition unionViewDefinition, StringBuilder createViewStringBuilder);
    public abstract void AddToDropViewScript (ClassDefinition classDefinition, StringBuilder dropViewStringBuilder);
    
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
      foreach (ClassDefinition classDefinition in classDefinitions)
        AddView (classDefinition);
    }

    public void AddView (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.StorageEntityDefinition is FilterViewDefinition)
      {
        AddFilterViewToCreateViewScript ((FilterViewDefinition) classDefinition.StorageEntityDefinition);
        AddToDropViewScript (classDefinition);
      }
      else if (classDefinition.StorageEntityDefinition is TableDefinition)
      {
        AddTableViewToCreateViewScript ((TableDefinition) classDefinition.StorageEntityDefinition);
        AddToDropViewScript (classDefinition);
      }
      else
      {
        // TODO 3606: Remove this check
        if (((UnionViewDefinition) classDefinition.StorageEntityDefinition).GetAllTables().Any())
        {
          AddUnionViewToCreateViewScript ((UnionViewDefinition) classDefinition.StorageEntityDefinition);
          AddToDropViewScript (classDefinition);
        }
      }
    }

    private void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition)
    {
      AppendCreateViewSeparator ();
      AddFilterViewToCreateViewScript (filterViewDefinition, _createViewStringBuilder);
    }

    private void AddTableViewToCreateViewScript (TableDefinition tableDefinition)
    {
      AppendCreateViewSeparator ();
      AddTableViewToCreateViewScript (tableDefinition, _createViewStringBuilder);
    }

    private void AddUnionViewToCreateViewScript (UnionViewDefinition unionViewDefinition)
    {
      AppendCreateViewSeparator ();
      AddUnionViewToCreateViewScript (unionViewDefinition, _createViewStringBuilder);
    }

    private void AppendCreateViewSeparator ()
    {
      if (_createViewStringBuilder.Length != 0)
        _createViewStringBuilder.Append (CreateViewSeparator);
    }

    private void AddToDropViewScript (ClassDefinition classDefinition)
    {
      if (_dropViewStringBuilder.Length != 0)
        _dropViewStringBuilder.Append ("\r\n");

      AddToDropViewScript (classDefinition, _dropViewStringBuilder);
    }
  }
}
