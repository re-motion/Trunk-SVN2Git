using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace ActaNova.WebTesting.PageObjects
{
  public class AppToolsPageObject : RemotionPageObject
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public AppToolsPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }
  }
}