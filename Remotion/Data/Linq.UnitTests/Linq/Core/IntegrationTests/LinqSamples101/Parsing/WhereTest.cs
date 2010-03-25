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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.Linq.UnitTests.Linq.Core.IntegrationTests.LinqSamples101.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.IntegrationTests.LinqSamples101.Parsing
{
  /// <summary>
  /// http://msdn.microsoft.com/en-us/bb737944.aspx
  /// </summary>
  [TestFixture]
  public class WhereTest : TestBase
  {
    [Test]
    public void Test_01 ()
    {
      CheckParsedQuery (
          () => from c in QuerySource.Customers where c.City == "London" select c,
#if NET_3_5
          "from Customer c in Customers where ([c].City = \"London\") select [c]"
#else
          "from Customer c in Customers where ([c].City == \"London\") select [c]"
#endif
          );
    }

    [Test]
    public void Test_02 ()
    {
      CheckParsedQuery (
          () => from e in QuerySource.Employees where e.HireDate >= new DateTime (1994, 1, 1) select e,
          "from Employee e in Employees where ([e].HireDate >= 01/01/1994 00:00:00) select [e]");
    }

    [Test]
    public void Test_First_Condition ()
    {
      CheckParsedQuery (
          () => (from o in QuerySource.Orders where o.Freight > 10M select o).First(),
          "from Order o in Orders where ([o].Freight > 10) select [o] => First()");
    }
  }
}
