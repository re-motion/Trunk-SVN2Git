﻿using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaTinyMceControlObject"/>.
  /// </summary>
  public class ActaNovaTinyMceSelector : BocControlSelectorBase<ActaNovaTinyMceControlObject>
  {
     public ActaNovaTinyMceSelector ()
        : base ("BocTextValue")
    {
    }

    /// <inheritdoc/>
    protected override ActaNovaTinyMceControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new ActaNovaTinyMceControlObject (newControlObjectContext);
    }
  }
}