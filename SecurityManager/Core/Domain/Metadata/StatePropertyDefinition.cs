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
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  public abstract class StatePropertyDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static StatePropertyDefinition NewObject ()
    {
      return NewObject<StatePropertyDefinition> ().With ();
    }

    public static StatePropertyDefinition NewObject (Guid metadataItemID, string name)
    {
      return NewObject<StatePropertyDefinition> ().With (metadataItemID, name);
    }

    public static new StatePropertyDefinition GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StatePropertyDefinition> (id);
    }

    // member fields

    // construction and disposing

    protected StatePropertyDefinition ()
    {
    }

    protected StatePropertyDefinition (Guid metadataItemID, string name)
    {
      MetadataItemID = metadataItemID;
      Name = name;
    }

    // methods and properties

    //TODO: Rename to StatePropertyReferences
    [DBBidirectionalRelation ("StateProperty")]
    public abstract ObjectList<StatePropertyReference> References { get; }

    [DBBidirectionalRelation ("StateProperty", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateDefinition> DefinedStates { get; }

    public StateDefinition GetState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Name == name)
          return state;
      }

      throw new ArgumentException (string.Format ("The state '{0}' is not defined for the property '{1}'.", name, Name), "name");
    }

    public bool ContainsState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Name == name)
          return true;
      }

      return false;
    }

    public StateDefinition GetState (int stateValue)
    {
      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Value == stateValue)
          return state;
      }

      throw new ArgumentException (string.Format ("A state with the value {0} is not defined for the property '{1}'.", stateValue, Name), "stateValue");
    }

    public bool ContainsState (int stateValue)
    {
      foreach (StateDefinition state in DefinedStates)
      {
        if (state.Value == stateValue)
          return true;
      }

      return false;
    }

    [StorageClassNone]
    public StateDefinition this[string stateName]
    {
      get { return GetState (stateName); }
    }

    public void AddState (string stateName, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("stateName", stateName);

      StateDefinition newStateDefinition = StateDefinition.NewObject ();
      newStateDefinition.Name = stateName;
      newStateDefinition.Value = value;
      newStateDefinition.Index = value;

      AddState (newStateDefinition);
    }

    public void AddState (StateDefinition newState)
    {
      ArgumentUtility.CheckNotNull ("newState", newState);
      DefinedStates.Add (newState);
    }
  }
}
