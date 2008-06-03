/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
