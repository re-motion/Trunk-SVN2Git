using System;

namespace Remotion.Utilities
{
  /// <summary>
  /// Base class for executing code in a separate <see cref="AppDomain"/>.
  /// </summary>
  [Serializable]
  public abstract class AppDomainRunnerBase
  {
    private readonly AppDomainSetup _appDomainSetup;

    protected AppDomainRunnerBase (AppDomainSetup appDomainSetup)
    {
      ArgumentUtility.CheckNotNull ("appDomainSetup", appDomainSetup);

      _appDomainSetup = appDomainSetup;
    }

    protected abstract void CrossAppDomainCallbackHandler ();

    public AppDomainSetup AppDomainSetup
    {
      get { return _appDomainSetup; }
    }

    public void Run ()
    {
      AppDomain appDomain = null;
      AppDomainAssemblyResolver appDomainAssemblyResolver = null;
      try
      {
        appDomain = AppDomain.CreateDomain (_appDomainSetup.ApplicationName, AppDomain.CurrentDomain.Evidence, _appDomainSetup);
        appDomainAssemblyResolver = new AppDomainAssemblyResolver (AppDomain.CurrentDomain.SetupInformation, appDomain);
        appDomain.DoCallBack (CrossAppDomainCallbackHandler);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);

        if (appDomainAssemblyResolver != null)
          appDomainAssemblyResolver.Dispose();
      }
    }
  }
}