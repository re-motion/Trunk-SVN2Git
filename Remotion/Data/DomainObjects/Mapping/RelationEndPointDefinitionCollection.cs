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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public class RelationEndPointDefinitionCollection : CommonCollection, IEnumerable<IRelationEndPointDefinition>
  {
    public static RelationEndPointDefinitionCollection CreateForAllRelationEndPoints (ClassDefinition classDefinition, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return
          new RelationEndPointDefinitionCollection (
              classDefinition.CreateSequence (cd => cd.BaseClass).SelectMany (cd => cd.MyRelationEndPointDefinitions), makeCollectionReadOnly);
    }

    public RelationEndPointDefinitionCollection ()
    {
    }

    public RelationEndPointDefinitionCollection (IEnumerable<IRelationEndPointDefinition> collection, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      foreach (var relationEndPoint in collection)
        Add (relationEndPoint);

      SetIsReadOnly (makeCollectionReadOnly);
    }

    public void SetReadOnly ()
    {
      SetIsReadOnly (true);
    }

    public new IEnumerator<IRelationEndPointDefinition> GetEnumerator ()
    {
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (IRelationEndPointDefinition relationEndPoint in (IEnumerable) this) // use base implementation
          // ReSharper restore LoopCanBeConvertedToQuery
        yield return relationEndPoint;
    }

    #region Standard implementation for "add-only" collections

    public bool Contains (IRelationEndPointDefinition relationEndPoint)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

      return BaseContains (relationEndPoint.PropertyName, relationEndPoint);
    }

    public bool Contains (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      return BaseContainsKey (propertyName);
    }

    public IRelationEndPointDefinition this [int index]
    {
      get { return (IRelationEndPointDefinition) BaseGetObject (index); }
    }

    public IRelationEndPointDefinition this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        return (IRelationEndPointDefinition) BaseGetObject (propertyName);
      }
    }

    public int Add (IRelationEndPointDefinition value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      if (string.IsNullOrEmpty (value.PropertyName))
        throw new InvalidOperationException ("End points without property name cannot be added to this collection.");

      int position = BaseAdd (value.PropertyName, value);

      return position;
    }

    #endregion
  }
}