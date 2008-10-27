/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Runtime.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain
{
  public abstract class NotSerializableMixinWithISerializable : MixinWithAbstractMembers, ISerializable
  {
    protected NotSerializableMixinWithISerializable ()
    {
    }

    protected NotSerializableMixinWithISerializable (SerializationInfo info, StreamingContext context)
    {
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}