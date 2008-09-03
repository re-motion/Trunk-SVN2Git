using System;
using Remotion.Diagnostics.ToText;

namespace Remotion.Diagnostics
{
  public static class To
  {
    [ThreadStatic]
    private static readonly ToTextProvider toTextProvider = new ToTextProvider();

    static To ()
    {
      AutoRegisterHandlers();
    }

    public static string Text (object obj)
    {
      return toTextProvider.ToTextString (obj);
    }


    public static void AutoRegisterHandlers ()
    {
      // TODO: This will be called for each new thread calling any method of the To class => synchronize, cache and share
      var autoRegistrator = new ToTextSpecificHandlerAutoRegistrator ();
      autoRegistrator.FindAndRegister (toTextProvider);
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