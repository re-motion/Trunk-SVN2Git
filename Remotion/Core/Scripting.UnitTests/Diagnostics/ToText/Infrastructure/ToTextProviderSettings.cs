// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  /// <summary>
  /// Settings class for the <see cref="ToTextProvider"/> class.
  /// </summary>
  public class ToTextProviderSettings
  {
    public ToTextProviderSettings ()
    {
      UseAutomaticObjectToText = true;
      EmitPublicProperties = true;
      EmitPublicFields = true;
      EmitPrivateProperties = true;
      EmitPrivateFields = true;

      UseAutomaticStringEnclosing = true;
      UseAutomaticCharEnclosing = true;

      UseInterfaceHandlers = true;

      //ParentHandlerSearchDepth = 0;
      //ParentHandlerSearchUpToRoot = false;
      //UseParentHandlers = false;

      ParentHandlerSearchDepth = 0;
      ParentHandlerSearchUpToRoot = true;
      UseParentHandlers = true;
    }

    public bool UseAutomaticObjectToText { get; set; }
    public bool EmitPublicProperties { get; set; }
    public bool EmitPublicFields { get; set; }
    public bool EmitPrivateProperties { get; set; }
    public bool EmitPrivateFields { get; set; }

    public bool UseAutomaticStringEnclosing { get; set; }
    public bool UseAutomaticCharEnclosing { get; set; }

    public int ParentHandlerSearchDepth { get; set; }
    public bool ParentHandlerSearchUpToRoot { get; set; }
    public bool UseParentHandlers { get; set; }

    public bool UseInterfaceHandlers { get; set; }

    public void SetAutomaticObjectToTextEmit (bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      EmitPublicProperties = emitPublicProperties;
      EmitPublicFields = emitPublicFields;
      EmitPrivateProperties = emitPrivateProperties;
      EmitPrivateFields = emitPrivateFields;
    }    
  }
}
