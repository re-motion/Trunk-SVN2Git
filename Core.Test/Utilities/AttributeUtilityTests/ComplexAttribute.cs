using System;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  public class ComplexAttribute : Attribute
  {
    public Type T;
    public object[] Os;
    public Type[] Ts;

    private string s;

    public ComplexAttribute ()
    {
    }

    public ComplexAttribute (Type t)
    {
      T = t;
    }

    public ComplexAttribute (string s)
    {
      this.s = s;
    }

    public ComplexAttribute (string s, params object[] os)
    {
      S = s;
      Os = os;
    }

    public ComplexAttribute (Type t, params Type[] ts)
    {
      T = t;
      Ts = ts;
    }

    public string S
    {
      get { return s; }
      set { s = value; }
    }
  }
}