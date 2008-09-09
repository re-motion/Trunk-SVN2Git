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
using System.Collections.Generic;
using Remotion.Diagnostics.ToText.Handlers;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// <para>Provides conversion of arbitray objects into human readable text form 
  /// using a fallback cascade starting with registered external object-to-text-conversion-handlers.</para>
  /// 
  /// <para>To convert a single object into its text form, call <see cref="ToTextString"/></para> 
  /// 
  /// <para>See <see cref="RegisterDefaultToTextProviderHandlers"/> for a description of the default ToText-handling-fallback-cascade.</para>
  /// </summary>
  public class ToTextProvider 
  {
    //private readonly ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> ();
    //private readonly ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceTypeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> ();

    private readonly ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> ();
    private readonly ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceTypeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> ();
    
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

    public ToTextProvider (ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> typeHandlerMap, ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> interfaceTypeHandlerMap)
    {
      RegisterDefaultToTextProviderHandlers();
      if (typeHandlerMap != null)
      {
        _typeHandlerMap.Add (typeHandlerMap);
      }
      if (interfaceTypeHandlerMap != null)
      {
        _interfaceTypeHandlerMap.Add (interfaceTypeHandlerMap);
      }
    }


    /// <summary>
    /// Registers the default handlers for the conversion of arbitray objects into human readable text form. 
    /// Handlers form a fallback cascade starting with registered external object-to-text-conversion-handlers.
    /// 
    /// The handlers are registered in the following order of precedence:
    /// <list type="number">
    /// <item>Handler for specific object type registered (see <see cref="RegisterSpecificTypeHandler{T}"/>).</item>
    /// <item>Implements <see cref="IToText"/> (i.e. object supplies <see cref="IToText.ToText"/> method).</item>
    /// <item>Is a string or character (see <see cref="ToTextProviderSettings.UseAutomaticCharEnclosing"/> and <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> respectively).</item>
    /// <item>Implements IFormattable ("is a primitive"); Primitives (e.g. floating point numbers) are alway output formatted locale agnostic.</item>
    /// <item>Is a (rectangular) array (rectangular arrays have to be treated seperately since the IEnumerable handler would treat them as one-dimensional).</item>
    /// <item>Implements IEnumerable.</item>
    /// <item>Handler for a specific interface which the object implements is registered (see e.g. <see cref="RegisterSpecificInterfaceHandlerWithLowestPriority{T}"/>).</item>
    /// <item>Handler for a base type of the specific object type registered (see <see cref="RegisterSpecificTypeHandler{T}"/>).</item>
    /// <item>Log instance members through reflection.</item>
    /// <item>Use object's <see cref="Object.ToString"/> method.</item>
    /// </list>
    /// 
    /// </summary>    
    private void RegisterDefaultToTextProviderHandlers()
    {
      RegisterToTextProviderHandler (new ToTextProviderNullHandler ());
      // We call this handler twice: first without and later with base class fallback. For this they need to share the registered type handlers in _typeHandlerMap.
      RegisterToTextProviderHandler (new ToTextProviderRegisteredTypeHandler (_typeHandlerMap,false));
      RegisterToTextProviderHandler (new ToTextProviderStringHandler ());
      RegisterToTextProviderHandler (new ToTextProviderIToTextHandlerHandler ());
      RegisterToTextProviderHandler (new ToTextProviderTypeHandler ());

      RegisterToTextProviderHandler (new ToTextProviderCharHandler ());
      RegisterToTextProviderHandler (new ToTextProviderFormattableHandler ());
      
      RegisterToTextProviderHandler (new ToTextProviderArrayHandler ());
      RegisterToTextProviderHandler (new ToTextProviderEnumerableHandler ());
      RegisterToTextProviderHandler (new ToTextProviderRegisteredInterfaceHandlerHandler (_interfaceTypeHandlerMap));
      // Second call of registered handler, this time with base class fallback.
      RegisterToTextProviderHandler (new ToTextProviderRegisteredTypeHandler (_typeHandlerMap, true));
      RegisterToTextProviderHandler (new ToTextProviderAutomaticObjectToTextHandler ());
      RegisterToTextProviderHandler (new ToTextProviderToStringHandler ());

    }


    public virtual string ToTextString (object obj)
    {
      var toTextBuilder = new ToTextBuilder (this);
      return toTextBuilder.WriteElement (obj).CheckAndConvertToString ();
    }

    public void ToText (object obj, IToTextBuilderBase toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      ToTextUsingToTextProviderHandlers (obj, toTextBuilder);
    }


    public void RegisterSpecificTypeHandler<T> (Action<T, IToTextBuilderBase> handler)
    {
      _typeHandlerMap.Add (typeof (T), new ToTextSpecificTypeHandlerWrapper<T> (handler));
    }

    public void RegisterSpecificTypeHandler (Type handledType, IToTextSpecificTypeHandler toTextSpecificTypeHandler)
    {
      _typeHandlerMap.Add (handledType, toTextSpecificTypeHandler);
    }

    public void RegisterSpecificInterfaceHandlerWithLowestPriority<T> (Action<T, IToTextBuilderBase> handler)
    {
      --_interfaceHandlerPriorityMin;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandlerWrapper<T> (handler, _interfaceHandlerPriorityMin));
    }

    public void RegisterSpecificInterfaceHandlerWithHighestPriority<T> (Action<T, IToTextBuilderBase> handler)
    {
      ++_interfaceHandlerPriorityMax;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandlerWrapper<T> (handler, _interfaceHandlerPriorityMax));
    }

    public void ClearSpecificTypeHandlers ()
    {
      _typeHandlerMap.Clear ();
    }



    private void Log (string s)
    {
      //Console.WriteLine ("[To]: " + s);
    }

    private void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }






    public bool ToTextUsingToTextProviderHandlers (object obj, IToTextBuilderBase toTextBuilder)
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


    //public ToTextProvider CloneDeepCopy ()
    //{
    //  var ttp = new ToTextProvider();
    //  ttp._typeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> ();
    //  private readonly ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceTypeHandlerMap = new ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> ();
      
    //  private int _interfaceHandlerPriorityMin = 0;
    //  private int _interfaceHandlerPriorityMax = 0;

    //  private readonly List<IToTextProviderHandler> _toTextProviderHandlers = new List<IToTextProviderHandler> ();
    //  private readonly Dictionary<Type, IToTextProviderHandler> _toTextProviderHandlerTypeToHandlerMap = new Dictionary<Type, IToTextProviderHandler> ();

    //  private readonly ToTextProviderSettings _toTextProviderSettings = new ToTextProviderSettings();
    //  public ToTextProviderSettings Settings
    //  {
    //    get { return _toTextProviderSettings; }
    //  }    
    //}
  }
}