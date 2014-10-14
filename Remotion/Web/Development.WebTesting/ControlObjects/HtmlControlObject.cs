using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary HTML control within a re-motion application.
  /// </summary>
  public abstract class HtmlControlObject : ControlObject
  {
    protected HtmlControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }
  }
}