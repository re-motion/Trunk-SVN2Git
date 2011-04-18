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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="SimpleColumnDefinitionFindingVisitor"/> findes all <see cref="SimpleColumnDefinition"/>s in the specified column list.
  /// </summary>
  public class SimpleColumnDefinitionFindingVisitor : IColumnDefinitionVisitor
  {
    public static IEnumerable<SimpleColumnDefinition> FindSimpleColumnDefinitions (IEnumerable<IColumnDefinition> columnDefinitions)
    {
      ArgumentUtility.CheckNotNull ("columnDefinitions", columnDefinitions);

      var visitor = new SimpleColumnDefinitionFindingVisitor();
      foreach (var columnDefinition in columnDefinitions)
        columnDefinition.Accept (visitor);
      return visitor.GetSimpleColumns();
    }

    private readonly List<SimpleColumnDefinition> _columnDefinitions;

    public SimpleColumnDefinitionFindingVisitor ()
    {
      _columnDefinitions = new List<SimpleColumnDefinition>();
    }

    public void VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("simpleColumnDefinition", simpleColumnDefinition);

      _columnDefinitions.Add (simpleColumnDefinition);
    }

    public void VisitIDColumnDefinition (IDColumnDefinition idColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("idColumnDefinition", idColumnDefinition);

      idColumnDefinition.ObjectIDColumn.Accept (this);
      if (idColumnDefinition.HasClassIDColumn)
        idColumnDefinition.ClassIDColumn.Accept (this);
    }

    public void VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullColumnDefinition", nullColumnDefinition);
    }

    public IEnumerable<SimpleColumnDefinition> GetSimpleColumns ()
    {
      return _columnDefinitions.AsReadOnly();
    }
  }
}