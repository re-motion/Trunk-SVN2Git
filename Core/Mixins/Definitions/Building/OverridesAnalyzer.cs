using System;
using System.Collections.Generic;
using Remotion.Mixins.Definitions;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class OverridesAnalyzer<TMember>
      where TMember : MemberDefinition
  {
    private readonly Type _attributeType;
    private readonly IEnumerable<TMember> _baseMembers;

    private MultiDictionary<string, TMember> _baseMembersByNameCache = null;

    public OverridesAnalyzer (Type attributeType, IEnumerable<TMember> baseMembers)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("attributeType", attributeType, typeof (IOverrideAttribute));
      ArgumentUtility.CheckNotNull ("baseMembers", baseMembers);

      _attributeType = attributeType;
      _baseMembers = baseMembers;
    }

    public IEnumerable<Tuple<TMember, TMember>> Analyze (IEnumerable<TMember> overriderMembers)
    {
      ArgumentUtility.CheckNotNull ("overriderMembers", overriderMembers);

      foreach (TMember member in overriderMembers)
      {
        IOverrideAttribute overrideAttribute = (IOverrideAttribute) AttributeUtility.GetCustomAttribute (member.MemberInfo, _attributeType, true);
        if (overrideAttribute != null)
        {
          TMember baseMember;
          if (BaseMembersByName.ContainsKey (member.Name))
          {
            IEnumerable<TMember> candidates = BaseMembersByName[member.Name];
            baseMember = FindBaseMember (overrideAttribute, member, candidates);
          }
          else
            baseMember = null;

          if (baseMember == null)
          {
            string message = string.Format ("The member overridden by '{0}' could not be found.", member.FullName);
            throw new ConfigurationException (message);
          }
          yield return new Tuple<TMember, TMember> (member, baseMember);
        }
      }
    }

    private MultiDictionary<string, TMember> BaseMembersByName
    {
      get
      {
        EnsureMembersCached();
        return _baseMembersByNameCache;
      }
    }

    private void EnsureMembersCached ()
    {
      if (_baseMembersByNameCache == null)
      {
        _baseMembersByNameCache = new MultiDictionary<string, TMember> ();
        foreach (TMember member in _baseMembers)
          _baseMembersByNameCache.Add (member.Name, member);
      }
    }

    private TMember FindBaseMember (IOverrideAttribute attribute, TMember overrider, IEnumerable<TMember> candidates)
    {
      TMember result = null;
      foreach (TMember candidate in candidates)
      {
        if (candidate.Name == overrider.Name && candidate.CanBeOverriddenBy (overrider)
            && (attribute.OverriddenType == null || ReflectionUtility.CanAscribe (candidate.DeclaringClass.Type, attribute.OverriddenType)))
        {
          if (result != null)
          {
            string message = string.Format (
                "Ambiguous override: Member {0} could override {1} and {2}.",
                overrider.FullName,
                result.FullName,
                candidate.FullName);
            throw new ConfigurationException (message);
          }
          else
            result = candidate;
        }
      }
      return result;
    }
  }
}
