using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.Linq.Visitor;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.Linq.VisitorTest.ExpressionTreeVisitorTest
{
  public class ExpressionTreeVisitor_SpecificExpressionsTestBase
  {
    protected MockRepository _mockRepository;
    private ExpressionTreeVisitor _visitorMock;

    [SetUp]
    public void Setup()
    {
      _mockRepository = new MockRepository();
      _visitorMock = _mockRepository.CreateMock<ExpressionTreeVisitor>();
    }

    protected Expression InvokeAndCheckVisitExpression (string methodName, Expression expression)
    {
      return (Expression) InvokeAndCheckVisitObject (methodName, expression);
    }

    protected object InvokeAndCheckVisitObject (string methodName, object argument)
    {
      return InvokeAndCheckVisitMethod<object, object> (delegate { return InvokeVisitMethod (methodName, argument); }, argument);
    }

    protected ReadOnlyCollection<T> InvokeAndCheckVisitExpressionList<T> (ReadOnlyCollection<T> expressions) where T : Expression
    {
      return InvokeAndCheckVisitMethod<ReadOnlyCollection<T>, ReadOnlyCollection<T>> (
          delegate { return InvokeVisitExpressionListMethod (expressions); }, expressions);
    }

    protected ReadOnlyCollection<MemberBinding> InvokeAndCheckVisitMemberBindingList (ReadOnlyCollection<MemberBinding> expressions)
    {
      return InvokeAndCheckVisitMethod<ReadOnlyCollection<MemberBinding>, ReadOnlyCollection<MemberBinding>> (
          delegate { return InvokeVisitMethod ("VisitMemberBindingList", expressions); }, expressions);
    }

    protected ReadOnlyCollection<ElementInit> InvokeAndCheckVisitElementInitList (ReadOnlyCollection<ElementInit> expressions)
    {
      return InvokeAndCheckVisitMethod<ReadOnlyCollection<ElementInit>, ReadOnlyCollection<ElementInit>> (
          delegate { return InvokeVisitMethod ("VisitElementInitList", expressions); }, expressions);
    }

    private R InvokeAndCheckVisitMethod<A, R> (Func<A, R> visitMethod, A argument)
    {
      Expect.Call (visitMethod (argument)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      _mockRepository.ReplayAll ();

      R result = visitMethod (argument);
      _mockRepository.VerifyAll ();

      return result;
    }

    protected T InvokeVisitMethod<T> (string methodName, T argument)
    {
      return (T) _visitorMock.GetType ().GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke (_visitorMock, new object[] { argument });
    }

    protected ReadOnlyCollection<T> InvokeVisitExpressionListMethod<T> (ReadOnlyCollection<T> expressions) where T : Expression
    {
      return (ReadOnlyCollection<T>) _visitorMock.GetType ().GetMethod ("VisitExpressionList", BindingFlags.NonPublic | BindingFlags.Instance)
                                         .MakeGenericMethod (typeof (T))
                                         .Invoke (_visitorMock, new object[] { expressions });
    }
  }
}