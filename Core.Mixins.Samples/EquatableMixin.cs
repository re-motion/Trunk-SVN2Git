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
using Remotion.Mixins;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples
{
  public class EquatableMixin<T> : Mixin<T>, IEquatable<T>
     where T : class
  {
    private FieldInfo[] _targetFields;

    protected override void OnInitialized ()
    {
      base.OnInitialized ();
      _targetFields = typeof (T).GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    bool IEquatable<T>.Equals (T other)
    {
      if (other == null)
        return false;

      foreach (FieldInfo field in _targetFields)
      {
        object thisFieldValue = field.GetValue (This);
        object otherFieldValue = field.GetValue (other);
        if (!object.Equals (thisFieldValue, otherFieldValue))
          return false;
      }
      return true;
    }

    [OverrideTarget]
    protected new bool Equals (object other)
    {
      return ((IEquatable<T>)this).Equals (other as T);
    }

    [OverrideTarget]
    protected new int GetHashCode ()
    {
      object[] fieldValues = new object[_targetFields.Length];
      for (int i = 0; i < fieldValues.Length; ++i)
        fieldValues[i] = _targetFields[i].GetValue (This);
      
      return EqualityUtility.GetRotatedHashCode (fieldValues);
    }
  }
}
