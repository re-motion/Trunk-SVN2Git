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
using System.ComponentModel.Design;

namespace Remotion.Web.UI.Design
{
public class AdvancedCollectionEditor: CollectionEditor
{
  public AdvancedCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    throw new NotImplementedException ("CreateNewItemTypes must be overridden in derived class.");
    //return new Type[] {typeof (...)};
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    return EditValue (context, provider, value, 800, 500, 4);
  }

  protected object EditValue (
      ITypeDescriptorContext context,
      IServiceProvider provider, 
      object value,
      int editorWidth, 
      int editorHeight, 
      double propertyGridLabelRatio)
  {
    IServiceProvider collectionEditorServiceProvider = null;
    if (provider.GetType() != typeof (CollectionEditorServiceProvider))
    {  
      collectionEditorServiceProvider = new CollectionEditorServiceProvider (
          provider, context.PropertyDescriptor.DisplayName, editorWidth, editorHeight, propertyGridLabelRatio);
    }
    else
    {
      collectionEditorServiceProvider = provider;
    }
    return base.EditValue (context, collectionEditorServiceProvider, value);
  }
}

}
