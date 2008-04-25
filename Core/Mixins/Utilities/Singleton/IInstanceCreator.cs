namespace Remotion.Mixins.Utilities.Singleton
{
  public interface IInstanceCreator<T>
  {
    T CreateInstance();
  }
}