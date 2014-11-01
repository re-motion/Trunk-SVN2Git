using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing an arbitrary ActaNova-based page.
  /// </summary>
  public class ActaNovaPageObject : AppToolsPageObject
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public ActaNovaPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }
  }
}