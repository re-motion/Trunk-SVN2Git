using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocEnumValueControlObject : BocControlObject
  {
    public BocEnumValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }
  }
}