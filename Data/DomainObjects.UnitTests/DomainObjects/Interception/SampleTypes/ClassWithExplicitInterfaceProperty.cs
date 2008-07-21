/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Interception.SampleTypes
{
  [DBTable]
  public class ClassWithExplicitInterfaceProperty : DomainObject, IPropertyInterface
  {
    public static ClassWithExplicitInterfaceProperty NewObject()
    {
      return NewObject<ClassWithExplicitInterfaceProperty>().With();
    }

    protected ClassWithExplicitInterfaceProperty ()
    {
    }

    int IPropertyInterface.Property
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}
