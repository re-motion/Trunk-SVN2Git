/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */
namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReflectionBasedPropertyFinderTestDomain
{
  public class BaseTestType
  {
    public static int BasePublicStaticProperty { get { return 0; } }
    protected static int BaseProtectedStaticProperty { get { return 0; } }

    public virtual int BasePublicInstanceProperty { get { return 0; } }
    protected int BaseProtectedInstanceProperty { get { return 0; } }
  }
}