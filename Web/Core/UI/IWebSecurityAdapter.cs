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
using Remotion.Security;
using System.Web.UI;

namespace Remotion.Web.UI
{
  //TODO FS: Move to SecurityInterfaces
  //verwendet in web-controls um security abfragen zu tun.
  public interface IWebSecurityAdapter : ISecurityAdapter
  {
    //verwendet fuer buttons etc, secObj = isntanz fur die sec gecheckt wird. handler ist eventhandler von butonclock etc der geschuetz werden soll.
    bool HasAccess (ISecurableObject securableObject, Delegate handler);
    //bool HasStatelessAccess (Type functionType);
    //void CheckAccess (ISecurableObject securableObject, Delegate handler);
  }
}
