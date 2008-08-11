using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
        ListListString = new List<List<string>>();
      }

      //public void AddMember (int i, Object o)
      //{
      //  if (ListListString.Count < i)
      //  {
      //    ListListString
      //  }
      //}

      public string Name { get; set; } 
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
    }



    public static string Text (object o)
    {
      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Implements IToText
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      if (o == null)
      {
        Log ("null");
        return "null";
      }

      Delegate handler = null;
      Type type = o.GetType();

      Log (type.ToString());

      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        return (String) handler.DynamicInvoke (o);
      }
      else if (type == typeof (string))
      {
        return (string) o;
      }
      else if (type.GetInterface ("IEnumerable") != null) 
      {
        //return o.ToString();
        IEnumerable collection = (IEnumerable) o;
        return CollectionToText (collection);
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

    public static void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }


    public static void RegisterStringHandlers ()
    {
      RegisterHandler<String> (x => "\"" + x + "\"");
      RegisterHandler<char> (x => "'" + x + "'");
    }


    public static string CollectionToText (IEnumerable collection)
    {
      const string start = "{";
      const string seperator = ",";
      const string end = "}";
      var sb = new StringBuilder ();

      sb.Append (start);
      bool insertSeperator = false; // no seperator before first element
      foreach (Object element in collection)
      {
        if (insertSeperator)
        {
          sb.Append (seperator);
        }
        else
        {
          insertSeperator = true;
        }

        sb.Append (To.Text (element));

        //Log (element.ToString());
      }
      sb.Append (end);
      return sb.ToString ();
    }



    public static void Log (string s)
    {
      Console.WriteLine ("[To]: " + s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }


  }
}