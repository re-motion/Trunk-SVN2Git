using System;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  using TestTuple = Remotion.Collections.Tuple<int, string, double, DateTime>;

  [TestFixture]
  public class TupelAbcdTest
  {
    private readonly DateTime date1 = new DateTime (2006, 7, 17, 11, 15, 10);
    private readonly DateTime date2 = new DateTime (2005, 7, 17, 11, 15, 10);

    [Test]
    public void Initialize ()
    {
      TestTuple tuple = new TestTuple (1, "X", 2.5, date1);

      Assert.AreEqual (1, tuple.A);
      Assert.AreEqual ("X", tuple.B);
    }

    [Test]
    public void EasyInitialize ()
    {
      TestTuple tuple = Tuple.NewTuple (1, "X", 2.5, date1);
      Assert.AreEqual (1, tuple.A);
      Assert.AreEqual ("X", tuple.B);
      Assert.AreEqual (2.5, tuple.C);
      Assert.AreEqual (date1, tuple.D);
    }

    [Test]
    public void Equals_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5,date1);

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      TestTuple tuple = new TestTuple (1, "X", 2.5, date1);

      Assert.IsTrue (tuple.Equals (tuple));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (-1, "X", 2.5, date1);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "A", 2.5, date1);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentC ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", -2.5, date1);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentD ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date2);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      TestTuple left = new TestTuple (1, "X", 2.5, date1);
      TestTuple right = new TestTuple (1, "X", 2.5, date1);

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}