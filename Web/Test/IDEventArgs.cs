using System;

namespace Remotion.Web.Test
{
  public class IDEventArgs : EventArgs
  {
    private readonly string _id;

    public IDEventArgs (string id)
    {
      _id = id;
    }

    public string ID
    {
      get { return _id; }
    }

  }
}