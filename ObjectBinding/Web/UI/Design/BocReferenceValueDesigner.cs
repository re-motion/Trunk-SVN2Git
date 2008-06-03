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
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{

public class BocReferenceValueDesigner: WebControlDesigner
{
  public override void OnComponentChanged(object sender, System.ComponentModel.Design.ComponentChangedEventArgs ce)
  {
    base.OnComponentChanged (sender, ce);
    if (ce.Member.Name == "Command")
    {
      PropertyDescriptor persistedCommand = TypeDescriptor.GetProperties (Component)["PersistedCommand"];
      RaiseComponentChanged (persistedCommand, null, ((BocReferenceValue) Component).PersistedCommand);
    }
  }
}

}
