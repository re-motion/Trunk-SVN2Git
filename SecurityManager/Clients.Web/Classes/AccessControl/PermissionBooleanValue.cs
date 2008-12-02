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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.Web;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  public class PermissionBooleanValue : BocBooleanValue
  {
    protected override BocBooleanValueResourceSet CreateResourceSet ()
    {
      return new BocBooleanValueResourceSet (
          "Permission",
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (PermissionBooleanValue), ResourceType.Image, "PermissionGranted.gif"),
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (PermissionBooleanValue), ResourceType.Image, "PermissionDenied.gif"),
          ResourceUrlResolver.GetResourceUrl (this, Context, typeof (PermissionBooleanValue), ResourceType.Image, "PermissionUndefined.gif"),
          "Granted",
          "Denied",
          "Undefined"
          );
    }
  }
}