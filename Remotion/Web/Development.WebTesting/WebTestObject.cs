using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class of <see cref="PageObject"/> and <see cref="ControlObject"/>, providing common state and logic.
  /// </summary>
  public abstract class WebTestObject<TWebTestObjectContext>
      where TWebTestObjectContext : WebTestObjectContext
  {
    private readonly TWebTestObjectContext _context;

    protected WebTestObject ([NotNull] TWebTestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    /// <summary>
    /// The web test object's <see cref="WebTestObjectContext"/>.
    /// </summary>
    public TWebTestObjectContext Context
    {
      get { return _context; }
    }

    /// <summary>
    /// Shortcut for <see cref="Context"/>.<see cref="WebTestObjectContext.Scope"/>.
    /// </summary>
    public ElementScope Scope
    {
      get { return Context.Scope; }
    }
  }
}