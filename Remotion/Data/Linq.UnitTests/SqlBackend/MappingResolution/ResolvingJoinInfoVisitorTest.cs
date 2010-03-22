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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.MappingResolution
{
  [TestFixture]
  public class ResolvingJoinInfoVisitorTest
  {
    private IMappingResolver _resolverMock;
    private UnresolvedJoinInfo _unresolvedJoinInfo;
    private UniqueIdentifierGenerator _generator;
    private SqlTable _sqlTable;

    
    [SetUp]
    public void SetUp ()
    {
      _resolverMock = MockRepository.GenerateMock<IMappingResolver>();
      _unresolvedJoinInfo = SqlStatementModelObjectMother.CreateUnresolvedJoinInfo_KitchenCook();
      _generator = new UniqueIdentifierGenerator ();
      _sqlTable = new SqlTable (new UnresolvedTableInfo (Expression.Constant ("Test"), typeof (Cook)));
    }

    [Test]
    public void ResolveJoinInfo_ResolvesJoinInfo ()
    {
      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (string), "Cook", "c");
      var primaryColumn = new SqlColumnExpression (typeof (int), "k", "ID");
      var foreignColumn = new SqlColumnExpression (typeof (int), "c", "KitchenID");

      var resolvedJoinInfo = new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);

      _resolverMock
          .Expect (mock => mock.ResolveJoinInfo (Arg<UnresolvedJoinInfo>.Is.Anything, Arg.Is(_generator)))
          .Return (resolvedJoinInfo);
      _resolverMock.Replay();

      var result = ResolvingJoinInfoVisitor.ResolveJoinInfo (_unresolvedJoinInfo, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (resolvedJoinInfo));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesJoinInfo_AndRevisitsResult ()
    {
      var memberInfo = typeof (Cook).GetProperty ("Substitution");
      var unresolvedResult = new UnresolvedJoinInfo (_sqlTable, memberInfo, JoinCardinality.One);

      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "s");
      var resolvedResult = new ResolvedJoinInfo (
          foreignTableInfo, new SqlColumnExpression (typeof (int), "c", "ID"), new SqlColumnExpression (typeof (int), "s", "ID"));

      using (_resolverMock.GetMockRepository().Ordered())
      {
        _resolverMock
            .Expect (mock => mock.ResolveJoinInfo (_unresolvedJoinInfo, _generator))
            .Return (unresolvedResult);
        _resolverMock
            .Expect (mock => mock.ResolveJoinInfo (unresolvedResult, _generator))
            .Return (resolvedResult);
      }
      _resolverMock.Replay ();

      var result = ResolvingJoinInfoVisitor.ResolveJoinInfo (_unresolvedJoinInfo, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (resolvedResult));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesJoinInfo_AndRevisitsResult_OnlyIfDifferent ()
    {
      var resolvedJoinInfo = new ResolvedJoinInfo (
          new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "c"),
          new SqlColumnExpression (typeof (string), "c", "ID"),
          new SqlColumnExpression (typeof (string), "c", "ID"));
      _resolverMock
          .Expect (mock => mock.ResolveJoinInfo (_unresolvedJoinInfo, _generator))
          .Return (resolvedJoinInfo);
      _resolverMock.Replay ();

      var result = ResolvingJoinInfoVisitor.ResolveJoinInfo (_unresolvedJoinInfo, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (resolvedJoinInfo));
      _resolverMock.VerifyAllExpectations ();
    }
  }
}