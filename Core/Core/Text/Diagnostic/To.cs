using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Text.Diagnostic
{
  public class To
  {
    public static string Text (object o)
    {
      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
      // *) Implements IToText
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      // Functionality:
      // * Register handlers for interfaces, which can be called by ToText handlers of specific types.

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
      else if (type.IsArray)
      {
        Array array = (Array) o;
        return ArrayToText (array);
      }
      else if (type.GetInterface ("IEnumerable") != null) 
      {
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



    private class ArrayToTextProcessor : OuterProduct.ProcessorBase
    {
      protected readonly Array _array;
      public readonly StringBuilder _result = new StringBuilder ();

      public ArrayToTextProcessor (Array rectangularArray) //, StringBuilder stringBuilder)
      {
        _array = rectangularArray;
        //_result = stringBuilder;
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          _result.Append (To.Text(_array.GetValue (ProcessingState.DimensionIndices)));
        }
        else
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          _result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
        {
          _result.Append ("}");
        }
        return true;
      }

      public String GetResult ()
      {
        return "{" + _result.ToString() + "}";
      }
    }

    public static string ArrayToText (Array array)
    {
      var outerProduct = new OuterProduct (array);
      var processor = new ArrayToTextProcessor (array);
      outerProduct.ProcessOuterProduct (processor);
      return processor.GetResult ();
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