using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary HTML control within a re-motion application.
  /// </summary>
  public abstract class HtmlControlObject : ControlObject
  {
    private readonly string _id;

    protected HtmlControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
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
  }
}