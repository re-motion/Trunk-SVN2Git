using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Text.Diagnostic
{
  public class ToTextProvider
  {

    //public string ToText (object o, ToTextBuilder toTextCollector)
    //{
    //  // Handle Cascade:
    //  // *) Is null
    //  // *) Type handler registered
    //  // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
    //  // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
    //  // *) Implements IToTextHandler
    //  // *) If !IsInterface: Base type handler registered (recursive)
    //  // *) Implements IEnumerable ("is container")
    //  // *) If enabled: Log properties through reflection
    //  // *) ToString()

    //  // Functionality:
    //  // * Register handlers for interfaces, which can be called by ToText handlers of specific types.

    //  if (o == null)
    //  {
    //    Log ("null");
    //    return "null";
    //  }

    //  Delegate handler = null;
    //  Type type = o.GetType ();

    //  Log (type.ToString ());

    //  _typeHandlerMap.TryGetValue (type, out handler);

    //  if (handler != null)
    //  {
    //    return (String) handler.DynamicInvoke (o);
    //  }
    //  else if (type == typeof (string))
    //  {
    //    return (string) o;
    //  }
    //  else if (type.IsArray)
    //  {
    //    Array array = (Array) o;
    //    return ArrayToText (array);
    //  }
    //  else if (type.GetInterface ("IEnumerable") != null)
    //  {
    //    var collection = (IEnumerable) o;
    //    return CollectionToText (collection);
    //  }
    //  else
    //  {
    //    return o.ToString ();
    //  }
    //}


    //public string ToText (object o)
    //{
    //}

    public void ToText (object o, ToTextBuilder toTextBuilder)
    {
      Assertion.IsNotNull (toTextBuilder);

      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
      // *) Implements IToTextHandler
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      // Functionality:
      // * Register handlers for interfaces, which can be called by ToText handlers of specific types.

      if (o == null)
      {
        Log ("null");
        toTextBuilder.AppendString ("null");
        return;
      }

      Delegate handler = null;
      Type type = o.GetType ();

      Log (type.ToString ());

      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        handler.DynamicInvoke (o, toTextBuilder);
      }
      else if (type == typeof (string))
      {
        toTextBuilder.AppendString ((string) o);
      }
      else if (type.IsArray)
      {
        ArrayToText ((Array) o, toTextBuilder);
      }
      else if (type.GetInterface ("IEnumerable") != null)
      {
        CollectionToText ((IEnumerable) o, toTextBuilder);
      }
      else
      {
        toTextBuilder.AppendString (o.ToString ());
      }
    }



    private Dictionary<Type, Delegate> _typeHandlerMap = new Dictionary<Type, Delegate> ();


    public void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T), handler);
    }

    public void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }


    public void RegisterStringHandlers ()
    {
      RegisterHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      RegisterHandler<char> ((x, ttb) => ttb.s ("'").ts (x).s ("'"));
    }


    public void CollectionToText (IEnumerable collection, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendEnumerable(collection);
    }

    //public void CollectionToText (IEnumerable collection, ToTextBuilder toTextBuilder)
    //{
    //  const string start = "{";
    //  const string seperator = ",";
    //  const string end = "}";
    //  //var sb = new StringBuilder ();

    //  toTextBuilder.Append (start);
    //  bool insertSeperator = false; // no seperator before first element
    //  foreach (Object element in collection)
    //  {
    //    if (insertSeperator)
    //    {
    //      toTextBuilder.Append (seperator);
    //    }
    //    else
    //    {
    //      insertSeperator = true;
    //    }

    //    toTextBuilder.ToText (element);
    //  }
    //  toTextBuilder.Append (end);
    //}



    //private class ArrayToTextProcessor : OuterProduct.ProcessorBase
    //{
    //  protected readonly Array _array;
    //  private ToTextBuilder _toTextColllector;
    //  //public readonly StringBuilder _result = new StringBuilder ();

    //  public ArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
    //  {
    //    _array = rectangularArray;
    //    _toTextColllector = toTextBuilder;
    //  }

    //  public override bool DoBeforeLoop ()
    //  {
    //    if (ProcessingState.IsInnermostLoop)
    //    {
    //      _toTextColllector.s (ProcessingState.IsFirstLoopElement ? "" : ",");
    //      _toTextColllector.ToText  (_array.GetValue (ProcessingState.DimensionIndices));
    //    }
    //    else
    //    {
    //      _toTextColllector.s (ProcessingState.IsFirstLoopElement ? "" : ",");
    //      _toTextColllector.s ("{");
    //    }
    //    return true;
    //  }

    //  public override bool DoAfterLoop ()
    //  {
    //    if (!ProcessingState.IsInnermostLoop)
    //    {
    //      _toTextColllector.s ("}");
    //    }
    //    return true;
    //  }

    //  //public String GetResult ()
    //  //{
    //  //  return "{" + _toTextColllector.ToString () + "}";
    //  //}
    //}

    //public void ArrayToText (Array array, ToTextBuilder toTextBuilder)
    //{
    //  var outerProduct = new OuterProduct (array);
    //  toTextBuilder.AppendString("{");
    //  var processor = new ArrayToTextProcessor (array, toTextBuilder);
    //  outerProduct.ProcessOuterProduct (processor);
    //  toTextBuilder.AppendString ("}");
    //  //return processor.GetResult ();
    //}

    public void ArrayToText (Array array, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendArray(array);
    }



    private static void Log (string s)
    {
      Console.WriteLine ("[To]: " + s);
    }

    private static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }
  }
}