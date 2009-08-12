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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteMixinTypeAttribute : Attribute
  {
    public static ConcreteMixinTypeAttribute Create (ClassContext targetClassContext, int mixinIndex, ConcreteMixinTypeIdentifier identifier)
    {
      ArgumentUtility.CheckNotNull ("targetClassContext", targetClassContext);

      var classContextSerializer = new AttributeClassContextSerializer ();
      targetClassContext.Serialize (classContextSerializer);

      var identifierSerializer = new AttributeConcreteMixinTypeIdentifierSerializer ();
      identifier.Serialize (identifierSerializer);

      return new ConcreteMixinTypeAttribute (classContextSerializer.Values, mixinIndex, identifierSerializer.Values);
    }

    private readonly object[] _classContextData;
    private readonly int _mixinIndex;
    private readonly object[] _concreteMixinTypeIdentifierData;

    public ConcreteMixinTypeAttribute (object[] classContextData, int mixinIndex, object[] concreteMixinTypeIdentifierData)
    {
      ArgumentUtility.CheckNotNull ("classContextData", classContextData);
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifierData", concreteMixinTypeIdentifierData);

      _mixinIndex = mixinIndex;
      _classContextData = classContextData;
      _concreteMixinTypeIdentifierData = concreteMixinTypeIdentifierData;
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
    }

    public object[] ClassContextData
    {
      get { return _classContextData; }
    }

    public object[] ConcreteMixinTypeIdentifierData
    {
      get { return _concreteMixinTypeIdentifierData; }
    }

    public MixinDefinition GetMixinDefinition (ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      var deserializer = new AttributeClassContextDeserializer (ClassContextData);
      var classContext = ClassContext.Deserialize (deserializer);

      return targetClassDefinitionCache.GetTargetClassDefinition (classContext).Mixins[MixinIndex];
    }

    public ConcreteMixinTypeIdentifier GetIdentifier ()
    {
      var deserializer = new AttributeConcreteMixinTypeIdentifierDeserializer (ConcreteMixinTypeIdentifierData);
      return ConcreteMixinTypeIdentifier.Deserialize (deserializer);
    }
  }
}
