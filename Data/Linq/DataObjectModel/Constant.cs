using Remotion.Utilities;

namespace Remotion.Data.Linq.DataObjectModel
{
  public struct Constant : IValue, ICriterion
  {
    public readonly object Value;

    public Constant (object value)
        : this()
    {
      Value = value;
    }

   
    public override string ToString ()
    {
      return Value != null ? Value.ToString() : "<null>";
    }

    public void Accept (IEvaluationVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitConstant (this);
    }
  }
}