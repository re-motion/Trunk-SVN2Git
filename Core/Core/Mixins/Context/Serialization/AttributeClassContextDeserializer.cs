/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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