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
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Provider;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>Base class for all providers.</summary>
  /// <remarks>
  /// <see cref="ExtendedProviderBase"/> changes the protocoll for initializing a configuration provider from using a default constructor
  /// followed by a call to <see cref="Initialize"/> to initialize the provider during construction.
  /// </remarks>
  public abstract class ExtendedProviderBase: ProviderBase, ISerializable
  {
    private static void SerializeNameValueCollection (SerializationInfo info, string key, NameValueCollection collection)
    {
      info.AddValue (key + ".Count", collection.Count);

      for (int i = 0; i < collection.Count; ++i)
      {
        info.AddValue (key + ".Keys[" + i + "]", collection.Keys[i]);
        info.AddValue (key + "[" + i + "]", collection[i]);
      }
    }

    private static NameValueCollection DeserializeNameValueCollection (SerializationInfo info, string key)
    {
      int count = info.GetInt32 (key + ".Count");
      NameValueCollection result = new NameValueCollection (count);

      for (int i = 0; i < count; ++i)
      {
        string itemKey = info.GetString (key + ".Keys[" + i + "]");
        string itemValue = info.GetString (key + "[" + i + "]");
        result.Add (itemKey, itemValue);
      }
      return result;
    }

    private readonly NameValueCollection _config;

    /// <summary>Initializes a new instance of the <see cref="ExtendedProviderBase"/>.</summary>
    /// <param name="name">The friendly name of the provider. Must not be <see langword="null" /> or empty.</param>
    /// <param name="config">
    /// A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
    /// Must not be <see langword="null" />.
    /// </param>
    protected ExtendedProviderBase (string name, NameValueCollection config)
    {
      NameValueCollection configClone = new NameValueCollection (config);
      Initialize (name, config);
      _config = configClone;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedProviderBase"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> provided by the serialization engine.</param>
    /// <param name="context">The <see cref="StreamingContext"/> provided by the serialization engine.</param>
    protected ExtendedProviderBase (SerializationInfo info, StreamingContext context)
        : this (ArgumentUtility.CheckNotNull ("info", info).GetString ("Name"), DeserializeNameValueCollection (info, "Config"))
    {
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public override sealed void Initialize (string name, NameValueCollection config)
    {
      base.Initialize (name, config);
      Assertion.IsNull (_config, "Initialize can only succeed when called for the first time, from the constructor");
    }

    protected NameValueCollection Config
    {
      get { return _config; }
    }

    protected string GetAndRemoveNonEmptyStringAttribute (NameValueCollection config, string attribute, string providerName, bool required)
    {
      ArgumentUtility.CheckNotNull ("config", config);
      ArgumentUtility.CheckNotNullOrEmpty ("attribute", attribute);
      ArgumentUtility.CheckNotNullOrEmpty ("providerName", providerName);

      string value = config.Get (attribute);
      if ((value == null && required) || (value != null && value.Length == 0))
      {
        throw new ConfigurationErrorsException (
            string.Format ("The attribute '{0}' is missing in the configuration of the '{1}' provider.", attribute, providerName));
      }
      config.Remove (attribute);
      
      return value;
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      GetObjectData (info, context);
    }

    /// <summary>
    /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
    /// <remarks>Override this method (and call the base implementation) to supply additional data for the serialization process.</remarks>
    protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("Name", Name);
      SerializeNameValueCollection (info, "Config", Config);
    }
  }
}
