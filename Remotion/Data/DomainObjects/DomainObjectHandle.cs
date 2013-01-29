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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Implements <see cref="IDomainObjectHandle{T}"/>, providing a typed handle to a <see cref="DomainObject"/>.
  /// Since this class is not covariant, instances are usually accessed through the <see cref="IDomainObjectHandle{T}"/> interface instead.
  /// </summary>
  /// <typeparam name="T">The type of <see cref="DomainObject"/> identified by this class.</typeparam>
  [Serializable]
  public class DomainObjectHandle<T> : IDomainObjectHandle<T>, IEquatable<DomainObjectHandle<T>>
      where T : DomainObject
  {
    public static object Parse (string formattedString)
    {
      ArgumentUtility.CheckNotNull ("formattedString", formattedString);

      var objectID = ObjectID.Parse (formattedString);
      return new DomainObjectHandle<T> (objectID);
    }

    private readonly ObjectID _objectID;

    public DomainObjectHandle (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      if (objectID.ClassDefinition.ClassType != typeof (T))
      {
        var message = string.Format ("The class type of ObjectID '{0}' doesn't match the handle type '{1}'.", objectID, typeof (T));
        throw new ArgumentException (message, "objectID");
      }

      _objectID = objectID;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public IDomainObjectHandle<TOther> Cast<TOther> () 
      where TOther : DomainObject
    {
      try
      {
        return (IDomainObjectHandle<TOther>) this;
      }
      catch (InvalidCastException ex)
      {
        var message = string.Format ("The handle for object '{0}' cannot be represented as a handle for type '{1}'.", _objectID, typeof (TOther));
        throw new InvalidCastException (message, ex);
      }
    }

    public bool Equals (DomainObjectHandle<T> other)
    {
      if (ReferenceEquals (null, other))
        return false;
      if (ReferenceEquals (this, other))
        return true;
      return Equals (_objectID, other._objectID);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      if (ReferenceEquals (this, obj))
        return true;
      if (obj.GetType () != this.GetType ())
        return false;
      return Equals ((DomainObjectHandle<T>) obj);
    }

    public override int GetHashCode ()
    {
      return (_objectID != null ? _objectID.GetHashCode () : 0);
    }

    public override string ToString ()
    {
      return _objectID.ToString();
    }
  }
}