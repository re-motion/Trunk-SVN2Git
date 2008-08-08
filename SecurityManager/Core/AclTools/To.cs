using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.Metadata;
using System.Collections;

namespace Remotion.SecurityManager.AclTools
{
  public partial class To
  {

    public static void Test ()
    {
      ClassStateTuple classStateTuple = null;
      Console.WriteLine (To.Text (classStateTuple));

      Object oClassStateTuple = classStateTuple;
      //oClassStateTuple.GetType().
      //typeof(ClassStateTuple).
    }
    //public static Dictionary<Type, ToText> toTextImplementations = new Dictionary<Type, ToText> {
    //    { typeof (string), StringToText },
    //    { typeof (IEnumerable), EnumerableToText }};

    public delegate string ToText (object t);

    public static string StringToText (object o)
    {
      return o.ToString();
    }

    //private ToText GetToText (object o)
    //{
    //  ToText toText = toTextImplementations
    //}

    //private static string EnumerableToText (object o)
    //{
    //  var strings = from item in ((IEnumerable)o).Cast<object>()
    //                select toTextImplementations
    //}

    public static string Text (Object obj)
    {
      return obj.ToString ();
    }

    public static string Text (Remotion.SecurityManager.Domain.Metadata.StateDefinition stateDefinition)
    {
      return stateDefinition.Name.LeftUntilChar ('|');
    }


    public static string Text<T> (IEnumerable<T> collection, string start, string seperator, string end)
    {
      Console.WriteLine ("typeof (T)=" + typeof (T));
      var sb = new StringBuilder();
      sb.Append (start);
      bool insertSeperator = false; // no seperator before first element
      foreach (T t in collection)
      {
        if (insertSeperator)
        {
          sb.Append (seperator);
        }
        else
        {
          insertSeperator = true;
        }

        //string s = To.Text (t);
        //sb.Append (s);
        sb.Append (To.Text (t));
        //sb.Append (" !!! ");
        //sb.Append (t.ToString());

      }
      sb.Append (end);
      return sb.ToString();
    }

    public static string Text<T> (IEnumerable<T> collection)
    {
      return To.Text (collection, "{", ",", "}");
    }

    //public static string Text<T> (IEnumerable<T> collection, ToText<T> toText)
    //{
    //  return To.Text (collection, "{", ",", "}", toText);
    //}


    public static string Text(ClassStateTuple classStateTuple) 
    {
      return string.Format ("({0}: {1})", To.Text(classStateTuple.Class), To.Text(classStateTuple.StateList));
    }

    public static string Text (SecurableClassDefinition securableClassDefinition)
    {
      return securableClassDefinition.Name.RightUntilChar ('.');
    }

    //public static string Text (ClassStateDictionary classStateDictionary)
    //{
    //  var stringbuilder = new StringBuilder ();
    //  foreach (var mapEntry in classStateDictionary)
    //  {
    //    stringbuilder.AppendLine (String.Format ("[{0},{1}]", To.Text (mapEntry.Key), mapEntry.Value));
    //  }
    //  return stringbuilder.ToString ();
    //}
  
  }


  public static class ExtensionMethods
  {
    public static string LeftUntilChar (this string s, char separator)
    {
      int iSeparator = s.IndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (0, iSeparator);
      }
      else
      {
        return s;
      }
    }

    public static string RightUntilChar (this string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }

    public static string ShortName (this StateDefinition stateDefinition)
    {
      return stateDefinition.Name.LeftUntilChar ('|');
    }
  }
}
