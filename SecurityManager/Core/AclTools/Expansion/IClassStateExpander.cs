using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public interface IClassStateExpander
  {
    Dictionary<Tuple<SecurableClassDefinition, StateCombination>, AccessControlList> Find (
        List<Tuple<SecurableClassDefinition, StateCombination>> list);
  }

}
