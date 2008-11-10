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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  public abstract class StatelessAccessControlList : AccessControlList
  {
    public static StatelessAccessControlList NewObject ()
    {
      return NewObject<StatelessAccessControlList> ().With ();
    }

    public new static StatelessAccessControlList GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StatelessAccessControlList> (id);
    }

    protected StatelessAccessControlList ()
    {
    }

    [DBBidirectionalRelation ("StatelessAccessControlList", ContainsForeignKey = true)]
    [DBColumn ("StatelessAcl_ClassID")]
    [Mandatory]
    protected abstract SecurableClassDefinition MyClass { get; set; }

    public override SecurableClassDefinition Class
    {
      get { return MyClass; }
      set { MyClass = value; }
    }
  }
}