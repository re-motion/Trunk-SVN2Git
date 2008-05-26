namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public interface IInitializableMixinTarget : IMixinTarget
  {
    object CreateBaseCallProxy (int depth);
    void SetFirstBaseCallProxy (object baseCallProxy);
  }
}