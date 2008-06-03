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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public abstract class ControllerBase
  {
    protected ControllerBase ()
    {
    }

    protected void AppendImage (ImageList imageList, Enum treeViewIcon)
    {
      Type type = GetType();
      string resourceID = EnumDescription.GetDescription (treeViewIcon);
      Stream stream = type.Assembly.GetManifestResourceStream (type, resourceID);
      Assertion.IsNotNull (stream, string.Format ("Resource '{0}' was not found in namespace '{1}'.", resourceID, type.Namespace));
      try
      {
        imageList.Images.Add (treeViewIcon.ToString (), Image.FromStream (stream));
      }
      catch
      {
        stream.Close();
        throw;
      }
    }

    protected ImageList CreateImageList (params Enum[] resourceEnums)
    {
      ImageList imageList = new ImageList ();
      imageList.TransparentColor = Color.Magenta;
      try
      {
        foreach (Enum enumValue in resourceEnums)
          AppendImage (imageList, enumValue);

        return imageList;
      }
      catch
      {
        imageList.Dispose();
        throw;
      }
    }
  }
}
