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
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Provides conversion of arbitray objects into human readable text form 
  /// using a fallback cascade starting with registered external object-to-text-conversion-handlers.
  /// 
  /// To convert a single object into its text form, call <see cref="ToTextString"/> 
  /// 
  /// The conversion is done in <see cref="ToText"/> method through the following mechanisms, in order of precedence:
  /// <list type="number">
  /// <item>Handler for object type registered (see <see cref="RegisterSpecificTypeHandler{T}"/>)</item>
  /// <item>Implements <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> (i.e. object supplies <c>ToText</c> method)</item>
  /// <item>Is a string or character (see <see cref="ToTextProviderSettings.UseAutomaticCharEnclosing"/> and <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> respectively)</item>
  /// <item>Is a primitive: Floating point numbers are alway output formatted US style.</item>
  /// <item>Is a (rectangular) array (arrays have are be treted seperately to prevent them from from being handled as IEnumerable)</item>
  /// <item>Implements IEnumerable</item>
  /// <item>Log instance members through reflection (see <see cref="IToText"/>)</item>
  /// <item>If all of the above fail, the object's <c>ToString</c> method is called</item>
  /// </list>
  /// 
  /// </summary>
  public class ToTextProvider
  {
    private readonly Dictionary<Type, IToTextSpecificTypeHandler> _typeHandlerMap = new Dictionary<Type, IToTextSpecificTypeHandler> ();
    private readonly Dictionary<Type, IToTextSpecificInterfaceHandler> _interfaceTypeHandlerMap = new Dictionary<Type, IToTextSpecificInterfaceHandler> ();
    
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
      // *) Implements IToText
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


    public void RegisterSpecificTypeHandler<T> (Action<T, ToTextBuilder> handler)
    {
      _typeHandlerMap.Add (typeof (T), new ToTextSpecificTypeHandler<T> (handler));
    }

    public void RegisterSpecificInterfaceHandlerWithLowestPriority<T> (Action<T, ToTextBuilder> handler)
    {
      --_interfaceHandlerPriorityMin;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandler<T> (handler, _interfaceHandlerPriorityMin));
    }

    public void RegisterSpecificInterfaceHandlerWithHighestPriority<T> (Action<T, ToTextBuilder> handler)
    {
      ++_interfaceHandlerPriorityMax;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandler<T> (handler, _interfaceHandlerPriorityMax));
    }

    public void ClearSpecificTypeHandlers ()
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

  public class ToTextProviderRegisteredHandler : ToTextProviderHandler
  {
    private readonly Dictionary<Type, IToTextSpecificTypeHandler> _typeHandlerMap;
    private readonly bool _searchForParentHandlers = false;

    public ToTextProviderRegisteredHandler (Dictionary<Type, IToTextSpecificTypeHandler> typeHandlerMap, bool searchForParentHandlers)
    {
      _typeHandlerMap = typeHandlerMap;
      _searchForParentHandlers = searchForParentHandlers;
    }

    private IToTextSpecificTypeHandler GetHandler (Type type)
    {
      return GetHandlerWithBaseClassFallback (type, 0, false, 0);
    }

    private IToTextSpecificTypeHandler GetHandlerWithBaseClassFallback (Type type, ToTextProviderSettings settings)
    {
      return GetHandlerWithBaseClassFallback (type, settings.ParentHandlerSearchDepth, settings.ParentHandlerSearchUpToRoot, 0);
    }

    private IToTextSpecificTypeHandler GetHandlerWithBaseClassFallback (Type type, int recursionDepthMax, bool searchToRoot, int recursionDepth)
    {
      if ((type == null) || (!searchToRoot && recursionDepth > recursionDepthMax))
      {
        return null;
      }

      IToTextSpecificTypeHandler specificTypeHandler;
      _typeHandlerMap.TryGetValue (type, out specificTypeHandler);

      if (specificTypeHandler != null)
      {
        return specificTypeHandler;
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


      IToTextSpecificTypeHandler specificTypeHandler = null;
      if (!_searchForParentHandlers || !toTextBuilder.ToTextProvider.Settings.UseParentHandlers)
      {
        specificTypeHandler = GetHandler (toTextParameters.Type);
      }
      else
      {
        specificTypeHandler = GetHandlerWithBaseClassFallback (toTextParameters.Type, settings);
      }


      if (specificTypeHandler != null)
      {
        specificTypeHandler.ToText (toTextParameters.Object, toTextParameters.ToTextBuilder);
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

      if (obj is IToText)
      {
        ((IToText) obj).ToText (toTextBuilder);
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
}