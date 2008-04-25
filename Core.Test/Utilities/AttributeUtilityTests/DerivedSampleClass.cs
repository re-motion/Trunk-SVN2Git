namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  public class DerivedSampleClass : SampleClass
  {
    public override string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    protected override string ProtectedPropertyWithAttribute
    {
      get { return null; }
    }

    [Multiple]
    public override string PropertyWithMultipleAttribute
    {
      get { return null; }
    }
  }
}