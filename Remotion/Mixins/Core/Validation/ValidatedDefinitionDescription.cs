// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Reflection.MemberSignatures;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Describes a validated <see cref="IVisitableDefinition"/> for output without actually holding on to the definition instance.
  /// </summary>
  [Serializable]
  public sealed class ValidatedDefinitionDescription : IEquatable<ValidatedDefinitionDescription>
  {
    public static ValidatedDefinitionDescription FromDefinition (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var parentDefinitionID = definition.Parent == null ? null : FromDefinition (definition.Parent);
      string signature = GetSignatureString (definition);
      return new ValidatedDefinitionDescription (definition.GetType().Name, definition.FullName, signature, parentDefinitionID);
    }

    private static string GetSignatureString (IVisitableDefinition definition)
    {
      var definitionAsMemberDefinition = definition as MemberDefinitionBase;
      if (definitionAsMemberDefinition == null)
        return null;

      var memberSignature = MemberSignatureProvider.GetMemberSignature (definitionAsMemberDefinition.MemberInfo);
      return memberSignature.ToString();
    }

    public static bool operator== (ValidatedDefinitionDescription definition1, ValidatedDefinitionDescription definition2)
    {
      return Equals (definition1, definition2);
    }

    public static bool operator!= (ValidatedDefinitionDescription definition1, ValidatedDefinitionDescription definition2)
    {
      return !(definition1 == definition2);
    }

    private readonly string _kind;
    private readonly string _fullName;
    private readonly string _signature;
    private readonly ValidatedDefinitionDescription _parentDescription;

    public ValidatedDefinitionDescription (string kind, string fullName, string signature, ValidatedDefinitionDescription parentDescription)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);
      ArgumentUtility.CheckNotNullOrEmpty ("fullName", fullName);

      _kind = kind;
      _fullName = fullName;
      _signature = signature;
      _parentDescription = parentDescription;
    }

    public string Kind
    {
      get { return _kind; }
    }

    public string FullName
    {
      get { return _fullName; }
    }

    public string Signature
    {
      get { return _signature; }
    }

    public ValidatedDefinitionDescription ParentDescription
    {
      get { return _parentDescription; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as ValidatedDefinitionDescription;
      return Equals(other);
    }

    public bool Equals (ValidatedDefinitionDescription other)
    {
      return !ReferenceEquals (other, null) 
          && other.Kind == Kind 
          && other.FullName == FullName 
          && other.Signature == Signature
          && Equals (other.ParentDescription, ParentDescription);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (Kind, FullName, Signature, ParentDescription);
    }

    public override string ToString ()
    {
      var parentPath = ParentDescription == null ? "" : ParentDescription + " -> ";
      var infoString = " [" + Kind + (Signature != null ? ", " + Signature : "") + "]";
      return parentPath + FullName + infoString;
    }
  }
}