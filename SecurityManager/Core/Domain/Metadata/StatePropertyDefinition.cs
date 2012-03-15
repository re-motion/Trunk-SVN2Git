// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
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
      return NewObject<StatePropertyDefinition>();
    }

    public static StatePropertyDefinition NewObject (Guid metadataItemID, string name)
    {
      return NewObject<StatePropertyDefinition> (ParamList.Create (metadataItemID, name));
    }

    public new static StatePropertyDefinition GetObject (ObjectID id)
    {
      return GetObject<StatePropertyDefinition> (id);
    }

    protected StatePropertyDefinition ()
    {
    }

    protected StatePropertyDefinition (Guid metadataItemID, string name)
    {
      MetadataItemID = metadataItemID;
      Name = name;
    }

    [DBBidirectionalRelation ("StateProperty")]
    protected abstract ObjectList<StatePropertyReference> StatePropertyReferences { get; }

    [DBBidirectionalRelation ("StateProperty")]
    [Mandatory]
    protected abstract ObjectList<StateDefinition> DefinedStatesInternal { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StateDefinition> DefinedStates
    {
      get { return DefinedStatesInternal.OrderBy (s => s.Index).ToList().AsReadOnly(); }
    }

    public StateDefinition GetState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return DefinedStatesInternal.Single (
          s => s.Name == name,
          () => CreateArgumentException ("name", "The state '{0}' is not defined for the property '{1}'.", name, Name));
    }

    public bool ContainsState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return DefinedStatesInternal.Any (s => s.Name == name);
    }

    public StateDefinition GetState (int stateValue)
    {
      return DefinedStatesInternal.Single (
          s => s.Value == stateValue,
          () => CreateArgumentException ("stateValue", "A state with the value {0} is not defined for the property '{1}'.", stateValue, Name));
    }

    public bool ContainsState (int stateValue)
    {
      return DefinedStatesInternal.Any (s => s.Value == stateValue);
    }

    [StorageClassNone]
    public StateDefinition this [string stateName]
    {
      get { return GetState (stateName); }
    }

    public void AddState (StateDefinition newState)
    {
      ArgumentUtility.CheckNotNull ("newState", newState);
      DefinedStatesInternal.Add (newState);
    }

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException (string.Format (format, args), argumentName);
    }
  }
}