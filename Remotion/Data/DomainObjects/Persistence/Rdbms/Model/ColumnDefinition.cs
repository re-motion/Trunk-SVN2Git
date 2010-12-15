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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class ColumnDefinition : IStoragePropertyDefinition
  {
    private readonly string _name;
    private readonly Type _propertyType;
    private readonly bool _isNullable;
    private readonly string _storageType;

    public ColumnDefinition (string name, Type propertyType, string storageType, bool isNullable)
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

    public override string ToString ()
    {
      return string.Format ("{0} {1} {2}", Name, StorageType, IsNullable ? "NULL" : "NOT NULL");
    }
  }
}