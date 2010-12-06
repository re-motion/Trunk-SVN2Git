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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class RelationDefinitionCollection : CommonCollection
{
  public RelationDefinitionCollection ()
  {
  }

  public RelationDefinitionCollection (IEnumerable<RelationDefinition> collection, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (var relationDefinition in collection)
      Add (relationDefinition);

    SetIsReadOnly (makeCollectionReadOnly);
  }

  public void SetReadOnly ()
  {
    SetIsReadOnly (true);
  }

  public RelationDefinition GetMandatory (string relationDefinitionID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);

    RelationDefinition relationDefinition = this[relationDefinitionID];
    if (relationDefinition != null)
      return relationDefinition;
    else
      throw CreateMappingException ("Relation '{0}' does not exist.", relationDefinitionID);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    return BaseContains (relationDefinition.ID, relationDefinition);
  }

  public bool Contains (string relationDefinitionID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);
    return BaseContainsKey (relationDefinitionID);
  }

  public RelationDefinition this [int index]  
  {
    get { return (RelationDefinition) BaseGetObject (index); }
  }

  public RelationDefinition this [string relationDefinitionID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);
      return (RelationDefinition) BaseGetObject (relationDefinitionID); 
    }
  }

  public int Add (RelationDefinition value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    
    return BaseAdd (value.ID, value);
  }

  #endregion

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
