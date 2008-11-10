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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Globalization.Classes;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurableClassDefinitionTreeView : BocTreeView
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public SecurableClassDefinitionTreeView ()
    {
    }

    // methods and properties

    protected override string GetText (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      string text = base.GetText (businessObject);

      SecurableClassDefinition classDefinition = businessObject as SecurableClassDefinition;
      if (classDefinition == null)
        return text;

      if (classDefinition.StatefulAccessControlLists.Count == 0)
        return string.Format (SecurableClassDefinitionTreeViewResources.NoAclsText, text);
      if (classDefinition.StatefulAccessControlLists.Count == 1)
        return string.Format (SecurableClassDefinitionTreeViewResources.SingleAclText, text);
      else
        return string.Format (SecurableClassDefinitionTreeViewResources.MultipleAclsText, text, classDefinition.StatefulAccessControlLists.Count);
    }
  }
}
