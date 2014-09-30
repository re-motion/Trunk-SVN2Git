using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="WebButton"/>.
  /// </summary>
  public class WebButtonControlObject : RemotionControlObject
  {
    private readonly CommandControlObject _command;

    public WebButtonControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _command = new CommandControlObject (id, context);
    }

    public UnspecifiedPageObject Click (IWaitingStrategy waitingStrategy = null)
    {
      return _command.Click (waitingStrategy);
    }
  }
}