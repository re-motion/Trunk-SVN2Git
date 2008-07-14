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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Remotion.Mixins;
using Remotion.ObjectBinding.Design.BindableObject;
using TypeUtility=Remotion.Utilities.TypeUtility;

namespace Remotion.ObjectBinding.Web.UI.Design
{
  public class BindableObjectDataSourceDesigner : BocDataSourceDesigner
  {
    public BindableObjectDataSourceDesigner ()
    {
    }

    public override string GetDesignTimeHtml ()
    {
      return CreatePlaceHolderDesignTimeHtml();
    }

    public override void Initialize (System.ComponentModel.IComponent component)
    {
      base.Initialize (component);

      BindableObjectTypeFinder typeFinder = new BindableObjectTypeFinder (component.Site);
      MixinConfiguration mixinConfiguration = typeFinder.GetMixinConfiguration (false);
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      IPropertyValueUIService propertyValueUIService = (IPropertyValueUIService) component.Site.GetService (typeof (IPropertyValueUIService));
      propertyValueUIService.AddPropertyValueUIHandler (PropertyValueUIHandler);
    }

    private void PropertyValueUIHandler (ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList)
    {
      if (propDesc.DisplayName == "TypeName")
      {
        string value = propDesc.GetValue (Component) as string;
        if (!IsValidTypeName (value))
        {
          Image image = new Bitmap (8, 8);
          using (Graphics g = Graphics.FromImage (image))
          {
            g.FillEllipse (Brushes.Red, 0, 0, 8, 8);
          }
          valueUIItemList.Add (
              new PropertyValueUIItem (
                  image, delegate { }, string.Format ("Could not load type '{0}'.", TypeUtility.ParseAbbreviatedTypeName (value))));
        }
      }
    }

    private bool IsValidTypeName (string value)
    {
      if (string.IsNullOrEmpty (value))
        return true;
      if (TypeUtility.GetDesignModeType (value, false) == null)
        return false;
      return true;
    }
  }
}
