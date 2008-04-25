using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class StateCombinationBuilder
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public StateCombinationBuilder ()
    {
    }

    // methods and properties

    public List<StateCombination> CreateAndAttach (SecurableClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      StateCombination stateCombination = StateCombination.NewObject();
      stateCombination.AccessControlList = AccessControlList.NewObject();
      stateCombination.AccessControlList.Class = classDefinition;
      stateCombination.Class = classDefinition;

      List<StateCombination> stateCombinations = new List<StateCombination>();
      stateCombinations.Add (stateCombination);

      return stateCombinations;
    }
  }
}