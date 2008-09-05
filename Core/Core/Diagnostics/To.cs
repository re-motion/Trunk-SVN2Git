using System;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Remotion.Diagnostics.ToText;

namespace Remotion.Diagnostics
{
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider _toTextProvider = new ToTextProvider (GetTypeHandlers(), GetInterfaceHandlers());
   
    private static readonly ToTextBuilder _toTextBuilderConsole = new ToTextBuilder (_toTextProvider, System.Console.Out);
    private static readonly ToTextBuilder _toTextBuilderError = new ToTextBuilder (_toTextProvider, System.Console.Error);
    //private static readonly ToTextBuilder _toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter(Path.GetTempFileName()));
    private static readonly ToTextBuilder _toTextBuilderLog = new ToTextBuilder (_toTextProvider, new StreamWriter (System.IO.Path.GetTempPath() + "\\remotion.log"));

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceHandlerMap;

    public static ToTextProvider ToTextProvider { get { return _toTextProvider; } }


    public static ToTextBuilder Console 
    {
      get
      {
        return _toTextBuilderConsole;
      }
    }

    public static ToTextBuilder Error 
    {
      get {
        return _toTextBuilderError;
      }
    }

    public static ToTextBuilder TempLog
    {
      get
      {
        return _toTextBuilderLog;
      }
    }

    public static string Text (object obj)
    {
      return _toTextProvider.ToTextString (obj);
    }

    public static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> GetTypeHandlers()
    {
      if (_typeHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
        //_typeHandlerMap = handlerCollector.CollectHandlers<IToTextSpecificTypeHandler> ();
        _typeHandlerMap = handlerCollector.CollectTypeHandlers ();
      }
      return _typeHandlerMap;
    }

    public static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> GetInterfaceHandlers ()
    {
      if (_interfaceHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
       // _interfaceHandlerMap = handlerCollector.CollectHandlers<IToTextSpecificInterfaceHandler> ();
        _interfaceHandlerMap = handlerCollector.CollectInterfaceHandlers();
      }
      return _interfaceHandlerMap;
    }

    public static void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _toTextProvider.RegisterSpecificTypeHandler<T> (handler);
    }

    public static void ClearHandlers ()
    {
      _toTextProvider.ClearSpecificTypeHandlers();
    }


    public static void TextEnableAutomatics (bool enable)
    {
      var settings = _toTextProvider.Settings;
      settings.UseAutomaticObjectToText = enable;
      settings.UseAutomaticStringEnclosing = enable;
      settings.UseAutomaticCharEnclosing = enable;
      settings.UseInterfaceHandlers = enable;
    }
  }
}