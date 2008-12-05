// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
