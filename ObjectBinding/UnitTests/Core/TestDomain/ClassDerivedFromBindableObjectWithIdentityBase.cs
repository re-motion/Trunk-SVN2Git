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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [Serializable]
  public class ClassDerivedFromBindableObjectWithIdentityBase : BindableObjectWithIdentityBase
  {
    private string _uniqueIdentifier;

    public ClassDerivedFromBindableObjectWithIdentityBase()
    {
    }

    public ClassDerivedFromBindableObjectWithIdentityBase(IBusinessObjectWithIdentity businessObjectImplementation) : base(businessObjectImplementation)
    {
    }

    public string String { get; set; }

    public override string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public void SetUniqueIdentifier (string newIdentifier)
    {
      _uniqueIdentifier = newIdentifier;
    }
  }
}