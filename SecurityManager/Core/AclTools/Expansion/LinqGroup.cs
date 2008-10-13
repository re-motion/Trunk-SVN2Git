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
using System.Linq;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class LinqGroup<TKey, Tentry>
  {
    public LinqGroup (IGrouping<TKey, Tentry> items)
    {
      Items = items;
    }

    public TKey Key
    {
      get { return Items.Key; }
    }
    public IGrouping<TKey, Tentry> Items { get; private set; }
  }
}

// TODO: Move to object mothers
namespace ObjectMother
{
  public class LinqGroup
  {
    public static Remotion.SecurityManager.AclTools.Expansion.LinqGroup<TKey, Tentry> New<TKey, Tentry> (IGrouping<TKey, Tentry> items)
    {
      return new Remotion.SecurityManager.AclTools.Expansion.LinqGroup<TKey, Tentry> (items);
    }
  }
}