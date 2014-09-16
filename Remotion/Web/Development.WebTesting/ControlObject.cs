using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and instead provide a semantic interface. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public abstract class ControlObject : TestObject
  {
    protected ControlObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }
  }
}