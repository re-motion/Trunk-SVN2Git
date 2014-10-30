using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.CompletionDetectionImplementation;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and instead provide a semantic interface. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public abstract class ControlObject : TestObject
  {
    private readonly string _id;

    protected ControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (context)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _id = id;
    }

    /// <summary>
    /// The control's ID.
    /// </summary>
    public string ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Allows access to child controls of the control.
    /// </summary>
    public IControlHost Children
    {
      get { return new ControlHost (Context); }
    }

    /// <summary>
    /// Returns a new <see cref="ICompletionDetection"/> object.
    /// </summary>
    protected ICompletionDetection Behavior
    {
      // Note: property exists for "syntactical sugar" only, therefore returning a new object in the get accessor is okay.
      get { return new CompletionDetector(); }
    }
  }
}