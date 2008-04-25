namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class NonInstantiableAbstractClass : DomainObject
  {
    public static NonInstantiableAbstractClass NewObject()
    {
      return NewObject<NonInstantiableAbstractClass>().With();
    }

    protected NonInstantiableAbstractClass ()
    {
    }
  
    public abstract void Foo ();
  }
}