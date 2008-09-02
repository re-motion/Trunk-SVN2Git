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
using System.ComponentModel.Design;
using Remotion.Reflection;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextSpecificHandlerAutoRegistration
  {

    private ITypeDiscoveryService _typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
      new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, false));

    public void FindAndRegister ()
    {
      const bool excludeGlobalTypes = true;
      //throw new NotImplementedException ();
      ICollection types = _typeDiscoveryService.GetTypes (typeof(IToTextSpecificTypeHandler), excludeGlobalTypes);
    }    

  }
}