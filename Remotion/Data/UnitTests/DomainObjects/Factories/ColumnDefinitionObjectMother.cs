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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public static class ColumnDefinitionObjectMother
  {
    public static readonly ColumnDefinition ObjectIDColumn = new ColumnDefinition ("ID", typeof(Guid), "uniqueidentifier", false, true);
    public static readonly ColumnDefinition ClassIDColumn = new ColumnDefinition ("ClassID", typeof (string), "varchar", true, false);
    public static readonly ColumnDefinition TimestampColumn = new ColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);

    public static ColumnDefinition CreateColumn ()
    {
      return new ColumnDefinition (Guid.NewGuid().ToString(), typeof (string), "varchar", true, false);
    }

    public static ColumnDefinition CreateColumn (string columnName)
    {
      return new ColumnDefinition (columnName, typeof (string), "varchar", true, false);
    }

    public static ColumnDefinition CreateTypedColumn (string columnName, Type propertyType, string storageType)
    {
      return new ColumnDefinition (columnName, propertyType, storageType, false, true);
    }

  }
}