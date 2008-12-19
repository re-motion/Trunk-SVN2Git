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
using System.Collections.Generic;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    public static ConcreteMixedTypeAttribute FromClassContext (ClassContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var serializer = new AttributeClassContextSerializer ();
      context.Serialize (serializer);

      return new ConcreteMixedTypeAttribute (serializer.Values);
    }

    private readonly object[] _data;

    public ConcreteMixedTypeAttribute (object[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      _data = data;
    }

    public object[] Data
    {
      get { return _data; }
    }

    public ClassContext GetClassContext ()
    {
      var deserializer = new AttributeClassContextDeserializer (Data);
      return ClassContext.Deserialize (deserializer);
    }

    public virtual TargetClassDefinition GetTargetClassDefinition (ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      return targetClassDefinitionCache.GetTargetClassDefinition (GetClassContext ());
    }
  }
}
