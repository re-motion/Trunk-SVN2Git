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
  /// <summary>
  /// Visits the given <see cref="IRdbmsStorageEntityDefinition"/> and executes a handler based on the entity's type.
  /// </summary>
  public class RdbmsStorageInlineEntityDefinitionVisitor
  {
    public static T Visit<T> (
        IRdbmsStorageEntityDefinition entityDefinition,
        Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> tableDefinitionHandler,
        Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> filterViewDefinitionHandler,
        Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> unionViewDefinitionHandler,
        Func<NullEntityDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> nullEntityDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
      ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

      var visitor = new RdbmsStorageEntityDefinitionVisitor<T> (
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          nullEntityDefinitionHandler);
      entityDefinition.Accept (visitor);
      return visitor.ReturnValue;
    }

    public static void Visit (
        IRdbmsStorageEntityDefinition entityDefinition,
        Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> tableDefinitionHandler,
        Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> filterViewDefinitionHandler,
        Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> unionViewDefinitionHandler,
        Action<NullEntityDefinition, Action<IRdbmsStorageEntityDefinition>> nullEntityDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
      ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

      var visitor = new RdbmsStorageEntityDefinitionVisitor (
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          nullEntityDefinitionHandler);

      entityDefinition.Accept (visitor);
    }

    private class RdbmsStorageEntityDefinitionVisitor<T> : IRdbmsStorageEntityDefinitionVisitor
    {
      private readonly Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _tableDefinitionHandler;
      private readonly Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _filterViewDefinitionHandler;
      private readonly Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _unionViewDefinitionHandler;
      private readonly Func<NullEntityDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _nullEntityDefinitionHandler;

      private T _returnValue;

      public RdbmsStorageEntityDefinitionVisitor (
          Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> tableDefinitionHandler,
          Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> filterViewDefinitionHandler,
          Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> unionViewDefinitionHandler,
          Func<NullEntityDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> nullEntityDefinitionHandler)
      {
        ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
        ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
        ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
        ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

        _tableDefinitionHandler = tableDefinitionHandler;
        _filterViewDefinitionHandler = filterViewDefinitionHandler;
        _unionViewDefinitionHandler = unionViewDefinitionHandler;
        _nullEntityDefinitionHandler = nullEntityDefinitionHandler;
      }

      public T ReturnValue
      {
        get { return _returnValue; }
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

        _returnValue = _tableDefinitionHandler (tableDefinition, ContinueWithNextEntity);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

        _returnValue = _unionViewDefinitionHandler (unionViewDefinition, ContinueWithNextEntity);
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

        _returnValue = _filterViewDefinitionHandler (filterViewDefinition, ContinueWithNextEntity);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

        _returnValue = _nullEntityDefinitionHandler (nullEntityDefinition, ContinueWithNextEntity);
      }

      private T ContinueWithNextEntity (IRdbmsStorageEntityDefinition entityDefinition)
      {
        entityDefinition.Accept (this);
        return _returnValue;
      }
    }

    private class RdbmsStorageEntityDefinitionVisitor : IRdbmsStorageEntityDefinitionVisitor
    {
      private readonly Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> _tableDefinitionHandler;
      private readonly Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> _filterViewDefinitionHandler;
      private readonly Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> _unionViewDefinitionHandler;
      private readonly Action<NullEntityDefinition, Action<IRdbmsStorageEntityDefinition>> _nullEntityDefinitionHandler;

      public RdbmsStorageEntityDefinitionVisitor (
          Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> tableDefinitionHandler,
          Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> filterViewDefinitionHandler,
          Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> unionViewDefinitionHandler,
          Action<NullEntityDefinition, Action<IRdbmsStorageEntityDefinition>> nullEntityDefinitionHandler)
      {
        ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
        ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
        ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
        ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

        _tableDefinitionHandler = tableDefinitionHandler;
        _filterViewDefinitionHandler = filterViewDefinitionHandler;
        _unionViewDefinitionHandler = unionViewDefinitionHandler;
        _nullEntityDefinitionHandler = nullEntityDefinitionHandler;
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

        _tableDefinitionHandler (tableDefinition, ContinueWithNextEntity);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

        _unionViewDefinitionHandler (unionViewDefinition, ContinueWithNextEntity);
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

        _filterViewDefinitionHandler (filterViewDefinition, ContinueWithNextEntity);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

        _nullEntityDefinitionHandler (nullEntityDefinition, ContinueWithNextEntity);
      }

      private void ContinueWithNextEntity (IRdbmsStorageEntityDefinition entityDefinition)
      {
        entityDefinition.Accept (this);
      }
    }
  }
}