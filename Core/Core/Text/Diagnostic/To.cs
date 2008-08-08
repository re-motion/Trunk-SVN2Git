using System;
using System.Collections.Generic;

namespace Remotion.Text.Diagnostic
{
  public class To
  {
    public class Test 
    {
      public Test ()
      {
        Name = "DefaultName";
        Int = 1234567;
      }

      public Test (string name, int i0)
      {
        Name = name;
        Int = i0;
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public List<List<string>> ListListString { get; private set; }
    }



    public static string Text (object o)
    {
      Delegate handler = null;
      _typeHandlerMap.TryGetValue (o.GetType(), out handler);
      if (handler != null)
      {
        return (String) handler.DynamicInvoke (o);
      }
      else
      {
        return o.ToString();
      }
    }

    private static Dictionary<Type, Delegate> _typeHandlerMap = new Dictionary<Type, Delegate>();


    public static void RegisterHandler<T> (Func<T,string> handler)
    {
      _typeHandlerMap.Add (typeof (T), handler);
    }

 


  }
}