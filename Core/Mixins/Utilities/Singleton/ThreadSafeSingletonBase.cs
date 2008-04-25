using System;

namespace Remotion.Mixins.Utilities.Singleton
{
  public class ThreadSafeSingletonBase<TSelf, TCreator>
      where TSelf : class
      where TCreator : IInstanceCreator<TSelf>, new()
  {
    private static ThreadSafeSingleton<TSelf> s_instance = new ThreadSafeSingleton<TSelf> (new TCreator().CreateInstance);

    public static TSelf Current
    {
      get { return s_instance.Current; }
    }

    public static bool HasCurrent
    {
      get { return s_instance.HasCurrent; }
    }

    public static void SetCurrent(TSelf value)
    {
      s_instance.SetCurrent (value);
    }
  }
}
