using System;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocMultilineTextValue : IBocTextValueBase
  {
    new string[] Value { get; }
  }
}