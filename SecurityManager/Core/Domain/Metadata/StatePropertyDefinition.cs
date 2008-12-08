// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  public abstract class StatePropertyDefinition : MetadataObject
  {
    public static StatePropertyDefinition NewObject ()
    {
      return NewObject<StatePropertyDefinition>().With();
    }

    public static StatePropertyDefinition NewObject (Guid metadataItemID, string name)
    {
      return NewObject<StatePropertyDefinition>().With (metadataItemID, name);
    }

    public new static StatePropertyDefinition GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StatePropertyDefinition> (id);
    }

    protected StatePropertyDefinition ()
    {
    }

    protected StatePropertyDefinition (Guid metadataItemID, string name)
    {
      MetadataItemID = metadataItemID;
      Name = name;
    }

    //TODO: Rename to StatePropertyReferences
    [DBBidirectionalRelation ("StateProperty")]
    public abstract ObjectList<StatePropertyReference> References { get; }

    [DBBidirectionalRelation ("StateProperty", SortExpression = "[Index] ASC")]
    [Mandatory]
    public abstract ObjectList<StateDefinition> DefinedStates { get; }

    public StateDefinition GetState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return DefinedStates.Single (
          s => s.Name == name,
          () => CreateArgumentException ("name", "The state '{0}' is not defined for the property '{1}'.", name, Name));
    }

    public bool ContainsState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return DefinedStates.Any (s => s.Name == name);
    }

    public StateDefinition GetState (int stateValue)
    {
      return DefinedStates.Single (
          s => s.Value == stateValue,
          () => CreateArgumentException ("stateValue", "A state with the value {0} is not defined for the property '{1}'.", stateValue, Name));
    }

    public bool ContainsState (int stateValue)
    {
      return DefinedStates.Any (s => s.Value == stateValue);
    }

    [StorageClassNone]
    public StateDefinition this [string stateName]
    {
      get { return GetState (stateName); }
    }

    public void AddState (string stateName, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("stateName", stateName);

      StateDefinition newStateDefinition = StateDefinition.NewObject();
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

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException (string.Format (format, args), argumentName);
    }
  }
}
