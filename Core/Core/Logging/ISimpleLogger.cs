namespace Remotion.Logging
{
  public interface ISimpleLogger
  {
    void It (object obj);
    void It (string s);
    void It (string format, params object[] parameters);
    void Item (object obj);
    void Item (string s);
    void Item (string format, params object[] parameters);

    void Sequence(params object[] parameters);
  }
}