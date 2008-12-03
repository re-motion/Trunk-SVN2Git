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

namespace Remotion.Mixins.Context.Serialization
{
  public class AttributeMixinContextDeserializer : IMixinContextDeserializer
  {
    private readonly object[] _values;

    public AttributeMixinContextDeserializer(object[] values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      
      if (values.Length != 4)
        throw new ArgumentException ("Expected an array with 4 elements.", "values");
      
      _values = values;
    }

    public Type GetMixinType()
    {
      return GetValue<Type> (0);
    }

    public MixinKind GetMixinKind()
    {
      return GetValue<MixinKind> (1);
    }

    public MemberVisibility GetIntroducedMemberVisibility()
    {
      return GetValue<MemberVisibility> (2);
    }

    public IEnumerable<Type> GetExplicitDependencies()
    {
      return GetValue<Type[]> (3);
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