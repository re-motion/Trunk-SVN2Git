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
  /// Class implementing <see cref="IWait"/> to wait for a keypress on the console.
  /// The <see cref="Wait"/>-method returns after a console key has been pressed.
  /// </summary>
  public class WaitForConsoleKeypress : IWait
  {
    public void Wait ()
    {
      Console.ReadKey ();
    }
  }
}