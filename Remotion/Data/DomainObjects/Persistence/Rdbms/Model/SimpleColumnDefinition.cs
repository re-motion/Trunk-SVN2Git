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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class SimpleColumnDefinition : IColumnDefinition
  {
    private readonly string _name;
    private readonly Type _propertyType;
    private readonly bool _isNullable;
    private readonly string _storageType;

    public SimpleColumnDefinition (string name, Type propertyType, string storageType, bool isNullable)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNullOrEmpty ("storageType", storageType);
      
      _name = name;
      _propertyType = propertyType;
      _storageType = storageType;
      _isNullable = isNullable;
    }

    public string Name
    {
      get { return _name; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public string StorageType
    {
      get { return _storageType; }
    }

    public bool IsNullable
    {
      get { return _isNullable; }
    }

    public void Accept (IColumnDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitSimpleColumnDefinition (this);
    }

    public bool Equals (IColumnDefinition other)
    {
      if (other == null || other.GetType () != GetType ())
        return false;

      var castOther = (SimpleColumnDefinition) other;
      return castOther.Name == Name 
          && castOther.PropertyType == PropertyType 
          && castOther.StorageType == StorageType 
          && castOther.IsNullable == IsNullable;
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as IColumnDefinition);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (Name, PropertyType, StorageType, IsNullable);
    }

    public override string ToString ()
    {
      return string.Format ("{0} {1} {2}", Name, StorageType, IsNullable ? "NULL" : "NOT NULL");
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}