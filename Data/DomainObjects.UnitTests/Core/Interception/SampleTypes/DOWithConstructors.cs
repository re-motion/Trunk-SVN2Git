namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [DBTable]
  public class DOWithConstructors : DomainObject
  {
    public readonly string FirstArg;
    public readonly string SecondArg;

    public DOWithConstructors (string firstArg, string secondArg)
    {
      FirstArg = firstArg;
      SecondArg = secondArg;
    }

    public DOWithConstructors (int arg)
        : this (arg.ToString(), null)
    {
    }
  }
}