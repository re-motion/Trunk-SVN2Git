using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Remotion.UnitTests.Xml
{
  [XmlType (ElementName, Namespace = SchemaUri)]
  public class SampleClass
  {
    public const string ElementName = "sampleClass";
    public const string SchemaUri = "http://www.re-motion.org/core/unitTests";
  
    public static XmlReader GetSchemaReader ()
    {
      return new XmlTextReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream (typeof (SampleClass), "SampleClass.xsd"));
    }

    private int _value;

    public SampleClass()
    {
    }

    [XmlElement ("value")]
    public int Value
    {
      get { return _value; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("Value", value, "Only positive integer values are allowed.");
         _value = value;
      }
    }
  }
}