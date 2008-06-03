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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Security
{
  //TODO: Hashcode base
  //TODO FS: Introduce ISecurityContext and move to interfaces-assembly/securityassembly
  /// <summary>Collects all security-specific information for an instance or type, and is passed as parameter during the permission check.</summary>
  [Serializable]
  public sealed class SecurityContext : ISecurityContext
  {
    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerTenant;
    private readonly IDictionary<string, EnumWrapper> _states;
    private readonly EnumWrapper[] _abstractRoles;

    public SecurityContext (Type classType)
      : this (classType, null, null, null, null, null)
    {
    }

    public SecurityContext (
        Type classType,
        string owner,
        string ownerGroup,
        string ownerTenant,
        IDictionary<string, Enum> states,
        ICollection<Enum> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("classType", classType, typeof (ISecurableObject));

      _class = TypeUtility.GetPartialAssemblyQualifiedName (classType);
      _owner = StringUtility.NullToEmpty (owner);
      _ownerGroup = StringUtility.NullToEmpty (ownerGroup);
      _ownerTenant = StringUtility.NullToEmpty (ownerTenant);
      _abstractRoles = InitializeAbstractRoles (abstractRoles);
      _states = InitializeStates (states);
    }

    public string Class
    {
      get { return _class; }
    }

    public string Owner
    {
      get { return _owner; }
    }

    public string OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string OwnerTenant
    {
      get { return _ownerTenant; }
    }

    public EnumWrapper[] AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      return _states.ContainsKey (propertyName);
    }

    public bool IsStateless
    {
      get { return _states.Count == 0; }
    }

    public int GetNumberOfStates ()
    {
      return _states.Count;
    }

    public override int GetHashCode ()
    {
      int hashCode = _class.GetHashCode ();
      if (_owner != null)
        hashCode ^= _owner.GetHashCode ();
      if (_ownerGroup != null)
        hashCode ^= _ownerGroup.GetHashCode ();
      if (_ownerTenant != null)
        hashCode ^= _ownerTenant.GetHashCode ();

      return hashCode;
    }

    public override bool Equals (object obj)
    {
      SecurityContext other = obj as SecurityContext;
      if (other == null)
        return false;
      return Equals (other);
    }

    public bool Equals (ISecurityContext other)
    {
      SecurityContext otherContext = other as SecurityContext;
      return otherContext != null && Equals (otherContext);
    }

    public bool Equals (SecurityContext other)
    {
      if (other == null)
        return false;

      if (!this._class.Equals (other._class, StringComparison.Ordinal))
        return false;

      if (!string.Equals (this._owner, other._owner, StringComparison.Ordinal))
        return false;

      if (!string.Equals (this._ownerGroup, other._ownerGroup, StringComparison.Ordinal))
        return false;

      if (!string.Equals (this._ownerTenant, other._ownerTenant, StringComparison.Ordinal))
        return false;

      if (!EqualsStates (this._states, other._states))
        return false;

      return EqualsAbstractRoles (this._abstractRoles, other._abstractRoles);
    }

    private bool EqualsStates (IDictionary<string, EnumWrapper> leftStates, IDictionary<string, EnumWrapper> rightStates)
    {
      if (leftStates.Count != rightStates.Count)
        return false;

      foreach (KeyValuePair<string, EnumWrapper> leftValuePair in leftStates)
      {
        EnumWrapper rightValue;
        if (!rightStates.TryGetValue (leftValuePair.Key, out rightValue))
          return false;
        if (!leftValuePair.Value.Equals (rightValue))
          return false;
      }

      return true;
    }

    private bool EqualsAbstractRoles (EnumWrapper[] leftAbstractRoles, EnumWrapper[] rightAbstractRoles)
    {
      if (leftAbstractRoles.Length != rightAbstractRoles.Length)
        return false;

      foreach (EnumWrapper leftAbstractRole in leftAbstractRoles)
      {
        bool isFound = false;
        foreach (EnumWrapper rightAbstractRole in rightAbstractRoles)
        {
          if (leftAbstractRole.Equals (rightAbstractRole))
          {
            isFound = true;
            break;
          }
        }
        if (!isFound)
          return false;
      }

      return true;
    }

    private EnumWrapper[] InitializeAbstractRoles (ICollection<Enum> abstractRoles)
    {
      List<EnumWrapper> abstractRoleList = new List<EnumWrapper> ();

      if (abstractRoles != null)
      {
        foreach (Enum abstractRole in abstractRoles)
        {
          Type roleType = abstractRole.GetType ();
          if (!Attribute.IsDefined (roleType, typeof (AbstractRoleAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as an abstract role. Valid abstract roles must have the {1} applied.",
                roleType, typeof (AbstractRoleAttribute).FullName);

            throw new ArgumentException (message, "abstractRoles");
          }

          abstractRoleList.Add (new EnumWrapper (abstractRole));
        }
      }
      return abstractRoleList.ToArray ();
    }

    private Dictionary<string, EnumWrapper> InitializeStates (IDictionary<string, Enum> states)
    {
      Dictionary<string, EnumWrapper> securityStates = new Dictionary<string, EnumWrapper> ();

      if (states != null)
      {
        foreach (KeyValuePair<string, Enum> valuePair in states)
        {
          Type stateType = valuePair.Value.GetType ();
          if (!Attribute.IsDefined (stateType, typeof (SecurityStateAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as a security state. Valid security states must have the {1} applied.",
                stateType, typeof (SecurityStateAttribute).FullName);

            throw new ArgumentException (message, "states");
          }

          securityStates.Add (valuePair.Key, new EnumWrapper (valuePair.Value));
        }
      }
      return securityStates;
    }
  }
}
