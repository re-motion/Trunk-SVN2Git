using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles registered types in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// Depending on the configuration of the class instance, only the handler whose type matches the instance type, or the nearest registered base type handler will be used used.
  /// Type handlers are registered through calling <see cref="ToTextProvider.RegisterSpecificTypeHandler{T}"/>.
  /// </summary>
  public class ToTextProviderRegisteredTypeHandler : ToTextProviderHandler
  {
    private readonly Dictionary<Type, IToTextSpecificTypeHandler> _typeHandlerMap;
    private readonly bool _searchForParentHandlers = false;


    /// <summary>
    /// Constructs a handler using the passed handler map.
    /// </summary>
    /// <param name="typeHandlerMap">The handler map. External container so it can be shared with a 2nd 
    /// instance configured to exhibit different registered type handling behavior.</param> 
    /// <param name="searchForParentHandlers">If <c>false</c> only uses a handler if his type matches the type of the instance passed 
    /// to <see cref="ToTextIfTypeMatches"/>. If <c>true</c> recursively searches for the nearest registered base type handler, 
    /// if no exact match can be found.</param>
    public ToTextProviderRegisteredTypeHandler (Dictionary<Type, IToTextSpecificTypeHandler> typeHandlerMap, bool searchForParentHandlers)
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
}