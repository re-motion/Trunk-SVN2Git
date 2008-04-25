using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  [Serializable]
  public class ClassContextEventArgs : EventArgs
  {
    public readonly ClassContext ClassContext;

    public ClassContextEventArgs (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ClassContext = classContext;
    }
  }
}