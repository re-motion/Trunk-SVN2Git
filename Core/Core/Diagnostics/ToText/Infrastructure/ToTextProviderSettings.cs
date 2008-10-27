// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
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