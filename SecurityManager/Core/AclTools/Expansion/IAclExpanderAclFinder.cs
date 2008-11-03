// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Returns an <see cref="AccessControlList"/> collection. See <see cref="AclExpander"/> ctor. 
  /// </summary>
  public interface IAclExpanderAclFinder
  {
    List<AccessControlList> FindAccessControlLists ();
  }
}