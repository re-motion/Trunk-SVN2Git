using System;
using System.Collections.Generic;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.MixerTool
{
  public interface IClassContextFinder
  {
    IEnumerable<ClassContext> FindClassContexts (MixinConfiguration configuration);
  }
}