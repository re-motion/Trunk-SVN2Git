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
using System.IO;

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  /// <summary>
  /// Interface for application classes turned into console applications by "wrapping" them in a 
  /// <see cref="ConsoleApplication{TApplication, TApplicationSettings}"/>.
  /// </summary>
  /// <typeparam name="TApplicationSettings">The settings-class for the for the application. 
  /// Needs to derive from <see cref="ConsoleApplicationSettings"/>.</typeparam>
  public interface IApplicationRunner<TApplicationSettings> where TApplicationSettings : ConsoleApplicationSettings, new()
  {
    void Run (TApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter);
  }
}