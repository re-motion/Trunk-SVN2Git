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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Base class for proxy objects used instead of the organizational structure domain objects.
  /// </summary>
  [Serializable]
  public abstract class OrganizationalStructureObjectProxy<T> : BindableObjectWithIdentityBase
      where T : BaseSecurityManagerObject
  {
    private readonly IObjectID<T> _id;
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    protected OrganizationalStructureObjectProxy (IObjectID<T> id, string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _id = id;
      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
    }

    public ObjectID ID
    {
      get { return _id.AsObjectID(); }
    }

    public IObjectID<T> TypedID
    {
      get { return _id; }
    }

    public override string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public override string DisplayName
    {
      get { return _displayName; }
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (this.GetType() != obj.GetType())
        return false;
      return this._id.Equals (((OrganizationalStructureObjectProxy<T>) obj)._id);
    }

    public override int GetHashCode ()
    {
      return _id.GetHashCode();
    }
  }
}