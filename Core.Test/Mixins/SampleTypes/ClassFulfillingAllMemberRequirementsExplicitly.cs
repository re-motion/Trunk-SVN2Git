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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (MixinRequiringAllMembersFace))]
  public class ClassFulfillingAllMemberRequirementsExplicitly : IMixinRequiringAllMembersRequirements
  {
    void IMixinRequiringAllMembersRequirements.Method ()
    {
    }

    int IMixinRequiringAllMembersRequirements.Property
    {
      get { return 37; }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    event Func<string> IMixinRequiringAllMembersRequirements.Event
    {
      add { throw new Exception ("The method or operation is not implemented."); }
      remove { throw new Exception ("The method or operation is not implemented."); }
    }
  }
}
