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

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class NullEndPointModification : RelationEndPointModification
  {
    public NullEndPointModification (RelationEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
        : base (affectedEndPoint, oldEndPoint, newEndPoint)
    {
    }

    public override void Begin ()
    {
      // do nothing
    }

    public override void Perform ()
    {
      // do nothing
    }

    public override void End ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfBegin ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfEnd ()
    {
      // do nothing
    }
  }
}
