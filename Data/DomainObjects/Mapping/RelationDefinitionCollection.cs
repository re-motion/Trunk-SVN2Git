/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class RelationDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationDefinitionCollection ()
  {
  }

  // standard constructor for collections
  public RelationDefinitionCollection (RelationDefinitionCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (RelationDefinition relationDefinition in collection)
    {
      Add (relationDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

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
