using System;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextSpecificHandler
  {
    void ToText (Object obj, IToTextBuilderBase toTextBuilder);
  }
}