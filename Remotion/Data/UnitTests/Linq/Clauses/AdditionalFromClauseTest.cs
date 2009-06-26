// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.Expressions;
using Rhino.Mocks;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.UnitTests.Linq.Clauses
{
  [TestFixture]
  public class AdditionalFromClauseTest
  {
    private AdditionalFromClause _additionalFromClause;
    private CloneContext _cloneContext;

    [SetUp]
    public void SetUp ()
    {
      _additionalFromClause = ExpressionHelper.CreateAdditionalFromClause();
      _cloneContext = new CloneContext (new ClonedClauseMapping ());      
    }

    [Test]
    public void Initialize ()
    {
      var fromExpression = ExpressionHelper.CreateExpression ();
      var fromClause = new AdditionalFromClause ("s", typeof (Student), fromExpression);

      Assert.That (fromClause.ItemName, Is.EqualTo ("s"));
      Assert.That (fromClause.ItemType, Is.SameAs (typeof (Student)));
      Assert.That (fromClause.FromExpression, Is.SameAs (fromExpression));

      Assert.That (fromClause.JoinClauses, Is.Empty);
      Assert.That (fromClause.JoinClauses.Count, Is.EqualTo (0));
    }

    [Test]
    public void ImplementInterface_IFromLetWhereClause ()
    {
      Assert.That (_additionalFromClause, Is.InstanceOfType (typeof (IBodyClause)));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateMock<IQueryModelVisitor> ();
      _additionalFromClause.Accept (visitorMock);
      visitorMock.AssertWasCalled (mock => mock.VisitAdditionalFromClause (_additionalFromClause));
    }
    
    [Test]
    public void Clone ()
    {
      var clone = _additionalFromClause.Clone (_cloneContext);

      Assert.That (clone, Is.Not.Null);
      Assert.That (clone, Is.Not.SameAs (_additionalFromClause));
      Assert.That (clone.ItemName, Is.EqualTo (_additionalFromClause.ItemName));
      Assert.That (clone.ItemType, Is.SameAs (_additionalFromClause.ItemType));
      Assert.That (clone.FromExpression, Is.SameAs (_additionalFromClause.FromExpression));
    }

    [Test]
    public void Clone_AdjustsExpressions ()
    {
      var referencedClause = ExpressionHelper.CreateMainFromClause();
      var fromExpression = new QuerySourceReferenceExpression (referencedClause);
      var additionalFromClause = new AdditionalFromClause ("s",
          typeof (Student), 
          fromExpression);

      var newReferencedClause = ExpressionHelper.CreateMainFromClause ();
      _cloneContext.ClonedClauseMapping.AddMapping (referencedClause, newReferencedClause);

      var clone = additionalFromClause.Clone (_cloneContext);

      Assert.That (((QuerySourceReferenceExpression) clone.FromExpression).ReferencedClause, Is.SameAs (newReferencedClause));
    }

    [Test]
    public void Clone_ViaInterface_PassesMapping ()
    {
      var clone = ((IBodyClause) _additionalFromClause).Clone (_cloneContext);
      Assert.That (_cloneContext.ClonedClauseMapping.GetClause (_additionalFromClause), Is.SameAs (clone));
    }

    [Test]
    public void Clone_JoinClauses ()
    {
      var originalJoinClause1 = ExpressionHelper.CreateJoinClause ();
      _additionalFromClause.JoinClauses.Add (originalJoinClause1);

      var originalJoinClause2 = ExpressionHelper.CreateJoinClause ();
      _additionalFromClause.JoinClauses.Add (originalJoinClause2);

      var clone = _additionalFromClause.Clone (_cloneContext);
      Assert.That (clone.JoinClauses.Count, Is.EqualTo (2));

      Assert.That (clone.JoinClauses[0], Is.Not.SameAs (originalJoinClause1));
      Assert.That (clone.JoinClauses[0].EqualityExpression, Is.SameAs (originalJoinClause1.EqualityExpression));
      Assert.That (clone.JoinClauses[0].InExpression, Is.SameAs (originalJoinClause1.InExpression));

      Assert.That (clone.JoinClauses[1], Is.Not.SameAs (originalJoinClause2));
      Assert.That (clone.JoinClauses[1].EqualityExpression, Is.SameAs (originalJoinClause2.EqualityExpression));
      Assert.That (clone.JoinClauses[1].InExpression, Is.SameAs (originalJoinClause2.InExpression));
    }

    [Test]
    public void Clone_JoinClauses_PassesMapping ()
    {
      var oldFromClause = ExpressionHelper.CreateMainFromClause ();
      var originalJoinClause = new JoinClause (
          "x",
          typeof(Student),
          new QuerySourceReferenceExpression (oldFromClause),
          ExpressionHelper.CreateExpression (),
          ExpressionHelper.CreateExpression ());
      _additionalFromClause.JoinClauses.Add (originalJoinClause);

      var newFromClause = ExpressionHelper.CreateMainFromClause ();
      _cloneContext.ClonedClauseMapping.AddMapping (oldFromClause, newFromClause);

      var clone = _additionalFromClause.Clone (_cloneContext);
      Assert.That (((QuerySourceReferenceExpression) clone.JoinClauses[0].InExpression).ReferencedClause, Is.SameAs (newFromClause));
    }

    [Test]
    public void Clone_AddsClauseToMapping ()
    {
      var clone = _additionalFromClause.Clone (_cloneContext);
      Assert.That (_cloneContext.ClonedClauseMapping.GetClause (_additionalFromClause), Is.SameAs (clone));
    }
  }
}
