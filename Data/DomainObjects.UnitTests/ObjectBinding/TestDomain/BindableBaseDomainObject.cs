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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  [BindableDomainObject]
  public abstract class BindableBaseDomainObject : DomainObject
  {
    [StringProperty (MaximumLength = 3)]
    public virtual string BasePropertyWithMaxLength3
    {
      get { return CurrentProperty.GetValue<string> (); }
      set { CurrentProperty.SetValue (value); }
    }

    [StringProperty (MaximumLength = 4)]
    public virtual string BasePropertyWithMaxLength4
    {
      get { return CurrentProperty.GetValue<string> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}
