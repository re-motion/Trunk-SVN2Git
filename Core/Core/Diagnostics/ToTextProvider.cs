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

namespace Remotion.Diagnostics
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
  /// <item>Implements <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> (i.e. object supplies <c>ToText</c> method)</item>
  /// <item>Is a string or character (see <see cref="ToTextProviderSettings.UseAutomaticCharEnclosing"/> and <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> respectively)</item>
  /// <item>Is a primitive: Floating point numbers are alway output formatted US style.</item>
  /// <item>Is a (rectangular) array (arrays have are be treted seperately to prevent them from from being handled as IEnumerable)</item>
  /// <item>Implements IEnumerable</item>
  /// <item>Log instance members through reflection (see <see cref="IToTextHandler"/>)</item>
  /// <item>If all of the above fail, the object's <c>ToString</c> method is called</item>
  /// </list>
  /// 
  /// </summary>
  public class ToTextProvider
  {
    private readonly Dictionary<Type, IToTextHandlerExternal> _typeHandlerMap = new Dictionary<Type, IToTextHandlerExternal> ();
    private readonly Dictionary<Type, IToTextInterfaceHandlerExternal> _interfaceTypeHandlerMap = new Dictionary<Type, IToTextInterfaceHandlerExternal> ();
    
    private int _interfaceHandlerPriorityMin = 0;
    private int _interfaceHandlerPriorityMax = 0;

    private readonly List<IToTextProviderHandler> _toTextProviderHandlers = new List<IToTextProviderHandler> ();
    private readonly Dictionary<Type, IToTextProviderHandler> _toTextProviderHandlerTypeToHandlerMap = new Dictionary<Type, IToTextProviderHandler> ();

    private readonly ToTextProviderSettings _toTextProviderSettings = new ToTextProviderSettings();
    public ToTextProviderSettings Settings
    {
      get { return _toTextProviderSettings; }
    }



    public ToTextProvider ()
    {
      RegisterDefaultToTextProviderHandlers();
    }




    private void RegisterDefaultToTextProviderHandlers()
    {
      // Handle Cascade:
      // *) Is null
      // *) Type handler registered
      // *) Is string (Treat seperately to prevent from being treated as IEnumerable)
      // *) Primitives (Formattables): To prevent them from being handled through reflection
      // *) Is rectangular array (Treat seperately to prevent from being treated as 1D-collection by IEnumerable)
      // *) Implements IToTextHandler
      // *) If !IsInterface: Base type handler registered (recursive)
      // *) Implements IEnumerable ("is container")
      // *) If enabled: Log properties through reflection
      // *) ToString()

      RegisterToTextProviderHandler (new ToTextProviderNullHandler ());
      // We call this handler twice: first without and later with base class fallback. For this they need to share the registered type handlers in _typeHandlerMap.
      RegisterToTextProviderHandler (new ToTextProviderRegisteredHandler (_typeHandlerMap,false));
      RegisterToTextProviderHandler (new ToTextProviderStringHandler ());
      RegisterToTextProviderHandler (new ToTextProviderIToTextHandlerHandler ());
      RegisterToTextProviderHandler (new ToTextProviderTypeHandler ());

      RegisterToTextProviderHandler (new ToTextProviderCharHandler ());
      RegisterToTextProviderHandler (new ToTextProviderFormattableHandler ());
      
      RegisterToTextProviderHandler (new ToTextProviderArrayHandler ());
      RegisterToTextProviderHandler (new ToTextProviderEnumerableHandler ());
      RegisterToTextProviderHandler (new ToTextProviderRegisteredInterfaceHandlerHandler (_interfaceTypeHandlerMap));
      // Second call of registered handler, this time with base class fallback.
      RegisterToTextProviderHandler (new ToTextProviderRegisteredHandler (_typeHandlerMap, true));
      RegisterToTextProviderHandler (new ToTextProviderAutomaticObjectToTextHandler ());
      RegisterToTextProviderHandler (new ToTextProviderToStringHandler ());

    }


    public string ToTextString (object obj)
    {
      var toTextBuilder = new ToTextBuilder (this);
      return toTextBuilder.ToText (obj).CheckAndConvertToString ();
    }

    public void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      ToTextUsingToTextProviderHandlers (obj, toTextBuilder);
    }


    public void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T), new ToTextHandlerExternal<T> (handler));
    }

    public void RegisterInterfaceHandlerAppendLast<T> (Action<T, ToTextBuilder> handler)
    {
      --_interfaceHandlerPriorityMin;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextInterfaceHandlerExternal<T> (handler, _interfaceHandlerPriorityMin));
    }

    public void RegisterInterfaceHandlerAppendFirst<T> (Action<T, ToTextBuilder> handler)
    {
      ++_interfaceHandlerPriorityMax;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextInterfaceHandlerExternal<T> (handler, _interfaceHandlerPriorityMax));
    }

    public void ClearHandlers ()
    {
      _typeHandlerMap.Clear ();
    }



    private void Log (string s)
    {
      Console.WriteLine ("[To]: " + s);
    }

    private void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }






    public bool ToTextUsingToTextProviderHandlers (object obj, ToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      Assertion.IsTrue (toTextBuilder.ToTextProvider == this);

      Type type = (obj != null) ? obj.GetType () : null;
      var parameters = new ToTextParameters () { Object = obj, Type = type, ToTextBuilder = toTextBuilder };

      foreach (var toTextProviderHandler in _toTextProviderHandlers)
      {
        if (!toTextProviderHandler.Disabled)
        {
          var feedback = new ToTextProviderHandlerFeedback ();
          toTextProviderHandler.ToTextIfTypeMatches (parameters, feedback);
          if (feedback.Handled)
          {
            Log ("[ToTextUsingToTextProviderHandlers] handled by: " + toTextProviderHandler);
            break;
          }
        }
      }

      return false;
    }



    public void RegisterToTextProviderHandler<T> (T toTextProviderHandler)  where T : IToTextProviderHandler
    {
      _toTextProviderHandlers.Add (toTextProviderHandler);
      _toTextProviderHandlerTypeToHandlerMap[typeof (T)] = toTextProviderHandler;
    }

    public T GetToTextProviderHandler<T> ()
    {

      IToTextProviderHandler toTextProviderHandler;
      _toTextProviderHandlerTypeToHandlerMap.TryGetValue (typeof (T), out toTextProviderHandler);
      if (toTextProviderHandler == null)
      {
        throw new KeyNotFoundException ("No ToTextProviderHandler registered for " + typeof(T));
      }
      return (T) toTextProviderHandler;
    }
  }


  public interface IToTextProviderHandler
  {
    void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);
    bool Disabled { get; set; }
  }

  public abstract class ToTextProviderHandler : IToTextProviderHandler
  {
    protected ToTextProviderHandler ()
    {
      Disabled = false;
    }

    protected void Log (string s)
    {
      Console.WriteLine ("[ToTextProviderHandler]: " + s);
    }

    public abstract void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);
    public bool Disabled { get; set; }
  }

  public class ToTextProviderHandlerFeedback
  {
    public ToTextProviderHandlerFeedback ()  
    {
      Handled = false;
    }
    public bool Handled { get; set; }
  }

  public class ToTextParameters
  {
    public object Object { get; set; }
    public Type Type { get; set; }
    public ToTextBuilder ToTextBuilder { get; set; }
  }


  public class ToTextProviderNullHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      if (toTextParameters.Object == null)
      {
        Log ("null");
        toTextParameters.ToTextBuilder.AppendString ("null");
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }

 
  public class ToTextProviderRegisteredHandler : ToTextProviderHandler
  {
    private readonly Dictionary<Type, IToTextHandlerExternal> _typeHandlerMap;
    private readonly bool _searchForParentHandlers = false;

    public ToTextProviderRegisteredHandler (Dictionary<Type, IToTextHandlerExternal> typeHandlerMap, bool searchForParentHandlers)
    {
      _typeHandlerMap = typeHandlerMap;
      _searchForParentHandlers = searchForParentHandlers;
    }

    private IToTextHandlerExternal GetHandler (Type type)
    {
      return GetHandlerWithBaseClassFallback (type, 0, false, 0);
    }

    private IToTextHandlerExternal GetHandlerWithBaseClassFallback (Type type, ToTextProviderSettings settings)
    {
      return GetHandlerWithBaseClassFallback (type, settings.ParentHandlerSearchDepth, settings.ParentHandlerSearchUpToRoot, 0);
    }

    private IToTextHandlerExternal GetHandlerWithBaseClassFallback (Type type, int recursionDepthMax, bool searchToRoot, int recursionDepth)
    {
      if ((type == null) || (!searchToRoot && recursionDepth > recursionDepthMax))
      {
        return null;
      }

      IToTextHandlerExternal handler;
      _typeHandlerMap.TryGetValue (type, out handler);

      if (handler != null)
      {
        return handler;
      }

      return GetHandlerWithBaseClassFallback (type.BaseType, recursionDepthMax, searchToRoot, recursionDepth + 1);
    }


    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;


      IToTextHandlerExternal handler = null;
      if (!_searchForParentHandlers || !toTextBuilder.ToTextProvider.Settings.UseParentHandlers)
      {
        handler = GetHandler (toTextParameters.Type);
      }
      else
      {
        handler = GetHandlerWithBaseClassFallback (toTextParameters.Type, settings);
      }


      if (handler != null)
      {
        handler.ToText (toTextParameters.Object, toTextParameters.ToTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }





  public class ToTextProviderStringHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (string))
      {
        string s= (string) obj;
        if (settings.UseAutomaticStringEnclosing)
        {
          toTextBuilder.AppendChar ('"');
          toTextBuilder.AppendString (s);
          toTextBuilder.AppendChar ('"');
        }
        else
        {
          toTextBuilder.AppendString(s);
        }
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }


  public class ToTextProviderIToTextHandlerHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is IToTextHandler)
      {
        ((IToTextHandler) obj).ToText (toTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }

  public class ToTextProviderTypeHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is Type) 
      {
        // Catch Type|s to avoid endless recursion. 
        toTextBuilder.AppendString (obj.ToString ());
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }



  public class ToTextProviderCharHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (char))
      {
        char c = (char) obj;
        if (settings.UseAutomaticCharEnclosing)
        {
          toTextBuilder.AppendChar ('\'');
          toTextBuilder.AppendChar (c);
          toTextBuilder.AppendChar ('\'');
        }
        else
        {
          toTextBuilder.AppendChar (c);
        }

        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }


  public class ToTextProviderFormattableHandler : ToTextProviderHandler
  {
    private static readonly CultureInfo s_cultureInfoInvariant = CultureInfo.InvariantCulture;

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);


      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if(obj is IFormattable)
      {
        IFormattable formattable = (IFormattable) obj;
        //toTextBuilder.AppendString (StringUtility.Format (obj, s_cultureInfoInvariant));
        toTextBuilder.AppendString (formattable.ToString (null, s_cultureInfoInvariant));
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }




  public class ToTextProviderArrayHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (type.IsArray)
      {
        toTextBuilder.AppendArray ((Array) obj);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }

  public class ToTextProviderEnumerableHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is IEnumerable)
      {
        toTextBuilder.AppendEnumerable ((IEnumerable) obj);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }

  public class ToTextProviderRegisteredInterfaceHandlerHandler : ToTextProviderHandler
  {
    private readonly Dictionary<Type, IToTextInterfaceHandlerExternal> _interfaceTypeHandlerMap;
    private int _interfaceHandlerPriorityMin = 0;
    private int _interfaceHandlerPriorityMax = 0;

    public ToTextProviderRegisteredInterfaceHandlerHandler (Dictionary<Type, IToTextInterfaceHandlerExternal> interfaceTypeHandlerMap)
    {
      _interfaceTypeHandlerMap = interfaceTypeHandlerMap;
    }

    public void RegisterInterfaceHandlerAppendLast<T> (Action<T, ToTextBuilder> handler)
    {
      --_interfaceHandlerPriorityMin;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextInterfaceHandlerExternal<T> (handler, _interfaceHandlerPriorityMin));
    }

    public void RegisterInterfaceHandlerAppendFirst<T> (Action<T, ToTextBuilder> handler)
    {
      ++_interfaceHandlerPriorityMax;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextInterfaceHandlerExternal<T> (handler, _interfaceHandlerPriorityMax));
    }
    
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      toTextProviderHandlerFeedback.Handled = false;

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (!settings.UseInterfaceHandlers)
      {
        return;
      }

      IToTextInterfaceHandlerExternal interfaceHandlerWithMaximumPriority = null;
      foreach (var interfaceType in type.GetInterfaces ())
      {
        IToTextInterfaceHandlerExternal interfaceHandler;
        _interfaceTypeHandlerMap.TryGetValue (interfaceType, out interfaceHandler);
        if (interfaceHandler != null &&
          (interfaceHandlerWithMaximumPriority == null || (interfaceHandler.Priority > interfaceHandlerWithMaximumPriority.Priority)))
        {
          interfaceHandlerWithMaximumPriority = interfaceHandler;
        }
      }

      if (interfaceHandlerWithMaximumPriority != null)
      {
        interfaceHandlerWithMaximumPriority.ToText (obj, toTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }



  public class ToTextProviderAutomaticObjectToTextHandler : ToTextProviderHandler
  {
    // Define a cache instance (dictionary syntax)
    private static readonly InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> s_memberInfoCache = new InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> ();

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (settings.UseAutomaticObjectToText)
      {
        ObjectToText (obj, toTextBuilder, settings.EmitPublicProperties, settings.EmitPublicFields, settings.EmitPrivateProperties, settings.EmitPrivateFields);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }


    private void ObjectToText (object obj, ToTextBuilder toTextBuilder,
                                       bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      Type type = obj.GetType ();

      toTextBuilder.beginInstance (type);

      if (emitPublicProperties)
      {
        ObjectToTextProcessMemberInfos (
            "Public Properties", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      }
      if (emitPublicFields)
      {
        ObjectToTextProcessMemberInfos ("Public Fields", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      }
      if (emitPrivateProperties)
      {
        ObjectToTextProcessMemberInfos ("Non Public Properties", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      }
      if (emitPrivateFields)
      {
        ObjectToTextProcessMemberInfos ("Non Public Fields", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);
      }
      toTextBuilder.endInstance ();
    }

    private void ObjectToTextProcessMemberInfos (string message, Object obj, BindingFlags bindingFlags,
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

          // Skip compiler generated backing fields
          bool isCompilerGenerated = memberInfo.IsDefined (typeof (CompilerGeneratedAttribute), false);
          if (!isCompilerGenerated)
          {
            object value = GetValue (obj, type, memberInfo);
            // AppendMember ToText value
            toTextBuilder.AppendMember (name, value);
          }
        }
      }
    }

    private object GetValue (object obj, Type type, MemberInfo memberInfo)
    {
      object value = null;
      if (memberInfo is PropertyInfo)
      {
        value = ((PropertyInfo) memberInfo).GetValue (obj, null);
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

    private bool AutomaticObjectToText
    {
      get { return true; }
    }
  }




  public class ToTextProviderToStringHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      toTextBuilder.AppendString (obj.ToString ());

      toTextProviderHandlerFeedback.Handled = true;
    }
  }


  public class ToTextProviderSettings
  {
    public ToTextProviderSettings ()
    {
      UseAutomaticObjectToText = true;
      EmitPublicProperties = true;
      EmitPublicFields = true;
      EmitPrivateProperties = true;
      EmitPrivateFields = true;

      UseAutomaticStringEnclosing = true;
      UseAutomaticCharEnclosing = true;

      UseInterfaceHandlers = true;

      ParentHandlerSearchDepth = 0;
      ParentHandlerSearchUpToRoot = true;
      UseParentHandlers = false;
    }

    public bool UseAutomaticObjectToText { get; set; }
    public bool EmitPublicProperties { get; set; }
    public bool EmitPublicFields { get; set; }
    public bool EmitPrivateProperties { get; set; }
    public bool EmitPrivateFields { get; set; }

    public bool UseAutomaticStringEnclosing { get; set; }
    public bool UseAutomaticCharEnclosing { get; set; }

    public int ParentHandlerSearchDepth { get; set; }
    public bool ParentHandlerSearchUpToRoot { get; set; }
    public bool UseParentHandlers { get; set; }

    public bool UseInterfaceHandlers { get; set; }

    public void SetAutomaticObjectToTextEmit (bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      EmitPublicProperties = emitPublicProperties;
      EmitPublicFields = emitPublicFields;
      EmitPrivateProperties = emitPrivateProperties;
      EmitPrivateFields = emitPrivateFields;
    }    
  }


}