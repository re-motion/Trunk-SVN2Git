using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Context
{
  public class ClassContextCollector
  {
    private readonly List<ClassContext> _classContexts = new List<ClassContext>();

    public IEnumerable<ClassContext> CollectedContexts
    {
      get { return _classContexts; }
    }

    public void Add (ClassContext context)
    {
      if (context != null)
        _classContexts.Add (context);
    }


    public ClassContext GetCombinedContexts (Type contextType)
    {
      switch (_classContexts.Count)
      {
        case 0:
          return null;
        case 1:
          return _classContexts[0].CloneForSpecificType (contextType);
        default:
          return new ClassContext (contextType).InheritFrom (_classContexts);
      }
    }
  }
}