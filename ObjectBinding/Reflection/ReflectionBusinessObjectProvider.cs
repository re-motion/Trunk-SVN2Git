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
using System.Collections;
using Remotion.Collections;
using Remotion.ObjectBinding.Web;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

public class ReflectionBusinessObjectProvider: BusinessObjectProvider
{
  private readonly static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  private ReflectionBusinessObjectProvider()
  {
    _serviceCache.Add (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService ());
  }

  private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService> ();

  protected override ICache<Type, IBusinessObjectService> ServiceCache
  {
    get { return _serviceCache; }
  }

}

}
