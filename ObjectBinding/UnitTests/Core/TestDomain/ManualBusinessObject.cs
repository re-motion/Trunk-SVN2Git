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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class ManualBusinessObject : IBusinessObject
  {
    private string _stringProperty = "Initial value";

    public string StringProperty
    {
      get { return _stringProperty; }
      set { _stringProperty = value; }
    }

    public object GetProperty (IBusinessObjectProperty property)
    {
      if (property.Identifier == "StringProperty")
        return StringProperty;
      else
        throw new NotSupportedException ();
    }

    public void SetProperty (IBusinessObjectProperty property, object value)
    {
      if (property.Identifier == "StringProperty")
        StringProperty = (string) value;
      else
        throw new NotSupportedException ();
    }

    public string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return GetProperty (property).ToString ();
    }

    public string DisplayName
    {
      get { return "Manual business object"; }
    }

    public string DisplayNameSafe
    {
      get { return "Manual business object (safe)"; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return BindableObjectProvider.GetBindableObjectClass(typeof (ManualBusinessObject)); }
    }
  }
}
