namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class OrderWithNewPropertyAccessBase: DomainObject
  {
    protected OrderWithNewPropertyAccessBase()
    {
    }

    public string BaseProperty
    {
      get { return string.Empty; }
      set { }
    }
  }
}