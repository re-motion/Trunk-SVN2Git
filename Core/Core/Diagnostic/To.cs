using System;

namespace Remotion.Diagnostic
{
  public static class To
  {
    private static readonly ToTextProvider toTextProvider = new ToTextProvider();

    public static string Text (object obj)
    {
      return toTextProvider.ToTextString (obj);
    }


    public static void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      toTextProvider.RegisterHandler<T> (handler);
    }

    public static void ClearHandlers ()
    {
      toTextProvider.ClearHandlers();
    }


    public static void TextEnableAutomatics (bool enable)
    {
      toTextProvider.UseAutomaticObjectToText = enable;
      toTextProvider.UseAutomaticStringEnclosing = enable;
      toTextProvider.UseAutomaticCharEnclosing = enable;
    }
  }
}