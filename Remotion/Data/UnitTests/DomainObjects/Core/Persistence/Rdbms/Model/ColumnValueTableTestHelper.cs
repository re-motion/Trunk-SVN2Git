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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  public static class ColumnValueTableTestHelper
  {
    public static void CheckTable (ColumnValueTable expectedTable, ColumnValueTable actualTable)
    {
      Assert.That (actualTable.Columns, Is.EqualTo (expectedTable.Columns));
      var actualRows = actualTable.Rows.ToArray();
      var expectedRows = expectedTable.Rows.ToArray();
      Assert.That (actualRows.Length, Is.EqualTo (expectedRows.Length));
      for (int i = 0; i < expectedRows.Length; ++i)
        Assert.That (actualRows[i].Values, Is.EqualTo (expectedRows[i].Values));
    }
  }
}