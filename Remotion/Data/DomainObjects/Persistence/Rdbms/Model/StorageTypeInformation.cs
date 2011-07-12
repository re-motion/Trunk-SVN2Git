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
using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="StorageTypeInformation"/> provides information of the storage-type and database-type for a column in a relational database.
  /// </summary>
  // TODO Review 4126: Convert to struct, add tests for Equals and GetHashCode
  public class StorageTypeInformation : IColumnTypeInformation
  {
    private readonly string _storageType;
    private readonly DbType _dbType;

    public StorageTypeInformation (string storageType, DbType dbType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageType", storageType);

      _storageType = storageType;
      _dbType = dbType;
    }

    public string StorageType
    {
      get { return _storageType; }
    }

    public DbType DbType
    {
      get { return _dbType; }
    }
  }
}