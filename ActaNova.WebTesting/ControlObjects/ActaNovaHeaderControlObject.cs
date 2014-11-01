using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova header area.
  /// </summary>
  public class ActaNovaHeaderControlObject : ActaNovaMainFrameControlObject, IControlHost
  {
    public ActaNovaHeaderControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the current ActaNova application context or <see langword="null" /> if no application context is active.
    /// </summary>
    /// <remarks>
    /// ActaNova currently displays the application context as "(Verfahrensbereich BA)", this property returns only "Verfahrensbereich BA".
    /// </remarks>
    public string CurrentApplicationContext
    {
      get
      {
        var applicationContextScope = Scope.FindId ("CurrentAppContextLabel");
        var currentApplicationContext = applicationContextScope.Text.Trim();

        if (!currentApplicationContext.StartsWith ("("))
          return null;

        return currentApplicationContext.Substring (1, currentApplicationContext.Length - 2);
      }
    }

    /// <summary>
    /// Returns the current ActaNova user name.
    /// </summary>
    public string CurrentUser
    {
      get
      {
        // Todo RM-6297: Implement as soon as BocReferenceValueControlObject is implemented.
        throw new NotImplementedException ("Not implemented yet.");
      }
    }

    /// <summary>
    /// Returns the current ActaNova group name.
    /// </summary>
    public string CurrentGroup
    {
      get
      {
        // Todo RM-6297: Implement as soon as BocReferenceValueControlObject is implemented.
        throw new NotImplementedException ("Not implemented yet.");
      }
    }

    // Todo RM-6297: Implement "BocReferenceValue GetDefaultGroupControl()" as soon as BocReferenceValueControlObject is implemented.
    // Todo RM-6297: Implement "BocReferenceValue GetCurrentTenantControl()" as soon as BocReferenceValueControlObject is implemented.
    // Todo RM-6297: Probably make ActaNovaHeaderControlObject an IControlHost? Or use ActaNovaMainPage as entry point?

    /// <summary>
    /// Returns the list of currently displayed ActaNova bread crumbs.
    /// </summary>
    public IReadOnlyList<ActaNovaBreadCrumbControlObject> BreadCrumbs
    {
      get
      {
        var breadCrumbsScope = Scope.FindId ("BreadCrumbsLabel");
        return RetryUntilTimeout.Run (
            () => breadCrumbsScope.FindAllCss (".breadCrumbLink")
                .Select (s => new ActaNovaBreadCrumbControlObject (Context.CloneForControl (s)))
                .ToList());
      }
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}