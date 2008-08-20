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
      //else if (_automaticObjectToText)
      //{
      //  AutomaticObjectToText(o, toTextBuilder);
      //}
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

    public void ProcessMemberInfos (string message, Object o, BindingFlags bindingFlags, MemberTypes memberTypeFlags, ToTextBuilder toTextBuilder)
    {
      Log ("---------------------------------------------------");
      Log (message);
      Log ("---------------------------------------------------");
      Type type = o.GetType ();
      MemberInfo[] memberInfos = type.GetMembers (bindingFlags);


      toTextBuilder.nl.s ("Members:").nl.s ();
      foreach (var memberInfo in memberInfos)
      {
        //toTextBuilder.nl.s (memberInfo.ToString ()).comma.space.s (memberInfo.Name);
        //if ((memberInfo.MemberType & (MemberTypes.Field | MemberTypes.Property)) != 0)
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          //LogVariables ("\nname: {0}, value: {1}", memberInfo.Name, type.GetProperty (memberInfo.Name).GetValue (o, null));
          //LogVariables ("name: {0}", memberInfo.Name);
          string name = memberInfo.Name;
          Log("name=" + name);
          var value = type.GetProperty(memberInfo.Name).GetValue(o, null);
          Log ("value=" + value);

          //if (type.IsPrimitive)
          //{
          //  toTextBuilder.Append (o);
          //}

          string valueToText = ToText (value);
          Log ("valueToText=" + valueToText);
          //LogVariables ("\nname: {0}, value: {1}", name, valueString);
        }
      }
      Log ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }

    public void AutomaticObjectToText (object o, ToTextBuilder toTextBuilder)
    {
      Log(">>>>>>>>>>> AUTOMATICOBJECTTOTEST: " + o + " " + o.GetType());
      //ObjectFieldsAndPropertiesToString(o);
      ProcessMemberInfos ("Public Properties", o, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      ProcessMemberInfos("Non Public Properties", o, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      ProcessMemberInfos ("Public Fields", o, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      ProcessMemberInfos ("Non Public Fields", o, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);

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