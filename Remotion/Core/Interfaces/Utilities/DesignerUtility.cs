// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.ComponentModel.Design;
using Remotion.Design;
using Remotion.Implementation;

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
        CheckDesignMode();
        return s_designModeHelper;
      }
    }

    public static IDesignerHost DesignerHost
    {
      get
      {
        CheckDesignMode();
        return s_designModeHelper.DesignerHost;
      }
    }

    public static bool IsDesignMode
    {
      get { return s_isDesignMode; }
    }

    public static Type GetDesignModeType (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      CheckDesignMode ();

      return DesignerHost.GetType (typeName);
    }

    private static void CheckDesignMode ()
    {
      if (!s_isDesignMode)
        throw new InvalidOperationException ("DesignModeHelper can only be accessed while DesignMode is active.");
    }
  }
}
