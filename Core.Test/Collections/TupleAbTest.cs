using System;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  using TestTuple = Remotion.Collections.Tuple<int, string>;

  [TestFixture]
  public class TupelAbTest
  {
    [Test]
    public void Initialize ()
    {
      TestTuple tuple = new TestTuple (1, "X");

      Assert.AreEqual (1, tuple.A);
      Assert.AreEqual ("X", tuple.B);
    }

    [Test]
    public void EasyInitialize ()
    {
      TestTuple tuple = Tuple.NewTuple (1, "X");
      Assert.AreEqual (1, tuple.A);
      Assert.AreEqual ("X", tuple.B);
    }

    [Test]
    public void Equals_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X");

      Assert.IsFalse (left.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      TestTuple tuple = new TestTuple (1, "X");

      Assert.IsTrue (tuple.Equals (tuple));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X");
      TestTuple right = new TestTuple (1, "X");

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithEqualNulls ()
    {
      Tuple<int?, string> left = new Tuple<int?, string> (null, null);
      Tuple<int?, string> right = new Tuple<int?, string> (null, null);

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentA ()
    {
      TestTuple left = new TestTuple (1, "X");
      TestTuple right = new TestTuple (-1, "X");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentB ()
    {
      TestTuple left = new TestTuple (1, "X");
      TestTuple right = new TestTuple (1, null);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      TestTuple left = new TestTuple (1, "X");
      TestTuple right = new TestTuple (1, "X");

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      TestTuple left = new TestTuple (1, "X");

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      TestTuple left = new TestTuple (1, "X");

      Assert.IsFalse (left.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      TestTuple left = new TestTuple (1, "X");
      TestTuple right = new TestTuple (1, "X");

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

    [Test]
    public void TestGetHashCodeNull ()
    {
      Tuple<int?, string> left = new Tuple<int?, string> (null, null);
      Tuple<int?, string> right = new Tuple<int?, string> (null, null);

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }
  }
}