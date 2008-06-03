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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithListProperties
  {
    private SimpleReferenceType[] _array;
    private SimpleReferenceType[] _readOnlyArray = new SimpleReferenceType[0];
    private SimpleReferenceType[] _readOnlyAttributeArray;
    private List<SimpleReferenceType> _listOfT = new List<SimpleReferenceType> ();
    private List<SimpleReferenceType> _readOnlyListOfT = new List<SimpleReferenceType> ();
    private ArrayList _arrayList = new ArrayList ();
    private ArrayList _readOnlyArrayList = new ArrayList ();

    public ClassWithListProperties ()
    {
    }

    public SimpleReferenceType[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public SimpleReferenceType[] ReadOnlyArray
    {
      get { return _readOnlyArray; }
    }

    [ObjectBinding (ReadOnly = true)]
    public SimpleReferenceType[] ReadOnlyAttributeArray
    {
      get { return _readOnlyAttributeArray; }
      set { _readOnlyAttributeArray = value; }
    }

    public List<SimpleReferenceType> ListOfT
    {
      get { return _listOfT; }
    }

    [ObjectBinding (ReadOnly = true)]
    public List<SimpleReferenceType> ReadOnlyListOfT
    {
      get { return _readOnlyListOfT; }
    }

    public ReadOnlyCollection<SimpleReferenceType> ReadOnlyCollectionOfT
    {
      get { return _listOfT.AsReadOnly(); }
    }

    public ReadOnlyCollection<SimpleReferenceType> ReadOnlyCollectionOfTWithSetter
    {
      get { return _listOfT.AsReadOnly (); }
      set { ; }
    }
    
    [ItemType (typeof (SimpleReferenceType))]
    public ArrayList ArrayList
    {
      get { return _arrayList; }
    }

    [ObjectBinding (ReadOnly = true)]
    [ItemType (typeof (SimpleReferenceType))]
    public ArrayList ReadOnlyArrayList
    {
      get { return _readOnlyArrayList; }
    }
  }
}
