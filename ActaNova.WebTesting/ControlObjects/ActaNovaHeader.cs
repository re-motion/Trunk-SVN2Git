using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova header area.
  /// </summary>
  public class ActaNovaHeader : ActaNovaMainFrameControlObject
  {
    public ActaNovaHeader ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
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
    /// Returns the list of currently displayed ActaNova bread crumbs.
    /// </summary>
    public IReadOnlyList<ActaNovaBreadCrumb> BreadCrumbs
    {
      get
      {
        var breadCrumbsScope = Scope.FindId ("BreadCrumbsLabel");
        return new RetryUntilTimeout<IReadOnlyList<ActaNovaBreadCrumb>> (
            () => breadCrumbsScope.FindAllCss (".breadCrumbLink")
                .Select (s => new ActaNovaBreadCrumb (ID, Context.CloneForScope (s)))
                .ToList(),
            Context.Configuration.SearchTimeout,
            Context.Configuration.RetryInterval).Run();
      }
    }
  }
}