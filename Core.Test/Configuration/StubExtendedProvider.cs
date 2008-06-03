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
using System.Collections.Specialized;
using Remotion.Configuration;

namespace Remotion.UnitTests.Configuration
{
  public class StubExtendedProvider: ExtendedProviderBase
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing


    public StubExtendedProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public new string GetAndRemoveNonEmptyStringAttribute (NameValueCollection config, string attribute, string providerName, bool required)
    {
     return base.GetAndRemoveNonEmptyStringAttribute (config, attribute, providerName, required);
    }
  }
}
