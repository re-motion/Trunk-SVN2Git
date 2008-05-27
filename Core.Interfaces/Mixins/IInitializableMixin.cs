namespace Remotion.Mixins
{
  public interface IInitializableMixin
  {
    void Initialize (object @this, object @base, bool deserialization);
  }
}