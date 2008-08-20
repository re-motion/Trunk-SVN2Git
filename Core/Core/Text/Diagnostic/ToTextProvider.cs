using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Text.Diagnostic
{
  public class ToTextProvider
  {
    private Dictionary<Type, Delegate> _typeHandlerMap = new Dictionary<Type, Delegate> ();
    private bool _automaticObjectToText = true;
    private bool _automaticStringEnclosing = true;
    private bool _automaticCharEnclosing = true;

    private readonly NumberFormatInfo _numberFormatInfoUS = new CultureInfo ("en-US", false).NumberFormat;


    public bool UseAutomaticObjectToText
    {
      get { return _automaticObjectToText; }
      set { _automaticObjectToText = value; }
    }

    public bool UseAutomaticStringEnclosing
    {
      get { return _automaticStringEnclosing; }
      set { _automaticStringEnclosing = value; }
    }

    public bool UseAutomaticCharEnclosing
    {
      get { return _automaticCharEnclosing; }
      set { _automaticCharEnclosing = value; }
    }

    //private string ToText (object obj)
    //{
    //  var toTextBuilder = new ToTextBuilder(this);
    //  return toTextBuilder.ToText(obj).ToString();
    //}

    public string ToTextString (object obj)
    {
      var toTextBuilder = new ToTextBuilder (this);
      return toTextBuilder.ToText (obj).ToString ();
    }

    public void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      Assertion.IsNotNull (toTextBuilder);

      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Is primitive: To prevent them from being handled through reflection
      // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
      // *) Implements IToTextHandler
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      // Functionality:
      // * Register handlers for interfaces, which can be called by ToText handlers of specific types.
      // * Automatic call stack indentation

      if (obj == null)
      {
        Log ("null");
        toTextBuilder.AppendString ("null");
        return;
      }

      Delegate handler = null;
      Type type = obj.GetType ();

      Log (type.ToString ());

      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        handler.DynamicInvoke (obj, toTextBuilder);
      }
      else if (type == typeof (string))
      {
        string s= (string) obj;
        if (UseAutomaticStringEnclosing)
        {
          toTextBuilder.Append ('"');
          toTextBuilder.AppendString (s);
          toTextBuilder.Append ('"');
        }
        else
        {
          toTextBuilder.AppendString(s);
        }
      }
      else if (type == typeof (object).GetType () || type == typeof (Type))
      {
        // Catch type RuntimeType here to avoid endless recursion in AutomaticObjectToText below
        toTextBuilder.AppendString (type.ToString ());
      }
      else if (type.IsPrimitive)
      {
        if (type == typeof (char))
        {
          char c = (char) obj;
          if (UseAutomaticCharEnclosing)
          {
            toTextBuilder.Append ('\'');
            toTextBuilder.Append (c);
            toTextBuilder.Append ('\'');
          }
          else
          {
            toTextBuilder.Append (c);
          }
        }
        else if (type == typeof (Single)) 
        {
          toTextBuilder.AppendString (((Single) obj).ToString (_numberFormatInfoUS));
        }
        else if (type == typeof (Double))
        {
          toTextBuilder.AppendString (((Double) obj).ToString (_numberFormatInfoUS));
        }
        else
        {
          // TODO: Make sure floating point numbers are emitted with '.' comma character (non-localized)
          toTextBuilder.Append (obj);
        }
      }
      else if (type.IsArray)
      {
        ArrayToText ((Array) obj, toTextBuilder);
      }
      else if (type.GetInterface ("IEnumerable") != null)
      {
        CollectionToText ((IEnumerable) obj, toTextBuilder);
      }
      else if (_automaticObjectToText)
      {
        AutomaticObjectToText (obj, toTextBuilder);
      }
      else
      {
        toTextBuilder.AppendString (obj.ToString ());
      }
    }


    public void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T), handler);
    }

    public void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }


    //public void RegisterStringHandlers ()
    //{
    //  RegisterHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
    //  RegisterHandler<char> ((x, ttb) => ttb.s ("'").ts (x).s ("'"));
    //}


    public void CollectionToText (IEnumerable collection, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendEnumerable(collection);
    }


    public void ArrayToText (Array array, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendArray(array);
    }


    // Outputs the names & values of all public fields and properties of the passed Object.
    private static void ObjectFieldsAndPropertiesToString (object obj)
    {
      Type type = obj.GetType();

      foreach (var fieldInfo in type.GetFields())
      {
        string fieldName = fieldInfo.Name;
        var fieldValue = type.GetField(fieldName).GetValue(obj);
        Console.WriteLine (String.Format ("\nField: name={0}, value={1}", fieldName, fieldValue));
      }

      foreach (var fieldInfo in type.GetProperties())
      {
        string propertyName = fieldInfo.Name;
        var propertyValue = type.GetProperty(propertyName).GetValue(obj, null);
        Console.WriteLine (String.Format ("\nProperty: name={0}, value={1}", propertyName, propertyValue));
      }
    }



    private object GetValue (object obj, Type type, MemberInfo memberInfo)
    {
      object value = null;
      if (memberInfo is PropertyInfo)
      {
        value = ((PropertyInfo)memberInfo).GetValue (obj, null);
      }
      else if (memberInfo is FieldInfo)
      {
        value = ((FieldInfo) memberInfo).GetValue (obj);
      }
      else
      {
        throw new System.NotImplementedException ();
      }
      return value;
    }

    public void AutomaticObjectToTextProcessMemberInfos (string message, Object obj, BindingFlags bindingFlags, 
      MemberTypes memberTypeFlags, ToTextBuilder toTextBuilder)
    {
      Type type = obj.GetType ();
      MemberInfo[] memberInfos = type.GetMembers (bindingFlags);


      foreach (var memberInfo in memberInfos)
      {
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          string name = memberInfo.Name;

          // Skip backing fields
          bool processMember = !name.Contains("k__");

          if (processMember)
          {
            object value = GetValue(obj, type, memberInfo);
            // AppendMember ToText|s value
            toTextBuilder.AppendMember(name, value);
          }
        }
      }
    }



    public void AutomaticObjectToText (object obj, ToTextBuilder toTextBuilder)
    {
      Type type = obj.GetType ();

      toTextBuilder.beginInstance(type);

      AutomaticObjectToTextProcessMemberInfos ("Public Properties", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Public Fields", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Non Public Properties", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Non Public Fields", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);

      toTextBuilder.endInstance();
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