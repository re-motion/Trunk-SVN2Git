using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase: IMappingLoader
  {
    protected MappingReflectorBase()
    {
    }

    protected abstract Type[] GetDomainObjectTypes();

    public ClassDefinitionCollection GetClassDefinitions()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectors())
        classReflector.GetClassDefinition (classDefinitions);

      return classDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectorsForRelations (classDefinitions))
        classReflector.GetRelationDefinitions (classDefinitions, relationDefinitions);

      return relationDefinitions;
    }

    private List<ClassReflector> CreateClassReflectors()
    {
      List<ClassReflector> classReflectors = new List<ClassReflector>();
      InheritanceHierarchyFilter inheritanceHierarchyFilter = new InheritanceHierarchyFilter (GetDomainObjectTypesSorted());
      foreach (Type domainObjectClass in inheritanceHierarchyFilter.GetLeafTypes())
        classReflectors.Add (ClassReflector.CreateClassReflector (domainObjectClass));
      
      return classReflectors;
    }

    private List<ClassReflector> CreateClassReflectorsForRelations (ClassDefinitionCollection classDefinitions)
    {
      List<ClassReflector> classReflectors = new List<ClassReflector> ();
      foreach (ReflectionBasedClassDefinition classDefinition in classDefinitions)
        classReflectors.Add (ClassReflector.CreateClassReflector (classDefinition.ClassType));

      return classReflectors;
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      Type[] domainObjectTypes = GetDomainObjectTypes ();

      Array.Sort (
          domainObjectTypes,
          delegate (Type left, Type right) { return string.Compare (left.FullName, right.FullName, StringComparison.OrdinalIgnoreCase); });

      return domainObjectTypes;
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}