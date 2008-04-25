using System;
using Remotion.Utilities;

namespace Remotion.Collections
{
  // TODO: Doc
  [Serializable]
  public class Tuple<TA, TB, TC> : IEquatable<Tuple<TA, TB, TC>>
  {
    private readonly TA _a;
    private readonly TB _b;
    private readonly TC _c;

    public Tuple (TA a, TB b, TC c)
    {
      _a = a;
      _b = b;
      _c = c;
    }

    public TA A
    {
      get { return _a; }
    }

    public TB B
    {
      get { return _b; }
    }

    public TC C
    {
      get { return _c; }
    }

    public bool Equals (Tuple<TA, TB, TC> other)
    {
      if (other == null)
        return false;

      return EqualityUtility.Equals (_a, other._a)
             && EqualityUtility.Equals (_b, other._b)
             && EqualityUtility.Equals (_c, other._c);
    }

    public override bool Equals (object obj)
    {
      Tuple<TA, TB, TC> other = obj as Tuple<TA, TB, TC>;
      if (other == null)
        return false;
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_a, _b, _c);
    }
  }
}