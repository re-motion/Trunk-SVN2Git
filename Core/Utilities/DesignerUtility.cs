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
using System.ComponentModel.Design;
using Remotion.Design;

namespace Remotion.Utilities
{
  //TODO: doc
  public static class DesignerUtility
  {
    private static bool s_isDesignMode;
    private static IDesignModeHelper s_designModeHelper;

    public static void SetDesignMode (IDesignModeHelper designModeHelper)
    {
      ArgumentUtility.CheckNotNull ("designModeHelper", designModeHelper);

      s_designModeHelper = designModeHelper;
      s_isDesignMode = true;
    }

    public static void ClearDesignMode ()
    {
      s_designModeHelper = null;
      s_isDesignMode = false;
    }

    public static IDesignModeHelper DesignModeHelper
    {
      get
      {
        if (!s_isDesignMode)
          throw new InvalidOperationException ("DesignModeHelper can only be accessed while DesignMode is active.");

        return s_designModeHelper;
      }
    }

    public static IDesignerHost DesignerHost
    {
      get
      {
        if (!s_isDesignMode)
          throw new InvalidOperationException ("DesignerHost can only be accessed while DesignMode is active.");

        return s_designModeHelper.DesignerHost;
      }
    }

    public static bool IsDesignMode
    {
      get { return s_isDesignMode; }
    }
  }
}
