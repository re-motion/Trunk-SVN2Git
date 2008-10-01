using System;
using Remotion.Diagnostics.ToText.Internal;

namespace Remotion.Diagnostics.ToText.Internal
{
  public interface IToTextSpecificHandler
  {
    void ToText (Object obj, IToTextBuilder toTextBuilder);
  }
}