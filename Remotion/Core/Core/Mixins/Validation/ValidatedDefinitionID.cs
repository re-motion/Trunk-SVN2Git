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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Identifies a validated <see cref="IVisitableDefinition"/> without actually holding on to the definition instance.
  /// </summary>
  [Serializable]
  public sealed class ValidatedDefinitionID : IEquatable<ValidatedDefinitionID>
  {
    public static ValidatedDefinitionID FromDefinition (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var parentDefinitionID = definition.Parent == null ? null : FromDefinition (definition.Parent);
      return new ValidatedDefinitionID (definition.GetType().Name, definition.FullName, parentDefinitionID);
    }

    public static bool operator== (ValidatedDefinitionID definition1, ValidatedDefinitionID definition2)
    {
      return Equals (definition1, definition2);
    }

    public static bool operator!= (ValidatedDefinitionID definition1, ValidatedDefinitionID definition2)
    {
      return !(definition1 == definition2);
    }

    private readonly string _kind;
    private readonly string _fullName;
    private readonly ValidatedDefinitionID _parentID;

    public ValidatedDefinitionID (string kind, string fullName, ValidatedDefinitionID parentID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);
      ArgumentUtility.CheckNotNullOrEmpty ("fullName", fullName);

      _kind = kind;
      _parentID = parentID;
      _fullName = fullName;
    }

    public string Kind
    {
      get { return _kind; }
    }

    public string FullName
    {
      get { return _fullName; }
    }

    public ValidatedDefinitionID ParentID
    {
      get { return _parentID; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as ValidatedDefinitionID;
      return Equals(other);
    }

    public bool Equals (ValidatedDefinitionID other)
    {
      return !ReferenceEquals (other, null) && other.Kind == Kind && other.FullName == FullName && Equals (other.ParentID, ParentID);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (Kind, FullName, ParentID);
    }

    public override string ToString ()
    {
      return (ParentID == null ? "" : ParentID + " -> ") + Kind + ":" + FullName;
    }
  }
}