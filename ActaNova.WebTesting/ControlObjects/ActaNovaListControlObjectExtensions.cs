﻿using ActaNova.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Extensions methods for the ActaNova BocList row and cells.
  /// </summary>
  public static class ActaNovaListControlObjectExtensions
  {
     // Note: if the number of extension methods is getting too high, it is better to refactor ActaNovaList to return ActaNova-specific rows & cells.

    /// <summary>
    /// Presses the expand button on the hierarchy row.
    /// </summary>
    public static void ExpandHierarchyRow (this BocListCellControlObject cell)
    {
      cell.Scope.FindCss ("img").PerformAction (s => s.Click(), cell.Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    /// <summary>
    /// Hovers the given <paramref name="cell"/> and returns the appearing <see cref="ActaNovaTreePopupTableControlObject"/>.
    /// </summary>
    public static ActaNovaTreePopupTableControlObject HoverAndGetTreePopup (this BocListCellControlObject cell)
    {
      cell.Scope.Hover();
      return cell.Children.GetControl (new SingleControlSelectionCommand<ActaNovaTreePopupTableControlObject> (new ActaNovaTreePopupTableSelector()));
    }
  }
}