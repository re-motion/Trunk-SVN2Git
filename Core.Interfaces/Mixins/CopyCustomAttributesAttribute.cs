using System;
using System.Reflection;
using System.Collections;

namespace Remotion.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies a class whose custom attributes should be added to the mixin's target class. This is useful when a mixin
  /// should add certain attributes without itself exposing those attributes.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class CopyCustomAttributesAttribute : Attribute
  {
    private readonly Type _attributeSourceType;
    private readonly string _attributeSourceMemberName;
    
    private Type[] _copiedAttributeTypes;

    // For CLS compatibility...
    public CopyCustomAttributesAttribute (Type attributeSourceType)
      : this (attributeSourceType, Type.EmptyTypes)
    {
    }

    // For CLS compatibility...
    public CopyCustomAttributesAttribute (Type attributeSourceType, string attributeSourceMemberName)
      : this (attributeSourceType, attributeSourceMemberName, Type.EmptyTypes)
    {
    }

    public CopyCustomAttributesAttribute (Type attributeSourceType, params Type[] copiedAttributeTypes)
    {
      if (attributeSourceType == null)
        throw new ArgumentNullException ("attributeSourceType");
      if (copiedAttributeTypes == null)
        throw new ArgumentNullException ("copiedAttributeTypes");

      _attributeSourceType = attributeSourceType;
      _attributeSourceMemberName = null;
      _copiedAttributeTypes = copiedAttributeTypes;
    }

    public CopyCustomAttributesAttribute (Type attributeSourceType, string attributeSourceMemberName, params Type[] copiedAttributeTypes)
    {
      if (attributeSourceType == null)
        throw new ArgumentNullException ("attributeSourceType");
      if (attributeSourceMemberName == null)
        throw new ArgumentNullException ("attributeSourceMemberName");
      if (copiedAttributeTypes == null)
        throw new ArgumentNullException ("copiedAttributeTypes");

      _attributeSourceType = attributeSourceType;
      _attributeSourceMemberName = attributeSourceMemberName;
      _copiedAttributeTypes = copiedAttributeTypes;
    }

    public Type AttributeSourceType
    {
      get { return _attributeSourceType; }
    }

    public string AttributeSourceMemberName
    {
      get { return _attributeSourceMemberName; }
    }

    public object AttributeSourceName
    {
      get
      {
        return AttributeSourceMemberName != null ? AttributeSourceType.FullName + "." + AttributeSourceMemberName : AttributeSourceType.FullName;
      }
    }

    public Type[] CopiedAttributeTypes
    {
      get { return _copiedAttributeTypes; }
      set { _copiedAttributeTypes = value; }
    }

    public MemberInfo GetAttributeSource (MemberTypes memberType)
    {
      if (AttributeSourceMemberName == null)
        return AttributeSourceType;
      else
      {
        MemberInfo[] members =
            AttributeSourceType.GetMember (AttributeSourceMemberName, memberType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (members.Length == 0)
          return null;
        else if (members.Length == 1)
          return members[0];
        else
        {
          throw new AmbiguousMatchException (
              string.Format (
                  "The source member string {0} matches several members on type {1}.",
                  AttributeSourceMemberName,
                  AttributeSourceType.FullName));
        }
      }
    }

    public bool IsCopiedAttributeType (Type type)
    {
      if (type == null)
        throw new ArgumentNullException ("type");
      return CopiedAttributeTypes.Length == 0 || ((IList) CopiedAttributeTypes).Contains (type);
    }
  }
}