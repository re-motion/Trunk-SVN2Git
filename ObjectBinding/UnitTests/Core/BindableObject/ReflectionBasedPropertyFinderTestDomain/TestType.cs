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
  public class TestType : BaseTestType
  {
    public static int PublicStaticProperty { get { return 0; } }
    protected static int ProtectedStaticProperty { get { return 0; } }

    public int PublicInstanceProperty { get { return 0; } }
    protected int ProtectedInstanceProperty { get { return 0; } }
  }
}