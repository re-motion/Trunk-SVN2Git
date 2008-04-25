using System;

namespace Remotion.Implementation
{
  public static class VersionDependentImplementationBridge<T>
  {
    public static readonly T Implementation = GetImplementation();

    private static T GetImplementation ()
    {
      ConcreteImplementationAttribute[] attributes = 
          (ConcreteImplementationAttribute[]) typeof (T).GetCustomAttributes (typeof (ConcreteImplementationAttribute), false);
      if (attributes.Length != 1)
      {
        string message = string.Format ("Cannot get a version-dependent implementation of type '{0}': Expected one " 
            + "ConcreteImplementationAttribute applied to the type, but found {1}.", typeof (T).FullName, attributes.Length);
        throw new InvalidOperationException(message);
      }
      return (T) attributes[0].InstantiateType();
    }
  }
}