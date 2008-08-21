namespace Remotion.Logging
{
  public class SimpleLoggerNull : ISimpleLogger
  {
    public void It(object obj)
    {
      // Does nothing on purpose
    }

    public void It(string s)
    {
      // Does nothing on purpose
    }

    public void It(string format, params object[] parameters)
    {
      // Does nothing on purpose
    }

    public void Item(object obj)
    {
      // Does nothing on purpose
    }

    public void Item(string s)
    {
      // Does nothing on purpose
    }

    public void Item(string format, params object[] parameters)
    {
      // Does nothing on purpose
    }

    public void Sequence (params object[] parameters)
    {
      // Does nothing on purpose
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}