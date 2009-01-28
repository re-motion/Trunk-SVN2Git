// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;
using Remotion.Data.DomainObjects;
using Remotion.Reflection;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class StateDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static StateDefinition NewObject ()
    {
      return NewObject<StateDefinition> ();
   }

    public static StateDefinition NewObject (string name, int value)
    {
      return NewObject<StateDefinition> (ParamList.Create (name, value));
    }

    public static new StateDefinition GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StateDefinition> (id);
    }

    // member fields

    // construction and disposing

    protected StateDefinition ()
    {
    }

    protected StateDefinition (string name, int value)
    {
      Name = name;
      Value = value;
    }

    // methods and properties

    [DBBidirectionalRelation ("DefinedStates")]
    [Mandatory]
    public abstract StatePropertyDefinition StateProperty { get; set; }

    public override sealed Guid MetadataItemID
    {
      get { throw new NotSupportedException ("States do not support MetadataItemID"); }
      set { throw new NotSupportedException ("States do not support MetadataItemID"); }
    }

    //TODO: Rename to StateUsages
    [EditorBrowsable( EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("StateDefinition")]
    protected abstract ObjectList<StateUsage> Usages { get; }
  }
}
