namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
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