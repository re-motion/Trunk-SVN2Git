using System;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.Configuration;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   Stores the session state for a single function token.
  /// </summary>
  [Serializable]
  public class WxeFunctionState
  {
    private static ILog s_log = LogManager.GetLogger (typeof (WxeFunctionState));

    private WxeFunction _function;
    private int _lifetime;
    private string _functionToken;
    private bool _isAborted;
    private bool _isCleanUpEnabled;
    private int _postBackID;

    public WxeFunctionState (WxeFunction function, bool enableCleanUp)
        : this (
            function,
            WebConfiguration.Current.ExecutionEngine.FunctionTimeout,
            enableCleanUp)
    {
    }

    public WxeFunctionState (
        WxeFunction function, int lifetime, bool enableCleanUp)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      _lifetime = lifetime;
      _functionToken = Guid.NewGuid().ToString();
      _function = function;
      _function.SetFunctionToken (_functionToken);
      _isCleanUpEnabled = enableCleanUp;
      _postBackID = 0;
      s_log.Debug (string.Format ("Created WxeFunctionState {0}.", _functionToken));
    }

    public WxeFunction Function
    {
      get { return _function; }
    }

    public int Lifetime
    {
      get { return _lifetime; }
    }

    public string FunctionToken
    {
      get { return _functionToken; }
    }

    /// <summary> 
    ///   Gets a flag that determines whether to automatically clean-up (i.e. abort) the function state after 
    ///   its function has executed.
    /// </summary>
    public bool IsCleanUpEnabled
    {
      get { return _isCleanUpEnabled; }
    }

    protected internal int PostBackID
    {
      get { return _postBackID; }
      set { _postBackID = value; }
    }

    public bool IsAborted
    {
      get { return _isAborted; }
    }

    /// <summary> Aborts the <b>WxeFunctionState</b> by calling <see cref="AbortRecursive"/>. </summary>
    /// <remarks> 
    ///   Use the <see cref="WxeFunctionStateManager.Abort">WxeFunctionStateCollection.Abort</see> method to abort
    ///   a <b>WxeFunctionState</b>.
    /// </remarks>
    protected internal void Abort ()
    {
      if (! _isAborted)
      {
        s_log.Debug (string.Format ("Aborting WxeFunctionState {0}.", _functionToken));
        AbortRecursive();
        _isAborted = true;
      }
    }

    /// <summary> Aborts the <b>WxeFunctionState</b>. </summary>
    protected virtual void AbortRecursive ()
    {
      if (_function != null)
        _function.Abort();
    }
  }
}