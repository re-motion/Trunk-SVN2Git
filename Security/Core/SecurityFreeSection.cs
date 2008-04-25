using System;
using System.Runtime.Remoting.Messaging;

namespace Remotion.Security
{
  public sealed class SecurityFreeSection : IDisposable
  {
    private static string s_activeSectionCountKey = typeof (SecurityFreeSection).AssemblyQualifiedName + "_ActiveSectionCount";

    public static bool IsActive
    {
      get { return ActiveSectionCount > 0; }
    }

    private static int ActiveSectionCount
    {
      get
      {
        int? count = (int?) CallContext.GetData (s_activeSectionCountKey);
        if (!count.HasValue)
        {
          count = 0;
          CallContext.SetData (s_activeSectionCountKey, count);
        }

        return count.Value;
      }
      set
      {
        CallContext.SetData (s_activeSectionCountKey, value);
      }
    }

    private bool _isDisposed;

    public SecurityFreeSection ()
    {
      ActiveSectionCount++;
    }

    void IDisposable.Dispose ()
    {
      Dispose ();
    }

    private void Dispose ()
    {
      if (!_isDisposed)
      {
        ActiveSectionCount--;
        _isDisposed = true;
      }
    }

    public void Leave ()
    {
      Dispose ();
    }
  }
}