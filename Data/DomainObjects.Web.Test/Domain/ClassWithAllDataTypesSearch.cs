// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [BindableObject]
  public class ClassWithAllDataTypesSearch
  {
    private string _stringProperty;
    private byte? _bytePropertyFrom;
    private byte? _bytePropertyTo;
    private ClassWithAllDataTypes.EnumType _enumProperty = ClassWithAllDataTypes.EnumType.Value1;
    private DateTime? _datePropertyFrom;
    private DateTime? _datePropertyTo;
    private DateTime? _dateTimePropertyFrom;
    private DateTime? _dateTimePropertyTo;

    public string StringProperty
    {
      get { return _stringProperty; }
      set { _stringProperty = value; }
    }

    public byte? BytePropertyFrom
    {
      get { return _bytePropertyFrom; }
      set { _bytePropertyFrom = value; }
    }

    public byte? BytePropertyTo
    {
      get { return _bytePropertyTo; }
      set { _bytePropertyTo = value; }
    }

    public ClassWithAllDataTypes.EnumType EnumProperty
    {
      get { return _enumProperty; }
      set { _enumProperty = value; }
    }

    [DateProperty]
    public DateTime? DatePropertyFrom
    {
      get { return _datePropertyFrom; }
      set { _datePropertyFrom = value; }
    }

    [DateProperty]
    public DateTime? DatePropertyTo
    {
      get { return _datePropertyTo; }
      set { _datePropertyTo = value; }
    }

    public DateTime? DateTimePropertyFrom
    {
      get { return _dateTimePropertyFrom; }
      set { _dateTimePropertyFrom = value; }
    }

    public DateTime? DateTimePropertyTo
    {
      get { return _dateTimePropertyTo; }
      set { _dateTimePropertyTo = value; }
    }

    public IQuery CreateQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithAllDataTypes");

      query.Parameters.Add ("@stringProperty", _stringProperty);
      query.Parameters.Add ("@bytePropertyFrom", _bytePropertyFrom);
      query.Parameters.Add ("@bytePropertyTo", _bytePropertyTo);
      query.Parameters.Add ("@enumProperty", _enumProperty);
      query.Parameters.Add ("@datePropertyFrom", _datePropertyFrom);
      query.Parameters.Add ("@datePropertyTo", _datePropertyTo);
      query.Parameters.Add ("@dateTimePropertyFrom", _dateTimePropertyFrom);
      query.Parameters.Add ("@dateTimePropertyTo", _dateTimePropertyTo);

      return query;
    }
  }
}
