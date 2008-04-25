using System;
using System.Xml;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
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
