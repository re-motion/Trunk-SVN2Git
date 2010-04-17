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
using Remotion.BridgeInterfaces;
using Remotion.Collections;
using Remotion.Implementation;

namespace Remotion.Security
{
  /// <summary>Represents an access type enum value.</summary>
  /// <remarks>
  /// Use the static <see cref="O:Remotion.Security.AccessType.Get"/> methods to convert an enum to an access type.
  /// <note>For the set of basic access types see <see cref="T:Remotion.Security.GeneralAccessTypes"/>.</note>
  /// </remarks>
  [Serializable]
  public struct AccessType : IEquatable<AccessType>
  {
    private static readonly ICache<Enum, AccessType> s_accessTypeByEnumCache =
        VersionDependentImplementationBridge<IInterlockedCacheFactoryImplementation>.Implementation.CreateCache<Enum, AccessType>();

    public static AccessType Get (Enum accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      return s_accessTypeByEnumCache.GetOrCreateValue (
          accessType,
          key =>
          {
            Type type = key.GetType();
            if (!Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
            {
              throw new ArgumentException (
                  string.Format (
                      "Enumerated type '{0}' cannot be used as an access type. Valid access types must have the {1} applied.",
                      type.FullName,
                      typeof (AccessTypeAttribute).FullName),
                  "accessType");
            }

            return Get (EnumWrapper.Get(accessType));
          });
    }

    public static AccessType Get (EnumWrapper accessType)
    {
      return new AccessType (accessType);
    }

    private EnumWrapper _value;

    private AccessType (EnumWrapper accessType)
    {
      _value = accessType;
    }

    public EnumWrapper Value
    {
      get { return _value; }
    }

    public override string ToString ()
    {
      return _value.ToString();
    }

    public bool Equals (AccessType other)
    {
      return _value.Equals (other._value);
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (obj.GetType() != typeof (AccessType))
        return false;
      return Equals ((AccessType) obj);
    }

    public override int GetHashCode ()
    {
      return _value.GetHashCode();
    }
  }
}