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
    public static ConcreteMixinTypeAttribute FromClassContext (int mixinIndex, ClassContext targetClassContext)
    {
      ArgumentUtility.CheckNotNull ("targetClassContext", targetClassContext);

      var serializer = new AttributeClassContextSerializer ();
      targetClassContext.Serialize (serializer);

      return new ConcreteMixinTypeAttribute (mixinIndex, serializer.Values);
    }

    private readonly int _mixinIndex;
    private readonly object[] _data;

    public ConcreteMixinTypeAttribute (int mixinIndex, object[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      _mixinIndex = mixinIndex;
      _data = data;
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
    }

    public object[] Data
    {
      get { return _data; }
    }

    public MixinDefinition GetMixinDefinition (ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      var respectiveMixedTypeAttribute = new ConcreteMixedTypeAttribute (Data);
      return respectiveMixedTypeAttribute.GetTargetClassDefinition (targetClassDefinitionCache).Mixins[MixinIndex];
    }
  }
}
