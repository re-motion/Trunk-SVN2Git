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
  /// Visits the given <see cref="IEntityDefinition"/> and executes a handler based on the entity's type.
  /// </summary>
  public class InlineEntityDefinitionVisitor
  {
    public static T Visit<T> (
        IEntityDefinition entityDefinition,
        Func<TableDefinition, Func<IEntityDefinition, T>, T> tableDefinitionHandler,
        Func<FilterViewDefinition, Func<IEntityDefinition, T>, T> filterViewDefinitionHandler,
        Func<UnionViewDefinition, Func<IEntityDefinition, T>, T> unionViewDefinitionHandler,
        Func<NullEntityDefinition, Func<IEntityDefinition, T>, T> nullEntityDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
      ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

      var visitor = new EntityDefinitionVisitor<T> (
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          nullEntityDefinitionHandler);
      entityDefinition.Accept (visitor);
      return visitor.ReturnValue;
    }

    public static void Visit (
        IEntityDefinition entityDefinition,
        Action<TableDefinition, Action<IEntityDefinition>> tableDefinitionHandler,
        Action<FilterViewDefinition, Action<IEntityDefinition>> filterViewDefinitionHandler,
        Action<UnionViewDefinition, Action<IEntityDefinition>> unionViewDefinitionHandler,
        Action<NullEntityDefinition, Action<IEntityDefinition>> nullEntityDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("tableDefinitionHandler", tableDefinitionHandler);
      ArgumentUtility.CheckNotNull ("filterViewDefinitionHandler", filterViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("unionViewDefinitionHandler", unionViewDefinitionHandler);
      ArgumentUtility.CheckNotNull ("nullEntityDefinitionHandler", nullEntityDefinitionHandler);

      var visitor = new EntityDefinitionVisitor (
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          nullEntityDefinitionHandler);

      entityDefinition.Accept (visitor);
    }

    private class EntityDefinitionVisitor<T> : IEntityDefinitionVisitor
    {
      private readonly Func<TableDefinition, Func<IEntityDefinition, T>, T> _tableDefinitionHandler;
      private readonly Func<FilterViewDefinition, Func<IEntityDefinition, T>, T> _filterViewDefinitionHandler;
      private readonly Func<UnionViewDefinition, Func<IEntityDefinition, T>, T> _unionViewDefinitionHandler;
      private readonly Func<NullEntityDefinition, Func<IEntityDefinition, T>, T> _nullEntityDefinitionHandler;

      private T _returnValue;

      public EntityDefinitionVisitor (
          Func<TableDefinition, Func<IEntityDefinition, T>, T> tableDefinitionHandler,
          Func<FilterViewDefinition, Func<IEntityDefinition, T>, T> filterViewDefinitionHandler,
          Func<UnionViewDefinition, Func<IEntityDefinition, T>, T> unionViewDefinitionHandler,
          Func<NullEntityDefinition, Func<IEntityDefinition, T>, T> nullEntityDefinitionHandler)
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

      private T ContinueWithNextEntity (IEntityDefinition entityDefinition)
      {
        entityDefinition.Accept (this);
        return _returnValue;
      }
    }

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private readonly Action<TableDefinition, Action<IEntityDefinition>> _tableDefinitionHandler;
      private readonly Action<FilterViewDefinition, Action<IEntityDefinition>> _filterViewDefinitionHandler;
      private readonly Action<UnionViewDefinition, Action<IEntityDefinition>> _unionViewDefinitionHandler;
      private readonly Action<NullEntityDefinition, Action<IEntityDefinition>> _nullEntityDefinitionHandler;

      public EntityDefinitionVisitor (
          Action<TableDefinition, Action<IEntityDefinition>> tableDefinitionHandler,
          Action<FilterViewDefinition, Action<IEntityDefinition>> filterViewDefinitionHandler,
          Action<UnionViewDefinition, Action<IEntityDefinition>> unionViewDefinitionHandler,
          Action<NullEntityDefinition, Action<IEntityDefinition>> nullEntityDefinitionHandler)
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

      private void ContinueWithNextEntity (IEntityDefinition entityDefinition)
      {
        entityDefinition.Accept (this);
      }
    }
  }
}