using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Base class for all ActaNova-related page objects.
  /// </summary>
  public abstract class ActaNovaPageObjectBase : PageObject
  {
    protected ActaNovaPageObjectBase ([NotNull] PageObjectContext context)
        : base (context)
    {
    }
  }
}