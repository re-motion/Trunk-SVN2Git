using System;
using System.Collections.Generic;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class GenericClassWithConstraints<TConcreteInterfaceConstraint, TConcreteClassConstraint, TStructConstraint, TClassNewConstraint,
      TSelfConstraint, TComplexSelfConstraint>
      where TConcreteInterfaceConstraint : ICloneable
      where TConcreteClassConstraint : List<string>
      where TStructConstraint : struct
      where TClassNewConstraint : class, new ()
      where TSelfConstraint : TConcreteInterfaceConstraint
      where TComplexSelfConstraint : List<List<TSelfConstraint[]>>
  {
  }
}