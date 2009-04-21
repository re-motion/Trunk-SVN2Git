// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Xml.Serialization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  [XmlRoot ("Person")]
  [XmlType]
  [Serializable]
  public class Person : BindableXmlObject
  {
    public static Person GetObject (Guid id)
    {
      return GetObject<Person> (id);
    }

    public static Person CreateObject ()
    {
      return CreateObject<Person>();
    }

    public static Person CreateObject (Guid id)
    {
      return CreateObject<Person> (id);
    }

    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private byte _height;
    private decimal? _income = 1000.50m;
    private Gender _gender;
    private MarriageStatus _marriageStatus;
    private DateTime _dateOfDeath;
    private bool _deceased = false;
    private string[] _cv;
    private Guid _partnerID;
    private Guid[] _childIDs;
    private Guid[] _jobIDs;

    protected Person ()
    {
    }

    [XmlAttribute]
    public string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    [XmlAttribute]
    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    [XmlAttribute]
    public DateTime DateOfBirth
    {
      get { return _dateOfBirth; }
      set { _dateOfBirth = value; }
    }

    [XmlAttribute]
    public byte Height
    {
      get { return _height; }
      set { _height = value; }
    }

    [XmlElement]
    public decimal? Income
    {
      get { return _income; }
      set { _income = value; }
    }

    [XmlAttribute]
    [DisableEnumValues(Gender.UnknownGender)]
    public Gender Gender
    {
      get { return _gender; }
      set { _gender = value; }
    }

    [XmlAttribute]
    [DisableEnumValues (MarriageStatus.Bigamist, MarriageStatus.Polygamist)]
    public MarriageStatus MarriageStatus
    {
      get { return _marriageStatus; }
      set { _marriageStatus = value; }
    }

    [XmlElement]
    [ObjectBinding (Visible = false)]
    public Guid PartnerID
    {
      get { return _partnerID; }
      set { _partnerID = value; }
    }

    [XmlIgnore]
    public Person Partner
    {
      get { return (_partnerID != Guid.Empty) ? Person.GetObject (_partnerID) : null; }
      set { _partnerID = (value != null) ? value.ID : Guid.Empty; }
    }

    [XmlElement]
    [ObjectBinding (Visible = false)]
    public Guid[] ChildIDs
    {
      get { return _childIDs; }
      set { _childIDs = value; }
    }

    [XmlIgnore]
    public Person[] Children
    {
      get
      {
        if (_childIDs == null)
          return new Person[0];

        Person[] children = new Person[_childIDs.Length];
        for (int i = 0; i < _childIDs.Length; i++)
          children[i] = Person.GetObject (_childIDs[i]);

        return children;
      }
      set
      {
        if (value != null)
        {
          ArgumentUtility.CheckNotNullOrItemsNull ("value", value);
          _childIDs = new Guid[value.Length];
          for (int i = 0; i < value.Length; i++)
            _childIDs[i] = value[i].ID;
        }
        else
        {
          _childIDs = new Guid[0];
        }
      }
    }

    [XmlIgnore]
    public BindableXmlObject[] ChildrenAsObjects
    {
      get
      {
        return Children;
      }
      set
      {
        Children = (Person[]) value;
      }
    }

    [XmlElement]
    [ObjectBinding (Visible = false)]
    public Guid[] JobIDs
    {
      get { return _jobIDs; }
      set { _jobIDs = value; }
    }

    [XmlIgnore]
    public Job[] Jobs
    {
      get
      {
        if (_jobIDs == null)
          return new Job[0];

        Job[] jobs = new Job[_jobIDs.Length];
        for (int i = 0; i < _jobIDs.Length; i++)
          jobs[i] = Job.GetObject (_jobIDs[i]);

        return jobs;
      }
      set
      {
        if (value != null)
        {
          ArgumentUtility.CheckNotNullOrItemsNull ("value", value);
          _jobIDs = new Guid[value.Length];
          for (int i = 0; i < value.Length; i++)
            _jobIDs[i] = value[i].ID;
        }
        else
        {
          _jobIDs = new Guid[0];
        }
      }
    }

    [XmlAttribute (DataType="date")]
    [DateProperty]
    public DateTime DateOfDeath
    {
      get { return _dateOfDeath; }
      set { _dateOfDeath = value; }
    }

    [XmlElement]
    public bool Deceased
    {
      get { return _deceased; }
      set { _deceased = value; }
    }

    [XmlElement]
    public string[] CV
    {
      get { return _cv; }
      set { _cv = value; }
    }

    public string CVString
    {
      get
      {
        if (_cv == null)
          return null;
        return string.Join ("<br/>", _cv);
      }
    }

    public override string DisplayName
    {
      get { return LastName + ", " + FirstName; }
    }

    public override string ToString ()
    {
      return DisplayName;
    }
  }

  public enum Gender
  {
    Male,
    Female,
    UnknownGender
  }

  public enum MarriageStatus
  {
    Married,
    Single,
    Divorced,
    Bigamist,
    Polygamist,
  }
}
