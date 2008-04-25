using System;

namespace Remotion.Mixins.Utilities.Singleton
{
  public class CallContextSingletonBase<TSelf, TCreator> where TCreator : IInstanceCreator<TSelf>, new()
  {
    private static CallContextSingleton<TSelf> s_instance = new CallContextSingleton<TSelf> (typeof (TSelf).FullName + "_Singleton",
      new TCreator().CreateInstance);

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
