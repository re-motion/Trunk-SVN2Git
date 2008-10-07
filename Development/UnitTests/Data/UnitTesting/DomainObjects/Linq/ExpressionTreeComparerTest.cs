using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.Linq
{
  [TestFixture]
  public class ExpressionTreeComparerTest
  {
    private ExpressionTreeComparer _expressionTreeComparer;

    [SetUp]
    public void SetUp ()
    {
      _expressionTreeComparer = new ExpressionTreeComparer ((actual, exptected) => Assert.That (actual, Is.EqualTo (exptected)));
    }

    [Test]
    public void Compare_Equal ()
    {
      IQueryable<SimpleDomainObject> expected = from d in QueryFactory.CreateQueryable<SimpleDomainObject>() where d.Value == 1 select d;
      IQueryable<SimpleDomainObject> actual = from d in QueryFactory.CreateQueryable<SimpleDomainObject>() where d.Value == 1 select d;

      _expressionTreeComparer.Compare (expected, actual);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void Compare_NotEqual ()
    {
      IQueryable<SimpleDomainObject> expected = from d in QueryFactory.CreateQueryable<SimpleDomainObject>() where d.Value == 1 select d;
      IQueryable<SimpleDomainObject> actual = from d in QueryFactory.CreateQueryable<SimpleDomainObject>() where d.Value == 0 select d;

      _expressionTreeComparer.Compare (expected, actual);
    }
  }
}