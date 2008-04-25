namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class ClassWithExplicitInterfaceProperty : DomainObject, IPropertyInterface
  {
    public static ClassWithExplicitInterfaceProperty NewObject()
    {
      return DomainObject.NewObject<ClassWithExplicitInterfaceProperty>().With();
    }

    protected ClassWithExplicitInterfaceProperty ()
    {
    }

    int IPropertyInterface.Property
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}