using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType7
  {
    string One<T> (T t);
    string Two ();
    string Three ();
    string Four ();
    string Five ();
    string NotOverridden ();
  }

  public class BaseType7 : IBaseType7
  {
    public virtual string One<T> (T t)
    {
      return "BaseType7.One(" + t + ")";
    }

    public virtual string Two ()
    {
      return "BaseType7.Two";
    }

    public virtual string Three ()
    {
      return "BaseType7.Three";
    }

    public virtual string Four ()
    {
      return "BaseType7.Four-" + Five();
    }

    public virtual string Five ()
    {
      return "BaseType7.Five";
    }

    public string NotOverridden ()
    {
      return "BaseType7.NotOverridden";
    }
  }
}
