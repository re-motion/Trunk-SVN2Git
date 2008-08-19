using System;
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Remotion.Web.UnitTests
{
  public class PairConstraint : EqualConstraint
  {
    private readonly Pair _expected;
    private Pair _actual;

    public PairConstraint (Pair expected)
        : base(expected)
    {
      _expected = expected;
    }

    public override bool Matches (object actual)
    {
      _actual = actual as Pair;

      if (base.Matches (actual))
        return true;

      if (actual == null)
        return _expected == null;

      return object.Equals (_expected.First, ((Pair) actual).First) && object.Equals (_expected.Second, ((Pair) actual).Second);
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      if (_expected == null || _actual == null)
      {
        base.WriteMessageTo (writer);
      }
      else
      {
        writer.DisplayStringDifferences (
            string.Format ("{{ {0} , {1} }}", _expected.First, _expected.Second),
            string.Format ("{{ {0} , {1} }}", _actual.First, _actual.Second),
            -1,
            false);
      }
    }
  }
}