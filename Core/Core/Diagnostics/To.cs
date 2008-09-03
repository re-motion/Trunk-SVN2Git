using System;
using System.Runtime.Remoting.Contexts;
using Remotion.Diagnostics.ToText;

namespace Remotion.Diagnostics
{
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider _toTextProvider = new ToTextProvider (GetTypeHandlers(), GetInterfaceHandlers());

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private static ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceHandlerMap;

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
      _toTextProvider.Settings.UseAutomaticObjectToText = enable;
      _toTextProvider.Settings.UseAutomaticStringEnclosing = enable;
      _toTextProvider.Settings.UseAutomaticCharEnclosing = enable;
    }
  }
}