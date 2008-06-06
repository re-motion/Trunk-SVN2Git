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
using Remotion.Utilities;

namespace Remotion.Design
{
  /// <summary>
  /// Base implementation of the <see cref="IDesignModeHelper"/> interface.
  /// </summary>
  public abstract class DesignModeHelperBase : IDesignModeHelper
  {
    private readonly IDesignerHost _designerHost;

    protected DesignModeHelperBase (IDesignerHost designerHost)
    {
      ArgumentUtility.CheckNotNull ("designerHost", designerHost);

      _designerHost = designerHost;
    }

    public abstract string GetProjectPath ();

    public abstract System.Configuration.Configuration GetConfiguration ();

    public IDesignerHost DesignerHost
    {
      get { return _designerHost; }
    }
  }
}
