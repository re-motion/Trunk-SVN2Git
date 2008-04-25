namespace Remotion.Mixins.Utilities.Singleton
{
  public class DefaultInstanceCreator<T> : IInstanceCreator<T> where T : new()
  {
    public T CreateInstance()
    {
      return new T();
    }
  }
}