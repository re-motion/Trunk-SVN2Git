using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.Linq.UnitTests.ClausesTest
{
  [TestFixture]
  public class JoinClauseTest
  {
    [Test]
    public void Intialize_WithIDAndInExprAndOnExprAndEqExpr()
    {
      ParameterExpression identifier = ExpressionHelper.CreateParameterExpression ();
      Expression inExpression = ExpressionHelper.CreateExpression ();
      Expression onExpression = ExpressionHelper.CreateExpression ();
      Expression equalityExpression = ExpressionHelper.CreateExpression ();

      IClause clause = ExpressionHelper.CreateClause();

      JoinClause joinClause = new JoinClause (clause, identifier, inExpression, onExpression, equalityExpression);

      Assert.AreSame (clause, joinClause.PreviousClause);
      Assert.AreSame (identifier, joinClause.Identifier);
      Assert.AreSame (inExpression, joinClause.InExpression);
      Assert.AreSame (onExpression, joinClause.OnExpression);
      Assert.AreSame (equalityExpression, joinClause.EqualityExpression);
      
      Assert.IsNull (joinClause.IntoIdentifier);
    }

    [Test]
    public void Intialize_WithIDAndInExprAndOnExprAndEqExprAndIntoID ()
    {
      ParameterExpression identifier = ExpressionHelper.CreateParameterExpression ();
      Expression inExpression = ExpressionHelper.CreateExpression ();
      Expression onExpression = ExpressionHelper.CreateExpression ();
      Expression equalityExpression = ExpressionHelper.CreateExpression ();
      ParameterExpression intoIdentifier = ExpressionHelper.CreateParameterExpression ();

      IClause clause = ExpressionHelper.CreateClause ();
      JoinClause joinClause = new JoinClause (clause,identifier, inExpression, onExpression, equalityExpression, intoIdentifier);

      Assert.AreSame (clause, joinClause.PreviousClause);
      Assert.AreSame (identifier, joinClause.Identifier);
      Assert.AreSame (inExpression, joinClause.InExpression);
      Assert.AreSame (onExpression, joinClause.OnExpression);
      Assert.AreSame (equalityExpression, joinClause.EqualityExpression);
      Assert.AreSame (equalityExpression, joinClause.EqualityExpression);
      Assert.AreSame (intoIdentifier, joinClause.IntoIdentifier);

    }

    [Test]
    public void JoinClause_ImplementsIQueryElement()
    {
      JoinClause joinClause = ExpressionHelper.CreateJoinClause();
      Assert.IsInstanceOfType (typeof (IQueryElement), joinClause);
    }

    [Test]
    public void Accept ()
    {
      JoinClause joinClause = ExpressionHelper.CreateJoinClause ();

      MockRepository repository = new MockRepository ();
      IQueryVisitor visitorMock = repository.CreateMock<IQueryVisitor> ();

      visitorMock.VisitJoinClause (joinClause);

      repository.ReplayAll ();

      joinClause.Accept (visitorMock);

      repository.VerifyAll ();

    }
  }
}