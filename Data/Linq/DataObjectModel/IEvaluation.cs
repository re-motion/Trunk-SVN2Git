namespace Remotion.Data.Linq.DataObjectModel
{
  public interface IEvaluation
  {
    void Accept (IEvaluationVisitor visitor);
  }
}