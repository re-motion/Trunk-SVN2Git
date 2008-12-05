// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Implementation;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
  /// <summary>Represents an access type enum value.</summary>
  /// <remarks>
  /// Use the static <see cref="O:Remotion.Security.AccessType.Get"/> methods to convert an enum to an access type.
  /// <note>For the set of basic access types see <see cref="T:Remotion.Security.GeneralAccessTypes"/>.</note>
  /// </remarks>
  [Serializable]
  public sealed class AccessType : IObjectReference
  {
    // types

    // static members and constants

    private static readonly ICache<EnumWrapper, AccessType> s_cache =
        VersionDependentImplementationBridge<IAccessTypeCacheImplementation>.Implementation.CreateCache();

    public static AccessType Get (Enum accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      Type type = accessType.GetType();
      if (!Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
      {
        throw new ArgumentException (
            string.Format (
                "Enumerated type '{0}' cannot be used as an access type. Valid access types must have the {1} applied.",
                type.FullName,
                typeof (AccessTypeAttribute).FullName),
            "accessType");
      }

      return Get (new EnumWrapper (accessType));
    }

    public static AccessType Get (EnumWrapper accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      return s_cache.GetOrCreateValue (accessType, key => new AccessType (key));
    }

    // member fields

    private EnumWrapper _value;

    // construction and disposing

    private AccessType (EnumWrapper accessType)
    {
      _value = accessType;
    }

    // methods and properties

    public EnumWrapper Value
    {
      get { return _value; }
    }

    public override string ToString ()
    {
      return _value.ToString();
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return Get (_value);
    }
  }
}
