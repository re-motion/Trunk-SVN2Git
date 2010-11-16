// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.Data.DomainObjects.Mapping
{
  public class ColumnDefinition : IStoragePropertyDefinition
  {
    private readonly string _name;
    private readonly PropertyInfo _propertyInfo;

    public ColumnDefinition (string name, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _name = name;
      _propertyInfo = propertyInfo;
    }

    public string Name
    {
      get { return _name; }
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public bool Equals (ColumnDefinition other)
    {
      if (ReferenceEquals (null, other))
        return false;
      if (ReferenceEquals (this, other))
        return true;
      return Equals (other._name, _name) && Equals (other._propertyInfo, _propertyInfo);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      if (ReferenceEquals (this, obj))
        return true;
      if (obj.GetType() != typeof (ColumnDefinition))
        return false;
      return Equals ((ColumnDefinition) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        return ((_name != null ? _name.GetHashCode() : 0) * 397) ^ (_propertyInfo != null ? _propertyInfo.GetHashCode() : 0);
      }
    }
  }
}