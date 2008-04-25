using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  //TODO: Doc
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ClassIDAttribute : Attribute
  {
    private string _classID;

    public ClassIDAttribute (string classID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
      _classID = classID;
    }

    public string ClassID
    {
      get { return _classID; }
    }
  }
}