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
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Persistence
{
[Serializable]
public class StorageProviderException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderException () : this ("A storage provider exception occurred.") {}
  public StorageProviderException (string message) : base (message) {}
  public StorageProviderException (string message, Exception inner) : base (message, inner) {}
  protected StorageProviderException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
