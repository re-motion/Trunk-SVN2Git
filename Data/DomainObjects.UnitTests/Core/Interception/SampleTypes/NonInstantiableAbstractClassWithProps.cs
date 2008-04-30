namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [Instantiable]
  [DBTable]
  public abstract class NonInstantiableAbstractClassWithProps : DomainObject
  {
    public static NonInstantiableAbstractClassWithProps NewObject ()
    {
      return NewObject<NonInstantiableAbstractClassWithProps> ().With();
    }

    protected NonInstantiableAbstractClassWithProps()
    {
    }

    [StorageClassNone]
    public abstract int Foo { get; }
  }
}