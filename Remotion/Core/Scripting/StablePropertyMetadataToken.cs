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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  public class StablePropertyMetadataToken : StableMetadataToken
  {
    private const int NoMethodToken = -1;

    public static int GetPropertyGetterSetterMetaDataToken (PropertyInfo property, bool getGetter)
    {
      var method = getGetter ? property.GetGetMethod () : property.GetSetMethod ();
      int token = NoMethodToken;
      if (method != null)
      {
        token = method.GetBaseDefinition ().MetadataToken;
      }
      return token;
    }

    private readonly int _getterToken;
    private readonly int _setterToken;


    public StablePropertyMetadataToken (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      _getterToken = GetPropertyGetterSetterMetaDataToken (property, true);
      _setterToken = GetPropertyGetterSetterMetaDataToken (property, false);
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as StablePropertyMetadataToken);
    }

    //public bool Equals (StablePropertyMetadataToken other)
    //{
    //  if (ReferenceEquals (null, other))
    //    return false;

    //  bool getterEqual = (_getterToken == NoMethodToken || other._getterToken == NoMethodToken) || (_getterToken == other._getterToken);
    //  return other._token == _token;
    //}

    //public override int GetHashCode ()
    //{
    //  return _token;
    //}  
  }
}