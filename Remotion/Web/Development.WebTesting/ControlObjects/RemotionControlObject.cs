using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based control.
  /// </summary>
  public abstract class RemotionControlObject : ControlObject
  {
    protected static readonly IWaitingStrategy _defaultWaitStrategy = new WxePostBackWaitingStrategy();
    private readonly string _id;

    /// <summary>
    /// Initializes the control object with the given <paramref name="context"/> and <paramref name="id"/>.
    /// </summary>
    protected RemotionControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (context)
    {
      _id = id;
    }

    /// <summary>
    /// The control's ID.
    /// </summary>
    protected string ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Returns a child element of the control, specified by an <paramref name="idSuffix"/> parameter.
    /// </summary>
    protected ElementScope FindChild (string idSuffix)
    {
      var fullId = string.Format ("{0}_{1}", ID, idSuffix);
      return Scope.FindId (fullId);
    }
  }
}