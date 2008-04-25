using System;
using Remotion.Utilities;
using System.IO;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public class AppDomainRunner : AppDomainRunnerBase
  {
    public static void Run (Proc<object[]> action, params object[] args)
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.DynamicBase = Path.GetTempPath();
      AppDomainRunner runner = new AppDomainRunner (setup, action, args);
      runner.Run ();
    }

    private Proc<object[]> _action;
    private object[] _args;

    public AppDomainRunner (AppDomainSetup domainSetup, Proc<object[]> action, object[] args)
      : base (domainSetup)
    {
      _action = action;
      _args = args;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      _action (_args);
    }
  }
}
