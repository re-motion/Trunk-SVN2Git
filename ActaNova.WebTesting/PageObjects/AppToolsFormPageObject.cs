﻿using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

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

    public UnspecifiedPageObject Perform (string itemID, ICompletionDetection completionDetection = null)
    {
      var fullItemID = string.Format ("{0}Button", itemID);
      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), fullItemID));
      return webButton.Click (completionDetection);
    }
  }
}