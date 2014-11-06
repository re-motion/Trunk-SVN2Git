using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.PageObjects
{
  public class AppToolsFormPageObject : AppToolsPageObject, IWebTestObjectWithWebButtons
  {
    public AppToolsFormPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return Scope.FindCss (".formPageTitleLabel").Text;
    }
  }
}