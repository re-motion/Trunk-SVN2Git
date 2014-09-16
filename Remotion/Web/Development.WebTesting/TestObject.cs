using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class of <see cref="PageObject"/> and <see cref="ControlObject"/>, holding common state and logic.
  /// </summary>
  public abstract class TestObject
  {
    private readonly TestObjectContext _context;

    /// <summary>
    /// The test object must be provided with a valid <see cref="TestObjectContext"/> which already references its HTML root element in the
    /// <see cref="TestObjectContext.Scope"/> property. This implies that finding the test object within the parent is the responsibility of the
    /// parent.
    /// </summary>    
    protected TestObject ([NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    /// <summary>
    /// The test object's context.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global : May be used by web tests to directly interact with the underlying web testing framework.
    public TestObjectContext Context
    {
      get { return _context; }
    }

    /// <summary>
    /// Shortcut for <see cref="Context"/>.<see cref="TestObjectContext.Scope"/>.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global : May be used by web tests to directly interact with the underlying web testing framework.
    public ElementScope Scope
    {
      get { return Context.Scope; }
    }

    /// <summary>
    /// Returns a new <see cref="UnspecifiedPageObject"/> with the same context as the current page.
    /// </summary>
    /// <returns>A new unspecified page object.</returns>
    protected UnspecifiedPageObject UnspecifiedPage ()
    {
      return new UnspecifiedPageObject (Context);
    }
  }
}