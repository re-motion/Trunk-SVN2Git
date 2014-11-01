﻿using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the first control of the given <typeparamref name="TControlObject"/> type within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class FirstControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IFirstControlSelector<TControlObject> _controlSelector;

    public FirstControlSelectionCommand ([NotNull] IFirstControlSelector<TControlObject> controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      _controlSelector = controlSelector;
    }

    public TControlObject Select (WebTestObjectContext context)
    {
      return _controlSelector.SelectFirst (context);
    }
  }
}