using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private bool _boolean;
    private byte _byte;
    private DateTime _date;
    private DateTime _dateTime;
    private decimal _decimal;
    private double _double;
    private TestEnum _enum;
    private Guid _guid;
    private short _int16;
    private int _int32;
    private long _int64;
    private SimpleBusinessObjectClass _businessObject;
    private float _single;
    private string _string;

    public ClassWithAllDataTypes ()
    {
    }

    public bool Boolean
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    public byte Byte
    {
      get { return _byte; }
      set { _byte = value; }
    }

    [DateProperty]
    public DateTime Date
    {
      get { return _date; }
      set { _date = value; }
    }

    public DateTime DateTime
    {
      get { return _dateTime; }
      set { _dateTime = value; }
    }

    public decimal Decimal
    {
      get { return _decimal; }
      set { _decimal = value; }
    }

    public double Double
    {
      get { return _double; }
      set { _double = value; }
    }

    public TestEnum Enum
    {
      get { return _enum; }
      set { _enum = value; }
    }

    public Guid Guid
    {
      get { return _guid; }
      set { _guid = value; }
    }
    
    public short Int16
    {
      get { return _int16; }
      set { _int16 = value; }
    }

    public int Int32
    {
      get { return _int32; }
      set { _int32 = value; }
    }

    public long Int64
    {
      get { return _int64; }
      set { _int64 = value; }
    }

    public SimpleBusinessObjectClass BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }
 
    public float Single
    {
      get { return _single; }
      set { _single = value; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
 }
}