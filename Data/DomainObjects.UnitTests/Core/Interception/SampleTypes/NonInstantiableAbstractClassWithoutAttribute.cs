namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [DBTable]
  public abstract class NonInstantiableAbstractClassWithoutAttribute : DomainObject
  {
    protected NonInstantiableAbstractClassWithoutAttribute ()
    {
    }
  }
}