using System;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextSpecificHandler
  {
    void ToText (Object obj, IToTextBuilder toTextBuilder);
  }
}