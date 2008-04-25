namespace Remotion.UnitTests.Mixins.Utilities.Serialization
{
  public class GenericType<T>
  {
    public GenericType () { }
      
    public GenericType (T t) { }

    public void NonGenericMethod (T t)
    {
    }

    public void GenericMethod<U> (T t, U u)
    {
    }

    public int NonGenericProperty
    {
      get { return 0; }
    }

    public T GenericProperty
    {
      get { return default (T); }
    }

    public object this [int index]
    {
      get { return null; }
    }

    public T this [string index]
    {
      get { return default (T); }
    }

    public int this[T index]
    {
      get { return 0; }
    }

    public T this[T index, int i]
    {
      get { return index; }
    }

    public T this[T index, T t]
    {
      get { return index; }
    }
  }
}