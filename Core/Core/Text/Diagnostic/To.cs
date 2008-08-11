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
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
    }



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
      //else if (type.IsArray)
      //{
      //  Array array = (Array) o;
      //  return ArrayToText (array);
      //}
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


    //public static string ArrayToText (Array array)
    //{
    //  int rank = array.Rank;
    //  var arrayIndices = new int[rank];
    //  for (int iRank = 0; iRank < rank; ++iRank)
    //  {

    //  }
    //}

    //  if (_stateCombinationCount == 0)
    //    return null;

    //  PropertyStateTuple[,] result = new PropertyStateTuple[_stateCombinationCount, _statePropertyCount];
    //  int[] currentStatePropertyIndices = new int[_statePropertyCount];

    //  for (int iCurrentStateCombination = 0; iCurrentStateCombination < _stateCombinationCount; ++iCurrentStateCombination)
    //  {
    //    ArrayToText_CalculateCurrentCombination (result, iCurrentStateCombination, currentStatePropertyIndices);
    //    ArrayToText_PrepareNextCombination (currentStatePropertyIndices);
    //  }

    //  return result;
    //}

    //private void ArrayToText_PrepareNextCombination (int[] arrayIndices)
    //{
    //  for (int iStateProperty2 = 0; iStateProperty2 < _statePropertyCount; ++iStateProperty2)
    //  {
    //    ++statePropertyIndices[iStateProperty2];
    //    if (statePropertyIndices[iStateProperty2] < _stateDefinitions[iStateProperty2].Length)
    //      break;
    //    else
    //      statePropertyIndices[iStateProperty2] = 0;
    //  }
    //}

    //private void ArrayToText_CalculateCurrentCombination (PropertyStateTuple[,] result, int currentStateCombination, int[] arrayIndices)
    //{
    //  for (int iStateProperty = 0; iStateProperty < _statePropertyCount; ++iStateProperty)
    //  {
    //    var stateProperty = ClassDefinition.StateProperties[iStateProperty];
    //    var stateDefinitionsForProperty = _stateDefinitions[iStateProperty];
    //    PropertyStateTuple newTuple = new PropertyStateTuple (stateProperty, stateDefinitionsForProperty[arrayIndices[_statePropertyCount - 1 - iStateProperty]]);
    //    result[currentStateCombination, iStateProperty] = newTuple;
    //  }
    //}



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