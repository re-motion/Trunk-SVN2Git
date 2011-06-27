// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.Resolved
{
  [TestFixture]
  public class ResolvedSimpleTableInfoTest
  {
    private ResolvedSimpleTableInfo _tableInfo;

    [SetUp]
    public void SetUp ()
    {
      _tableInfo = SqlStatementModelObjectMother.CreateResolvedTableInfo ();
    }

    [Test]
    public void Accept ()
    {
      var tableInfoVisitorMock = MockRepository.GenerateMock<ITableInfoVisitor>();
      tableInfoVisitorMock.Expect (mock => mock.VisitSimpleTableInfo (_tableInfo));

      tableInfoVisitorMock.Replay();
      _tableInfo.Accept (tableInfoVisitorMock);

      tableInfoVisitorMock.VerifyAllExpectations();
    }

    [Test]
    public void GetResolvedTableInfo ()
    {
      var result = _tableInfo.GetResolvedTableInfo();

      Assert.That (result, Is.SameAs (_tableInfo));
    }

    [Test]
    public new void ToString ()
    {
      var tableInfo = new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "t0");
      var result = tableInfo.ToString ();
      
      Assert.That (result, Is.EqualTo ("[CookTable] [t0]"));
    }
  }
}