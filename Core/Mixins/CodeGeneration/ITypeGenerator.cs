using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    Type GetBuiltType ();
    IEnumerable<Tuple<MixinDefinition, Type>> GetBuiltMixinTypes ();
  }
}
