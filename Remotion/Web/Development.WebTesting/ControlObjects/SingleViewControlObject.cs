﻿using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="SingleView"/>.
  /// </summary>
  public class SingleViewControlObject : RemotionControlObject, IControlHost
  {
    public SingleViewControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public ScopeControlObject GetTopControls ()
    {
      var scope = FindChild ("TopControl");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    public ScopeControlObject GetView ()
    {
      var scope = FindChild ("View");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    public ScopeControlObject GetBottomControls ()
    {
      var scope = FindChild ("BottomControl");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    // Todo RM-6297: ControlHostingRemotionControlObject to remove code duplication with other IControlHost implementations?
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return controlSelectionCommand.Select (Context);
    }
  }
}