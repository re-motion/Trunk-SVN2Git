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

namespace Remotion.Web.UI.Controls
{

public interface IEditableControl: IControl
{
  /// <summary>
  ///   Specifies whether the value of the control has been changed on the Client since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Resetting <c>IsDirty</c> during saving is not implemented by all controls.
  /// </remarks>
  // TODO: redesign IsDirty semantics!
  bool IsDirty { get; set; }
  string[] GetTrackedClientIDs();
}

}
