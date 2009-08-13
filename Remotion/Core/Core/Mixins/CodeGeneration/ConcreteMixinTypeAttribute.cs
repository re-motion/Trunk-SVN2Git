// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteMixinTypeAttribute : Attribute
  {
    public static ConcreteMixinTypeAttribute Create (ConcreteMixinTypeIdentifier identifier)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);

      var identifierSerializer = new AttributeConcreteMixinTypeIdentifierSerializer ();
      identifier.Serialize (identifierSerializer);

      return new ConcreteMixinTypeAttribute (identifierSerializer.Values);
    }

    private readonly object[] _concreteMixinTypeIdentifierData;

    public ConcreteMixinTypeAttribute (object[] concreteMixinTypeIdentifierData)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifierData", concreteMixinTypeIdentifierData);

      _concreteMixinTypeIdentifierData = concreteMixinTypeIdentifierData;
    }

    public object[] ConcreteMixinTypeIdentifierData
    {
      get { return _concreteMixinTypeIdentifierData; }
    }

    public ConcreteMixinTypeIdentifier GetIdentifier ()
    {
      var deserializer = new AttributeConcreteMixinTypeIdentifierDeserializer (ConcreteMixinTypeIdentifierData);
      return ConcreteMixinTypeIdentifier.Deserialize (deserializer);
    }
  }
}
