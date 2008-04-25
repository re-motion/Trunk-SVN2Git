using System;
using Remotion.Utilities;

namespace Remotion.Collections
{
  // TODO: Doc
   [Serializable]
  public class Tuple<TA, TB> : IEquatable<Tuple<TA, TB>>
  {
    private readonly TA _a;
    private readonly TB _b;

    public Tuple (TA a, TB b)
    {
      _a = a;
      _b = b;
    }

    public TA A
    {
      get { return _a; }
    }

    public TB B
    {
      get { return _b; }
    }

    public bool Equals (Tuple<TA, TB> other)
    {
      return EqualityUtility.NotNullAndSameType (this, other)
             && EqualityUtility.Equals (_a, other._a) 
             && EqualityUtility.Equals (_b, other._b);
    }

    public override bool Equals (object obj)
    {
      return EqualityUtility.EqualsEquatable (this, obj);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_a, _b);
    }
  }
}