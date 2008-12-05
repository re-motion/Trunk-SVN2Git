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
using System.Runtime.Serialization;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.Serialization
{
  public class AttributeClassContextDeserializer : IClassContextDeserializer
  {
    private readonly object[] _values;

    public AttributeClassContextDeserializer (object[] values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      
      if (values.Length != 3)
        throw new ArgumentException ("Expected an array with 3 elements.", "values");
      
      _values = values;
    }

    public Type GetClassType ()
    {
      return GetValue<Type> (0);
    }

    public IEnumerable<MixinContext> GetMixins()
    {
      var mixins = GetValue<object[][]> (1);
      return mixins.Select (oa => MixinContext.Deserialize (new AttributeMixinContextDeserializer (oa)));
    }

    public IEnumerable<Type> GetCompleteInterfaces()
    {
      return GetValue<Type[]> (2);
    }

    private T GetValue<T> (int index)
    {
      var value = _values[index];

      if (!(value is T))
      {
        var message = string.Format ("Expected value of type '{0}' at index {1} in the values array, but found '{2}'.",
            typeof (T).FullName, index, value != null ? value.GetType ().FullName : "null");
        throw new SerializationException (message);
      }

      return (T) value;
    }
  }
}
