/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.ComponentModel;
using System.ComponentModel.Design;

namespace Remotion.Design
{
  /// <summary>
  /// The <see cref="IDesignModeHelper"/> interface defines methods to encapsulate the access to various design-mode properties of a project.
  /// It is intended to be used by components offering design-time support.
  /// </summary>
  public interface IDesignModeHelper
  {
    IDesignerHost DesignerHost { get; }
    string GetProjectPath();
    System.Configuration.Configuration GetConfiguration();
  }
}
