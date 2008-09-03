using System;
using System.Runtime.Remoting.Contexts;
using Remotion.Diagnostics.ToText;

namespace Remotion.Diagnostics
{
  [Synchronization]
  public static class To
  {
    [ThreadStatic]
    private static ToTextProvider toTextProvider = new ToTextProvider(GetTypeHandlers());

    private static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;

    public static string Text (object obj)
    {
      return toTextProvider.ToTextString (obj);
    }


    public static ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> GetTypeHandlers()
    {
      if (_typeHandlerMap == null)
      {
        var handlerCollector = new ToTextSpecificHandlerCollector ();
        _typeHandlerMap = handlerCollector.CollectHandlers<IToTextSpecificTypeHandler> ();
      }
      return _typeHandlerMap;
    }

    public static void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      toTextProvider.RegisterSpecificTypeHandler<T> (handler);
    }

    public static void ClearHandlers ()
    {
      toTextProvider.ClearSpecificTypeHandlers();
    }



    public static void TextEnableAutomatics (bool enable)
    {
      toTextProvider.Settings.UseAutomaticObjectToText = enable;
      toTextProvider.Settings.UseAutomaticStringEnclosing = enable;
      toTextProvider.Settings.UseAutomaticCharEnclosing = enable;
    }
  }
}