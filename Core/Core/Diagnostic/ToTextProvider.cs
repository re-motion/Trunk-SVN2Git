/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Diagnostic
{
  /// <summary>
  /// Provides conversion of arbitray objects into human readable text form 
  /// using a fallback cascade starting with registered external object-to-text-conversion-handlers.
  /// 
  /// To convert a single object into its text form, call <see cref="ToTextString"/> 
  /// 
  /// The conversion is done in <see cref="ToText"/> method through the following mechanisms, in order of precedence:
  /// <list type="number">
  /// <item>Handler for object type registered (see <see cref="RegisterHandler{T}"/>)</item>
  /// <item>Implements <see cref="UseAutomaticStringEnclosing"/> (i.e. object supplies <c>ToText</c> method)</item>
  /// <item>Is a string or character (see <see cref="UseAutomaticCharEnclosing"/> and <see cref="UseAutomaticObjectToText"/> respectively)</item>
  /// <item>Is a primitive: Floating point numbers are alway output formatted US style.</item>
  /// <item>Is a (rectangular) array (arrays have are be treted seperately to prevent them from from being handled as IEnumerable)</item>
  /// <item>Implements IEnumerable</item>
  /// <item>Log instance members through reflection (see <see cref="IToTextHandler"/>)</item>
  /// <item>If all of the above fail, the object's <c>ToString</c> method is called</item>
  /// </list>
  /// 
  /// </summary>
  //TODO: auto proeprties, properties on single line, check resharper settigns
  public class ToTextProvider
  {
    private readonly Dictionary<Type, IToTextHandlerExternal> _typeHandlerMap = new Dictionary<Type, IToTextHandlerExternal> ();
    private static readonly NumberFormatInfo s_numberFormatInfoInvariant = CultureInfo.InvariantCulture.NumberFormat;

    // Define a cache instance (dictionary syntax)
    private static readonly InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> s_memberInfoCache = new InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]>();
    
    private bool _automaticObjectToText = true;
    private bool _automaticStringEnclosing = true;
    private bool _automaticCharEnclosing = true;

    private bool _emitPublicProperties = true;
    private bool _emitPublicFields = true;
    private bool _emitPrivateProperties = true;
    private bool _emitPrivateFields = true;


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

    public bool EmitPublicProperties
    {
      get { return _emitPublicProperties; }
      set { _emitPublicProperties = value; }
    }

    public bool EmitPublicFields
    {
      get { return _emitPublicFields; }
      set { _emitPublicFields = value; }
    }

    public bool EmitPrivateProperties
    {
      get { return _emitPrivateProperties; }
      set { _emitPrivateProperties = value; }
    }

    public bool EmitPrivateFields
    {
      get { return _emitPrivateFields; }
      set { _emitPrivateFields = value; }
    }

    public int ParentHandlerSearchDepth
    {
      get; set;
    }

    public bool ParentHandlerSearchUpToRoot
    {
      get; set;
    }

    public void SetAutomaticObjectToTextEmit (bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      _emitPublicProperties = emitPublicProperties;
      _emitPublicFields = emitPublicFields;
      _emitPrivateProperties = emitPrivateProperties;
      _emitPrivateFields = emitPrivateFields;
    }



    public string ToTextString (object obj)
    {
      var toTextBuilder = new ToTextBuilder (this);
      return toTextBuilder.ToText (obj).ToString ();
    }

    //TODO: refactor and split to type based strategy (IToTextHandler?)
    public void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);

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

      //IMixinTarget

      // TODO Functionality:
      // * Automatic call stack indentation

      if (obj == null)
      {
        Log ("null");
        toTextBuilder.AppendString ("null");
        return;
      }

      Type type = obj.GetType ();

      Log (type.ToString ());

      IToTextHandlerExternal handler = null;
      handler = GetHandler(type);


      if (handler != null)
      {
        //handler.DynamicInvoke (obj, toTextBuilder);
        handler.ToText (obj, toTextBuilder);
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
      else if (obj is IToTextHandler)
      {
        ((IToTextHandler) obj).ToText (toTextBuilder);
      }
      else if (obj is Type) 
      {
        // Catch Type|s here to avoid endless recursion in AutomaticObjectToText below.
        toTextBuilder.AppendString (obj.ToString ());
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
          // Make sure floating point numbers are emitted with '.' comma character (non-localized)
          // to avoid problems with comma as an e.g. sequence seperator character.
          // Since ToText is to be used for debug output, localizing everything to the common
          // IT norm of using US syntax (except for dates) makes sense.
          toTextBuilder.AppendString (((Single) obj).ToString (s_numberFormatInfoInvariant));
        }
        else if (type == typeof (Double))
        {
          toTextBuilder.AppendString (((Double) obj).ToString (s_numberFormatInfoInvariant));
        }
        else
        {
          // Emit primitives who have no registered specific handler without further processing.
          toTextBuilder.Append (obj);
        }
      }
      else if (type.IsArray)
      {
        ArrayToText ((Array) obj, toTextBuilder);
      }
          //else if (type.GetInterface ("IEnumerable") != null)
      else if (obj is IEnumerable)
      {
        CollectionToText ((IEnumerable) obj, toTextBuilder);
      }
      else
      {
        handler = GetHandlerWithBaseClassFallback (type);
        if (handler != null)
        {
          handler.ToText (obj, toTextBuilder);
        }
        else if (_automaticObjectToText)
        {
          AutomaticObjectToText (obj, toTextBuilder, EmitPublicProperties, EmitPublicFields, EmitPrivateProperties, EmitPrivateFields);
        }
        else
        {
          toTextBuilder.AppendString (obj.ToString ());
        }
      }
    }

    private IToTextHandlerExternal GetHandler (Type type)
    {
      return GetHandlerWithBaseClassFallback(type);
    }

    private IToTextHandlerExternal GetHandlerWithBaseClassFallback (Type type)
    {
      return GetHandlerWithBaseClassFallback (type, ParentHandlerSearchDepth, ParentHandlerSearchUpToRoot, 0);
    }

    private IToTextHandlerExternal GetHandlerWithBaseClassFallback (Type type, int recursionDepthMax, bool searchToRoot, int recursionDepth)
    {
      if (!searchToRoot && recursionDepth > recursionDepthMax)
      {
        return null;
      }

      IToTextHandlerExternal handler;
      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        return handler;
      }

      Type baseType = type.BaseType;
      if(baseType == null)
      {
        return null;
      }

      return GetHandlerWithBaseClassFallback (baseType, recursionDepthMax, searchToRoot, recursionDepth + 1);
    }


    public void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T),new ToTextHandlerExternal<T> (handler));
    }

    public void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }



    public static void CollectionToText (IEnumerable collection, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendEnumerable(collection);
    }


    public static void ArrayToText (Array array, ToTextBuilder toTextBuilder)
    {
      toTextBuilder.AppendArray(array);
    }



    private static object GetValue (object obj, Type type, MemberInfo memberInfo)
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

    //TODO: rename, NON static
    private static void AutomaticObjectToTextProcessMemberInfos (string message, Object obj, BindingFlags bindingFlags, 
                                                                 MemberTypes memberTypeFlags, ToTextBuilder toTextBuilder)
    {
      Type type = obj.GetType ();

      // Cache the member info result
      MemberInfo[] memberInfos = s_memberInfoCache.GetOrCreateValue (new Tuple<Type, BindingFlags> (type, bindingFlags), tuple => tuple.A.GetMembers (tuple.B));

      foreach (var memberInfo in memberInfos)
      {
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          string name = memberInfo.Name;

          // Skip backing fields
          //bool isCompilerGenerated = name.Contains("k__");
          bool isCompilerGenerated = memberInfo.IsDefined (typeof (CompilerGeneratedAttribute), false);
          if (!isCompilerGenerated)
          {
            object value = GetValue(obj, type, memberInfo);
            // AppendMember ToText value
            toTextBuilder.AppendMember(name, value);
          }
        }
      }
    }


    //TODO: rename
    public void AutomaticObjectToText (object obj, ToTextBuilder toTextBuilder, 
                                       bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      Type type = obj.GetType ();

      toTextBuilder.beginInstance(type);

      if (emitPublicProperties)
      {
        AutomaticObjectToTextProcessMemberInfos (
            "Public Properties", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      }
      if (emitPublicFields)
      {
        AutomaticObjectToTextProcessMemberInfos ("Public Fields", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      }
      if (emitPrivateProperties)
      {
        AutomaticObjectToTextProcessMemberInfos ("Non Public Properties", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      }
      if (emitPrivateFields)
      {
        AutomaticObjectToTextProcessMemberInfos ("Non Public Fields", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);
      }
      toTextBuilder.endInstance();
    }



    private void Log (string s)
    {
      Console.WriteLine ("[To]: " + s);
    }

    private void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }

 
  }
}