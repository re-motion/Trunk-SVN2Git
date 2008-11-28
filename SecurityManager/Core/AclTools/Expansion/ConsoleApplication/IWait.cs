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

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  /// <summary>
  /// Interface for classes that support some sort of wait functionality. The <see cref="Wait"/>-method 
  /// returns when the event the class implementing <see cref="IWait"/> occured (e.g. see <see cref="WaitForConsoleKeypress"/>).
  /// </summary>
  // TODO AE: Interface (and class) names should have nouns as names.
  public interface IWait
  {
    void Wait ();
  }
}