namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface ITargetClassWithAdditionalDependencies
  {
    string GetString ();
  }

  public class TargetClassWithAdditionalDependencies : ITargetClassWithAdditionalDependencies
  {
    public virtual string GetString ()
    {
      return "TargetClassWithAdditionalDependencies";
    }
  }
}