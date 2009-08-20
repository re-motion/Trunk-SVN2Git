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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples
{
  public class EquatableMixin<[BindToTargetType]T> : Mixin<T>, IEquatable<T>
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
        if (!Equals (thisFieldValue, otherFieldValue))
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
