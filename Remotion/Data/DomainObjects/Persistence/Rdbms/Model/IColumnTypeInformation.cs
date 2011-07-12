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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="IColumnTypeInformation"/> defines the API for all implementations that are responsible to hold information about how a column is 
  /// declared in a relational database.
  /// </summary>
  // TODO Review 4126: Remove this interface
  public interface IColumnTypeInformation
  {
    string StorageType { get; }
    DbType DbType { get; }
  }
}