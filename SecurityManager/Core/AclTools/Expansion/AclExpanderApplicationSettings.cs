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
using Remotion.Text.CommandLine;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplicationSettings
  {
    //public const string UserFirstNamePlaceholder = "first";
    //public const string UserLastNamePlaceholder = "last";
    //public const string UserNamePlaceholder = "user";

    //public string UserFirstNamePlaceholder { get { return _userFirstNamePlaceholder; } }
    //public string UserLastNamePlaceholder { get { return _userLastNamePlaceholder; } }
    //public string UserNamePlaceholder { get { return _userNamePlaceholder; } }


    [CommandLineStringArgument (true, Placeholder = "user", Description = "Fully qualified name of user(s) to query access types for.")]
    public string UserName;

    [CommandLineStringArgument (true, Placeholder = "last", Description = "Last name of user(s) to query access types for.")]
    public string UserLastName;

    [CommandLineStringArgument (true, Placeholder = "first", Description = "First name of user(s) to query access types for.")]
    public string UserFirstName;

    //[CommandLineModeArgumentAttribute (true, Placeholder = "user", Description = "Fully qualified name of user(s) to query access types for.")]
    //public string xxx;
         
    
    [CommandLineFlagArgument ("verbose", false, Description = "Verbose output")]
    public bool Verbose;

  }
}