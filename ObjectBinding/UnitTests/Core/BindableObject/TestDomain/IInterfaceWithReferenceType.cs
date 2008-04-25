namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public interface IInterfaceWithReferenceType<T>
      where T : class
  {
    T ExplicitInterfaceScalar { get; set; }
    T ExplicitInterfaceReadOnlyScalar { get; }
  }
}