// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;
using Remotion.Mixins;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles registered types in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// Depending on the configuration of the class instance, only the handler whose type matches the instance type, or the nearest registered base type handler will be used used.
  /// Type handlers are registered through calling <see cref="ToTextProvider.RegisterSpecificTypeHandler{T}"/>.
  /// </summary>
  public class ToTextProviderRegisteredTypeHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
  {
    private readonly ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> _typeHandlerMap;
    private readonly bool _searchForParentHandlers = false;


    /// <summary>
    /// Constructs a handler using the passed handler map.
    /// </summary>
    /// <param name="typeHandlerMap">The handler map. External container so it can be shared with a 2nd 
    /// instance configured to exhibit different registered type handling behavior.</param> 
    /// <param name="searchForParentHandlers">If <c>false</c> only uses a handler if his type matches the type of the instance passed 
    /// to <see cref="ToTextIfTypeMatches"/>. If <c>true</c> recursively searches for the nearest registered base type handler, 
    /// if no exact match can be found.</param>
    public ToTextProviderRegisteredTypeHandler (ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> typeHandlerMap, bool searchForParentHandlers)
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
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      //Type type = toTextParameters.Type;

      // For mixin types we currently use the type handler for the target type.
      // TODO: Search for exact type handler first, do fallback to GetUnderlyingTargetType if none can be found.
      // TODO: Make work for OPF-generated types
      Type type = GetUnderlyingMixinType(toTextParameters.Type);

      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;


      IToTextSpecificTypeHandler specificTypeHandler = null;
      if (!_searchForParentHandlers || !toTextBuilder.ToTextProvider.Settings.UseParentHandlers)
      {
        specificTypeHandler = GetHandler (type);

        // TODO: Move GetHandler-calls to class to avoid code duplication of commented out code below.
        // Do not forget to change "Type type = GetUnderlyingMixinType(toTextParameters.Type);" above.
        //if (specificTypeHandler == null)
        //{
        //  Type underlyingMixinType  = GetUnderlyingMixinType (type);
        //  if (underlyingMixinType != type)
        //  {
        //    specificTypeHandler = GetHandler (underlyingMixinType);
        //  }
        //}
      }
      else
      {
        specificTypeHandler = GetHandlerWithBaseClassFallback (type, settings);
      }


      if (specificTypeHandler != null)
      {
        specificTypeHandler.ToText (toTextParameters.Object, toTextParameters.ToTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }

    private static Type GetUnderlyingMixinType (Type type)
    {
      return MixinTypeUtility.GetUnderlyingTargetType (type);
    }
  }
}