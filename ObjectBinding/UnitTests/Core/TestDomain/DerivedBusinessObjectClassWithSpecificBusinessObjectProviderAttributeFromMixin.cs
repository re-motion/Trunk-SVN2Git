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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [Uses (typeof (MixnWithProviderAttribute))]
  [Serializable]
  public class DerivedBusinessObjectClassWithSpecificBusinessObjectProviderAttributeFromMixin : BaseBusinessObjectClass
  {
    [BindableObjectWithIdentityProvider]
    [SuppressAttributes (typeof (BusinessObjectProviderAttribute))]
    public class MixnWithProviderAttribute
    { }

    private string _public;

    public DerivedBusinessObjectClassWithSpecificBusinessObjectProviderAttributeFromMixin ()
    {
    }

    public new string Public
    {
      get { return _public; }
      set { _public = value; }
    }

    private string Private
    {
      get { return null; }
      set { }
    }
  }
}