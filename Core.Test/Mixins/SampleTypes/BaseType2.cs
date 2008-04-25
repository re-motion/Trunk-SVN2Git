using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType2
  {
    string IfcMethod ();
  }
  
  [Serializable]
  public class BaseType2 : IBaseType2, ISerializable
  {
    public string S;

    public BaseType2 ()
    {
    }

    public BaseType2 (SerializationInfo info, StreamingContext context)
    {
      S = info.GetString ("S");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("S", S);
    }

    public string IfcMethod ()
    {
      return "BaseType2.IfcMethod";
    }
  }
}
