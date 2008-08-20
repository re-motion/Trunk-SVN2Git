using System;
using System.Collections;
using System.Collections.Generic;
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

    private string ToText (object o)
    {
      var toTextBuilder = new ToTextBuilder(this);
      return toTextBuilder.ToText(o).ToString();
    }

    public void ToText (object o, ToTextBuilder toTextBuilder)
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
        string s= (string) o;
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
      else if (type == typeof (char))
      {
        char c = (char) o;
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
      else if (type.IsPrimitive)
      {
        // TODO: Make sure floating point numbers are emitted with '.' comma character (non-localized)
        toTextBuilder.Append (o);
      }
      else if (type.IsArray)
      {
        ArrayToText ((Array) o, toTextBuilder);
      }
      else if (type.GetInterface ("IEnumerable") != null)
      {
        CollectionToText ((IEnumerable) o, toTextBuilder);
      }
      else if (_automaticObjectToText)
      {
        AutomaticObjectToText (o, toTextBuilder);
      }
      else
      {
        toTextBuilder.AppendString (o.ToString ());
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


    public void RegisterStringHandlers ()
    {
      RegisterHandler<String> ((x, ttb) => ttb.s ("\"").ts (x).s ("\""));
      RegisterHandler<char> ((x, ttb) => ttb.s ("'").ts (x).s ("'"));
    }


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



    private object GetValue (object o, Type type, MemberInfo memberInfo)
    {
      object value = null;
      if (memberInfo is PropertyInfo)
      {
        value = ((PropertyInfo)memberInfo).GetValue (o, null);
      }
      else if (memberInfo is FieldInfo)
      {
        //var field = type.GetField (memberInfo.Name);
        //Assertion.IsNotNull (field);
        //value = field.GetValue (o);
        value = ((FieldInfo) memberInfo).GetValue (o);
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
      Log ("---------------------------------------------------");
      Log (message);
      Log ("---------------------------------------------------");
      Type type = obj.GetType ();
      MemberInfo[] memberInfos = type.GetMembers (bindingFlags);


      //toTextBuilder.nl.s ("Members:").nl.s ();
      foreach (var memberInfo in memberInfos)
      {
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          string name = memberInfo.Name;
          Log("name=" + name);

          // Skip backing fields
          bool processMember = !name.Contains("k__");

          if (processMember)
          {
            object value = GetValue(obj, type, memberInfo);

            Log ("value=" + value);

            string valueToText = ToText (value);
            Log ("valueToText=" + valueToText);

            //toTextBuilder.s("(").m(name, value).s(")");
            toTextBuilder.m(name, value);
          }
        }
      }
      Log ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }



    public void AutomaticObjectToText (object o, ToTextBuilder toTextBuilder)
    {
      Log(">>>>>>>>>>> AUTOMATICOBJECTTOTEST: " + o + " " + o.GetType());

      Type type = o.GetType ();
      toTextBuilder.beginInstance(type);

      AutomaticObjectToTextProcessMemberInfos ("Public Properties", o, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Public Fields", o, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Non Public Properties", o, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      AutomaticObjectToTextProcessMemberInfos ("Non Public Fields", o, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);

      toTextBuilder.endInstance();

      //Type type = o.GetType();

      //toTextBuilder.nl.s ("Fields:").nl.s();
      //foreach (var info in type.GetFields())
      //{
      //  toTextBuilder.AppendString(info.ToString());
      //}

      //toTextBuilder.nl.s ("Properties:").nl.s ();
      //foreach (var info in type.GetProperties ())
      //{
      //  toTextBuilder.AppendString (info.ToString ());

      //  Log( "!!!! " + type.GetProperty(info.Name).GetValue(o, null).ToString() );
      //}

      

      //MemberInfo[] memberInfos =
      //  type.GetMembers ( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

      //toTextBuilder.nl.s ("Members:").nl.s();
      //foreach (var memberInfo in memberInfos)
      //{

      //  toTextBuilder.nl.s(memberInfo.ToString()).comma.space.s(memberInfo.Name);
      //  //memberInfo.GetType().Is
      //}
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