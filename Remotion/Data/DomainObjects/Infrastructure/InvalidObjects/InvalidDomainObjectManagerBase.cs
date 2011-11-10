// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.Data.DomainObjects.Infrastructure.InvalidObjects
{
  /// <summary>
  /// Keeps a collection of <see cref="DomainObject"/> references that were marked as invalid in a given <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public abstract class InvalidDomainObjectManagerBase : IInvalidDomainObjectManager
  {
    private readonly Dictionary<ObjectID, DomainObject> _invalidObjects = new Dictionary<ObjectID, DomainObject> ();

    public int InvalidObjectCount
    {
      get { return _invalidObjects.Count; }
    }

    public IEnumerable<ObjectID> InvalidObjectIDs
    {
      get { return _invalidObjects.Keys; }
    }

    public bool IsInvalid (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return _invalidObjects.ContainsKey (id);
    }

    public DomainObject GetInvalidObjectReference (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      DomainObject invalidDomainObject;
      if (!_invalidObjects.TryGetValue (id, out invalidDomainObject))
        throw new ArgumentException (String.Format ("The object '{0}' has not been marked invalid.", id), "id");
      else
        return invalidDomainObject;
    }

    public bool MarkInvalid (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      
      if (IsInvalid (domainObject.ID))
      {
        if (GetInvalidObjectReference (domainObject.ID) != domainObject)
        {
          var message = string.Format ("Cannot mark the given object invalid, another object with the same ID '{0}' has already been marked.", domainObject.ID);
          throw new InvalidOperationException (message);
        }

        return false;
      }

      _invalidObjects.Add (domainObject.ID, domainObject);
      return true;
    }

    public bool MarkNotInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _invalidObjects.Remove (objectID);
    }

    public abstract void MarkInvalidThroughHierarchy (DomainObject domainObject);
  }
}