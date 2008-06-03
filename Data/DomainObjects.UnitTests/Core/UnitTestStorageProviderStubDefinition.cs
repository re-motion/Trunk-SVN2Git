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
using System.Xml;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core
{
  public class UnitTestStorageProviderStubDefinition : StorageProviderDefinition
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public UnitTestStorageProviderStubDefinition (
        string storageProviderID,
        Type storageProviderType)
        : base (storageProviderID, storageProviderType)
    {
    }

    public UnitTestStorageProviderStubDefinition (
        string storageProviderID,
        Type storageProviderType,
        XmlNode configurationNode)
        : base (storageProviderID, storageProviderType)
    {
    }

    // methods and properties

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull ("identityType", identityType);

      // Note: UnitTestStorageProviderStubDefinition supports all identity types for testing purposes.
      return true;
    }

  }
}
