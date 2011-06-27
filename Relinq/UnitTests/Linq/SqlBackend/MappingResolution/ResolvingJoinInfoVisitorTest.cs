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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.MappingResolution
{
  [TestFixture]
  public class ResolvingJoinInfoVisitorTest
  {
    private IMappingResolver _resolverMock;
    private UnresolvedJoinInfo _unresolvedJoinInfo;
    private UniqueIdentifierGenerator _generator;
    private IMappingResolutionStage _stageMock;
    private IMappingResolutionContext _mappingResolutionContext;


    [SetUp]
    public void SetUp ()
    {
      _resolverMock = MockRepository.GenerateMock<IMappingResolver>();
      _unresolvedJoinInfo = SqlStatementModelObjectMother.CreateUnresolvedJoinInfo_KitchenCook();
      _generator = new UniqueIdentifierGenerator();
      _stageMock = MockRepository.GenerateMock<IMappingResolutionStage>();
      _mappingResolutionContext = new MappingResolutionContext();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesUnresolvedJoinInfo ()
    {
      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (string), "Cook", "c");
      var primaryColumn = new SqlColumnDefinitionExpression (typeof (int), "k", "ID", false);
      var foreignColumn = new SqlColumnDefinitionExpression (typeof (int), "c", "KitchenID", false);

      var resolvedJoinInfo = new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);

      _resolverMock
          .Expect (mock => mock.ResolveJoinInfo (Arg<UnresolvedJoinInfo>.Is.Anything, Arg.Is (_generator)))
          .Return (resolvedJoinInfo);
      _resolverMock.Replay();

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (foreignTableInfo, _mappingResolutionContext))
          .Return (foreignTableInfo);
      _stageMock.Replay ();
      
      var result = ResolvingJoinInfoVisitor.ResolveJoinInfo (_unresolvedJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);

      Assert.That (result, Is.SameAs (resolvedJoinInfo));
      _resolverMock.VerifyAllExpectations();
      _stageMock.VerifyAllExpectations();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesUnresolvedJoinInfo_AndRevisitsResult ()
    {
      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "c");
      var resolvedJoinInfo = new ResolvedJoinInfo (
          foreignTableInfo,
          new SqlColumnDefinitionExpression (typeof (string), "c", "ID", false),
          new SqlColumnDefinitionExpression (typeof (string), "c", "ID", false));
      _resolverMock
          .Expect (mock => mock.ResolveJoinInfo (_unresolvedJoinInfo, _generator))
          .Return (resolvedJoinInfo);
      _resolverMock.Replay();

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (foreignTableInfo, _mappingResolutionContext))
          .Return (foreignTableInfo);
      _stageMock.Replay ();

      var result = ResolvingJoinInfoVisitor.ResolveJoinInfo (_unresolvedJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);

      Assert.That (result, Is.SameAs (resolvedJoinInfo));
      _resolverMock.VerifyAllExpectations();
      _stageMock.VerifyAllExpectations();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesCollectionJoinInfo ()
    {
      var memberInfo = typeof (Cook).GetProperty ("IllnessDays");
      var unresolvedCollectionJoinInfo = new UnresolvedCollectionJoinInfo (Expression.Constant (new Cook()), memberInfo);

      var sqlEntityExpression = SqlStatementModelObjectMother.CreateSqlEntityDefinitionExpression (typeof (Cook));

      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (string), "Cook", "c");
      var primaryColumn = new SqlColumnDefinitionExpression (typeof (int), "k", "ID", false);
      var foreignColumn = new SqlColumnDefinitionExpression (typeof (int), "c", "KitchenID", false);
      var expectedResolvedJoinInfo = new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);

      _stageMock
          .Expect (mock => mock.ResolveCollectionSourceExpression (unresolvedCollectionJoinInfo.SourceExpression, _mappingResolutionContext))
          .Return (sqlEntityExpression);
      _stageMock.Replay();

      _resolverMock
          .Expect (
              mock =>
              mock.ResolveJoinInfo (
                  Arg<UnresolvedJoinInfo>.Matches (a => a.MemberInfo == memberInfo && a.OriginatingEntity.Type == typeof(Cook)),
                  Arg.Is (_generator)))
          .Return (expectedResolvedJoinInfo);
      _resolverMock.Replay();

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (foreignTableInfo, _mappingResolutionContext))
          .Return (foreignTableInfo);
      _stageMock.Replay ();

      var resolvedJoinInfo = ResolvingJoinInfoVisitor.ResolveJoinInfo (unresolvedCollectionJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);

      Assert.That (resolvedJoinInfo, Is.SameAs (expectedResolvedJoinInfo));

      _stageMock.VerifyAllExpectations();
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void ResolveJoinInfo_ResolvesCollectionJoinInfo_UnaryExpression ()
    {
      var memberInfo = typeof (Cook).GetProperty ("IllnessDays");
      var unresolvedCollectionJoinInfo = new UnresolvedCollectionJoinInfo (Expression.Constant (new Cook ()), memberInfo);

      var sqlEntityExpression = SqlStatementModelObjectMother.CreateSqlEntityDefinitionExpression (typeof (Cook));
      var fakeUnaryExpression = Expression.Not(Expression.Constant(1));

      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (string), "Cook", "c");
      var primaryColumn = new SqlColumnDefinitionExpression (typeof (int), "k", "ID", false);
      var foreignColumn = new SqlColumnDefinitionExpression (typeof (int), "c", "KitchenID", false);
      var expectedResolvedJoinInfo = new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);

      _stageMock
          .Expect (mock => mock.ResolveCollectionSourceExpression (unresolvedCollectionJoinInfo.SourceExpression, _mappingResolutionContext))
          .Return (fakeUnaryExpression);
      _stageMock
          .Expect (mock => mock.ResolveCollectionSourceExpression (fakeUnaryExpression.Operand, _mappingResolutionContext))
          .Return (sqlEntityExpression);
      _stageMock.Replay ();

      _resolverMock
          .Expect (
              mock =>
              mock.ResolveJoinInfo (
                  Arg<UnresolvedJoinInfo>.Matches (a => a.MemberInfo == memberInfo && a.OriginatingEntity.Type == typeof (Cook)),
                  Arg.Is (_generator)))
          .Return (expectedResolvedJoinInfo);
      _resolverMock.Replay ();

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (foreignTableInfo, _mappingResolutionContext))
          .Return (foreignTableInfo);
      _stageMock.Replay ();

      var resolvedJoinInfo = ResolvingJoinInfoVisitor.ResolveJoinInfo (unresolvedCollectionJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);

      Assert.That (resolvedJoinInfo, Is.SameAs (expectedResolvedJoinInfo));

      _stageMock.VerifyAllExpectations ();
      _resolverMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Only entities can be used as the collection source in from expressions, '1' cannot. Member: 'Int32[] IllnessDays'")]
    public void ResolveJoinInfo_ResolvesCollectionJoinInfo_NoEntity ()
    {
      var memberInfo = typeof (Cook).GetProperty ("IllnessDays");
      var unresolvedCollectionJoinInfo = new UnresolvedCollectionJoinInfo (Expression.Constant (new Cook ()), memberInfo);
      var fakeExpression = Expression.Constant (1);

      var foreignTableInfo = new ResolvedSimpleTableInfo (typeof (string), "Cook", "c");
      var primaryColumn = new SqlColumnDefinitionExpression (typeof (int), "k", "ID", false);
      var foreignColumn = new SqlColumnDefinitionExpression (typeof (int), "c", "KitchenID", false);
      var expectedResolvedJoinInfo = new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);

      _stageMock
          .Expect (mock => mock.ResolveCollectionSourceExpression (unresolvedCollectionJoinInfo.SourceExpression, _mappingResolutionContext))
          .Return (fakeExpression);
      _stageMock.Replay ();

      _resolverMock
          .Expect (
              mock =>
              mock.ResolveJoinInfo (
                  Arg<UnresolvedJoinInfo>.Matches (a => a.MemberInfo == memberInfo && a.OriginatingEntity.Type == typeof (Cook)),
                  Arg.Is (_generator)))
          .Return (expectedResolvedJoinInfo);
      _resolverMock.Replay ();

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (foreignTableInfo, _mappingResolutionContext))
          .Return (foreignTableInfo);
      _stageMock.Replay ();

      ResolvingJoinInfoVisitor.ResolveJoinInfo (unresolvedCollectionJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);
    }


    [Test]
    public void ResolveJoinInfo_ResolvedLeftJoinInfo ()
    {
      var tableInfo = new ResolvedSimpleTableInfo (typeof (Cook), "CookTable", "c");
      var leftJoinInfo = new ResolvedJoinInfo (tableInfo, new SqlLiteralExpression (1), new SqlLiteralExpression (1));

      _stageMock
          .Expect (mock => mock.ResolveTableInfo (tableInfo, _mappingResolutionContext))
          .Return (tableInfo);
      _stageMock.Replay();

      var resolvedJoinInfo = ResolvingJoinInfoVisitor.ResolveJoinInfo (leftJoinInfo, _resolverMock, _generator, _stageMock, _mappingResolutionContext);

      _stageMock.VerifyAllExpectations();
      Assert.That (resolvedJoinInfo.ForeignTableInfo, Is.SameAs (tableInfo));
    }
  }
}