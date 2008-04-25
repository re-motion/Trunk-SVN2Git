using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class GuidProperty : PropertyBase, IBusinessObjectStringProperty
  {
    public GuidProperty (Parameters parameters)
        : base (parameters)
    {
    }

    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    public int? MaxLength
    {
      get { return 38; }
    }

    public override object ConvertFromNativePropertyType (object nativeValue)
    {
      if (IsList)
        return ConvertFromGuidListToStringArray (nativeValue);
      else
        return ConvertFromGuidToString (nativeValue);
    }

    public override object ConvertToNativePropertyType (object publicValue)
    {
      if (IsList)
        return ConvertFromStringListToGuidList (publicValue);
      else
        return ConvertFromStringToGuid (publicValue);
    }

    private string ConvertFromGuidToString (object nativeValue)
    {
      Guid? guid = ArgumentUtility.CheckType<Guid?> ("nativeValue", nativeValue);
      if (guid == null)
        return null;
      return guid.ToString();
    }

    private string[] ConvertFromGuidListToStringArray (object nativeValue)
    {
      if (nativeValue == null)
        return null;
      IList nativeValueList = ArgumentUtility.CheckType<IList> ("nativeValue", nativeValue);
      string[] publicValueList = new string[nativeValueList.Count];
      for (int i = 0; i < nativeValueList.Count; i++)
        publicValueList[i] = ConvertFromGuidToString (nativeValueList[i]);
      return publicValueList;
    }

    private Guid? ConvertFromStringToGuid (object publicValue)
    {
      string stringValue = ArgumentUtility.CheckType<string> ("publicValue", publicValue);
      if (stringValue == null)
        return null;
      if (stringValue == string.Empty)
        return Guid.Empty;
      return new Guid (stringValue);
    }

    private IList ConvertFromStringListToGuidList (object publicValue)
    {
      if (publicValue == null)
        return null;
      IList publicValueList = ArgumentUtility.CheckType<IList> ("publicValue", publicValue);
      IList nativeValueList = ListInfo.CreateList (publicValueList.Count);
      for (int i = 0; i < publicValueList.Count; i++)
        nativeValueList[i] = ConvertFromStringToGuid (publicValueList[i]);
      return nativeValueList;
    }

  }
}