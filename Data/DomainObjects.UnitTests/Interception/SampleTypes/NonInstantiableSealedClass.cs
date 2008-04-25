namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public sealed class NonInstantiableSealedClass : DomainObject
  {
    public static NonInstantiableSealedClass NewObject ()
    {
      return NewObject<NonInstantiableSealedClass> ().With();
    }

    public NonInstantiableSealedClass()
    {
    }
  }
}