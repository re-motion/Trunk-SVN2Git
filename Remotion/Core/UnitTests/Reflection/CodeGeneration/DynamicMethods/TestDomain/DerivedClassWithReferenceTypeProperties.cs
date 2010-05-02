namespace Remotion.UnitTests.Reflection.CodeGeneration.DynamicMethods.TestDomain
{
  public class DerivedClassWithReferenceTypeProperties : ClassWithReferenceTypeProperties
  {
    private SimpleReferenceType _propertyWithPublicGetterAndSetter;

    public override SimpleReferenceType PropertyWithPublicGetterAndSetter
    {
      get { return _propertyWithPublicGetterAndSetter; }
      set { _propertyWithPublicGetterAndSetter = value; }
    }
  }
}