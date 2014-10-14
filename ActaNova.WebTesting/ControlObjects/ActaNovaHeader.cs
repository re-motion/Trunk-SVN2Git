using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaHeader : ActaNovaMainFrameControlObject
  {
    public ActaNovaHeader ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string CurrentApplicationContext
    {
      get
      {
        // TODO RM-6297: Parse label text?
        var applicationContextScope = Scope.FindId ("CurrentAppContextLabel");
        return applicationContextScope.Text;
      }
    }

    public IReadOnlyList<ActaNovaBreadCrumb> BreadCrumbs
    {
      get
      {
        var breadCrumbsScope = Scope.FindId ("BreadCrumbsLabel");
        return new RetryUntilTimeout<IReadOnlyList<ActaNovaBreadCrumb>> (
            () => breadCrumbsScope.FindAllCss (".breadCrumbLink")
                .Select (s => new ActaNovaBreadCrumb (s.Id, Context.CloneForScope (s)))
                .ToList(),
            Context.Configuration.SearchTimeout,
            Context.Configuration.RetryInterval).Run();
      }
    }
  }
}