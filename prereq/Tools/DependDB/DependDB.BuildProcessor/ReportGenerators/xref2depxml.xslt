<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0" xmlns="http://rubicon.eu/1.6/DepXML" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ru="http://www.rubicon.eu"
                exclude-result-prefixes="ru">
  <xsl:output method="xml" indent="yes" />

  <xsl:variable name="involvedTypes" select="//InvolvedType" />
  <xsl:variable name="assemblies" select="//Assembly" />

  <xsl:function name="ru:GetNameWithoutGenericParameters">
    <xsl:param name="typeName" />

    <xsl:choose>
      <xsl:when test="contains($typeName, '&lt;')">
        <xsl:value-of select="substring-before($typeName, '&lt;')" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$typeName" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:function name="ru:GetGenericTypeParameters">
    <xsl:param name="typeName" />

    <xsl:if test="contains($typeName, '&lt;')">
      <xsl:variable name="params" select="tokenize(substring-before(substring-after($typeName, '&lt;'), '&gt;'), ',')" />
      <xsl:element name="GenericTypeParameters">
        <xsl:call-template name="genericParameters">
          <xsl:with-param name="params" select="$params" />
        </xsl:call-template>
      </xsl:element>
    </xsl:if>
  </xsl:function>

  <xsl:function name="ru:GetGenericTypeParameterCount">
    <xsl:param name="typeName" />

    <xsl:if test="contains($typeName, '&lt;')">
      <xsl:variable name="count" select="count(tokenize(substring-before(substring-after($typeName, '&lt;'), '&gt;'), ','))" />
      <xsl:choose>
        <xsl:when test="$count > 0">
          <xsl:value-of select="concat('`', $count)" />
        </xsl:when>
      </xsl:choose>
    </xsl:if>
  </xsl:function>

  <xsl:function name="ru:GetGenericTypeDefinitionOrType">
    <xsl:param name="type" />

    <xsl:copy-of select="if ($type/@generic-definition-ref) then $involvedTypes[@id = $type/@generic-definition-ref] else $type" />
  </xsl:function>

  <xsl:function name="ru:GetRelevantSignature">
    <xsl:param name="signature" />

    <xsl:element name="Signature">
      <xsl:apply-templates select="$signature//*" mode="cleanUpSignature" />
    </xsl:element>
  </xsl:function>

  <xsl:function name="ru:SignatureIsMatch">
    <xsl:param name="signature1" />
    <xsl:param name="signature2" />

    <xsl:value-of select="string(ru:GetRelevantSignature($signature1)) = string(ru:GetRelevantSignature($signature2))" />
  </xsl:function>

  <xsl:template match="/">
    <xsl:element name="DependencyReport">
      <xsl:element name="Assemblies">
        <xsl:for-each select="//Assemblies/Assembly">
          <xsl:variable name="assembly" select="." />
          <xsl:if test="count(//*[@assembly-ref = $assembly/@id]) > 0">
            <xsl:element name="Assembly">
              <xsl:attribute name="id">
                <xsl:value-of select="@id" />
              </xsl:attribute>
              <xsl:attribute name="name">
                <xsl:value-of select="@name" />
              </xsl:attribute>
              <xsl:attribute name="culture">
                <xsl:value-of select="@culture" />
              </xsl:attribute>
              <xsl:attribute name="version">
                <xsl:value-of select="@version" />
              </xsl:attribute>
              <xsl:attribute name="publicKeyToken">
                <xsl:value-of select="@publicKeyToken" />
              </xsl:attribute>

              <!-- this is a dummy value that must be overwritten with the correct origin in the XReferencer task -->
              <xsl:attribute name="origin">
                <xsl:text>OwnProject</xsl:text>
              </xsl:attribute>
            </xsl:element>
          </xsl:if>
        </xsl:for-each>
      </xsl:element>
      <xsl:element name="Dependencies">
        <xsl:for-each select="//InvolvedTypes/InvolvedType[@is-target = 'true']/Mixins/Mixin">
          <xsl:choose>
            <!-- Class <- Mixin -->
            <xsl:when test="@relation = 'Extends' or @relation = 'Used by'">
              <xsl:element name="Dependency">
                <xsl:variable name="context" select="." />
                <xsl:variable name="target" select="../.." />
                <xsl:variable name="mixin" select="//InvolvedTypes/InvolvedType[@id = $context/@ref]" />

                <xsl:apply-templates select="if (@relation = 'Used by') then $mixin else $target" mode="referenced" />
                <xsl:element name="Dependents">
                  <xsl:apply-templates select="if (@relation = 'Used by') then $target else $mixin" mode="dependent">
                    <xsl:with-param name="relation" select="if (@relation = 'Extends') then 'MixinExtends' else 'MixinUsedBy'" />
                  </xsl:apply-templates>
                </xsl:element>
              </xsl:element>
            </xsl:when>
          </xsl:choose>
        </xsl:for-each>

        <!-- OverrideTarget -> Member -->
        <xsl:for-each select="//InvolvedTypes/InvolvedType/Members/Member/OverriddenMembers/Member-Reference[@type = 'OverrideTarget']">
          <xsl:element name="Dependency">
            <xsl:variable name="member-ref" select="." />
            <xsl:apply-templates select="//Member[@id = $member-ref/@ref]" mode="referenced" />
            <xsl:element name="Dependents">
              <xsl:apply-templates select="../.." mode="dependent">
                <xsl:with-param name="relation" select="'MixinOverrideTarget'" />
              </xsl:apply-templates>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>

        <!-- OverrideMixin -> Member -->
        <xsl:for-each select="//InvolvedTypes/InvolvedType/Members/Member/OverriddenMembers/Member-Reference[@type = 'OverrideMixin']">
          <xsl:element name="Dependency">
            <xsl:variable name="member-ref" select="." />
            <xsl:apply-templates select="//Member[@id = $member-ref/@ref]" mode="referenced" />
            <xsl:element name="Dependents">
              <xsl:apply-templates select="../.." mode="dependent">
                <xsl:with-param name="relation" select="'MixinOverrideMixin'" />
              </xsl:apply-templates>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>

        <!-- Special handling for members with sub-members (properties/events, sub-members are methods like get_X())-->
        <!-- OverrideTarget -> SubMember -->
        <xsl:for-each select="//InvolvedTypes/InvolvedType/Members/Member/SubMember/OverriddenMembers/Member-Reference[@type = 'OverrideTarget']">
          <xsl:element name="Dependency">
            <xsl:variable name="member-ref" select="." />
            <xsl:apply-templates select="//SubMember[@id = $member-ref/@ref]" mode="referenced" />
            <xsl:element name="Dependents">
              <xsl:apply-templates select="../.." mode="dependent">
                <xsl:with-param name="relation" select="'MixinOverrideTarget'" />
              </xsl:apply-templates>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>

        <!-- OverrideMixin -> SubMember -->
        <xsl:for-each select="//InvolvedTypes/InvolvedType/Members/Member/SubMember/OverriddenMembers/Member-Reference[@type = 'OverrideMixin']">
          <xsl:element name="Dependency">
            <xsl:variable name="member-ref" select="." />
            <xsl:apply-templates select="//SubMember[@id = $member-ref/@ref]" mode="referenced" />
            <xsl:element name="Dependents">
              <xsl:apply-templates select="../.." mode="dependent">
                <xsl:with-param name="relation" select="'MixinOverrideMixin'" />
              </xsl:apply-templates>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>

        <!-- Target Class -> Required Interface -->
        <xsl:for-each-group select="//InvolvedTypes/InvolvedType/Mixins/Mixin/TargetCallDependencies/Dependency[@is-interface = 'true']"
                            group-by="string-join(@*, '?')">
          <xsl:element name="Dependency">
            <xsl:variable name="dependency" select="current-group()[1]" />
            <xsl:apply-templates select="$dependency" mode="referenced" />
            <xsl:element name="Dependents">
              <xsl:apply-templates select="../../../.." mode="dependent">
                <xsl:with-param name="relation" select="'MixinDynamicTypeImplementation'" />
              </xsl:apply-templates>
            </xsl:element>
          </xsl:element>
        </xsl:for-each-group>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="*" mode="cleanUpSignature">
    <xsl:choose>
      <xsl:when test="name(.) = 'ParameterName' or name(.) = 'GenericMethodParameter'"><!-- Remove --></xsl:when>
      <xsl:when test="name(.) = 'ExplicitInterfaceName'">
        <xsl:value-of select="concat(ru:GetNameWithoutGenericParameters(.), replace(., '[^,&gt;&lt;]', ''))" />
      </xsl:when>
      <xsl:when test="name(.) = 'Keyword' or name(.) = 'Name'">
        <xsl:value-of select="." /><xsl:text> </xsl:text>
      </xsl:when>
      <xsl:when test="name(.) = 'Type'">
        <xsl:value-of select="if (./@genericParameter) then @genericParameter else ." /><xsl:text> </xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="." />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="dependencyPart">
    <xsl:param name="type" />

    <xsl:attribute name="metadataToken">
      <xsl:value-of select="$type/@metadataToken" />
    </xsl:attribute>
    <xsl:attribute name="assemblyRef">
      <xsl:value-of select="$type/@assembly-ref" />
    </xsl:attribute>
    <xsl:attribute name="assemblyName">
      <xsl:value-of select="$assemblies[@id = $type/@assembly-ref]/@name" />
    </xsl:attribute>
    <xsl:attribute name="assemblyNameGroupId">
      <xsl:value-of select="0" />
    </xsl:attribute>
    <xsl:attribute name="typeType">
      <xsl:choose>
        <xsl:when test="$type/@is-interface = 'true'">
          <xsl:text>Interface</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>Class</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
    <xsl:attribute name="fullyQualifiedTypeName">
      <xsl:value-of
        select="concat($type/@namespace, '.', ru:GetNameWithoutGenericParameters($type/@name), ru:GetGenericTypeParameterCount($type/@name))" />
    </xsl:attribute>
    <xsl:attribute name="fullyQualifiedTypeNameGroupId">
      <xsl:value-of select="0" />
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="InvolvedType" mode="referenced">
    <xsl:element name="Referenced">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(.)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="InvolvedType" mode="dependent">
    <xsl:param name="relation" />

    <xsl:element name="Dependent">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(.)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:attribute name="dependencyType" select="$relation" />
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="Dependency" mode="referenced">
    <xsl:element name="Referenced">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(.)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="Dependency" mode="dependent">
    <xsl:param name="relation" />

    <xsl:element name="Dependent">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(.)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:attribute name="dependencyType" select="$relation" />
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="Member" mode="referenced">
    <xsl:element name="Referenced">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(../..)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
      <xsl:call-template name="memberPart" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="SubMember" mode="referenced">
    <xsl:element name="Referenced">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(../../..)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
      <xsl:call-template name="memberPart" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="SubMember" mode="dependent">
    <xsl:param name="relation" />

    <xsl:element name="Dependent">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(../../..)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:attribute name="dependencyType" select="$relation" />
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
      <xsl:call-template name="memberPart" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="Member" mode="dependent">
    <xsl:param name="relation" />

    <xsl:element name="Dependent">
      <xsl:variable name="type" select="ru:GetGenericTypeDefinitionOrType(../..)" />
      <xsl:call-template name="dependencyPart">
        <xsl:with-param name="type" select="$type" />
      </xsl:call-template>
      <xsl:attribute name="dependencyType" select="$relation" />
      <xsl:copy-of select="ru:GetGenericTypeParameters($type/@name)" />
      <xsl:call-template name="memberPart" />
    </xsl:element>
  </xsl:template>

  <xsl:template name="memberPart">
    <xsl:choose>
      <xsl:when test="@type = 'Method' or @type = 'Constructor'">
        <xsl:call-template name="methodElement" />
      </xsl:when>
      <xsl:when test="@type = 'Event'">
        <xsl:call-template name="eventElement" />
      </xsl:when>
      <xsl:when test="@type = 'Property'">
        <xsl:call-template name="propertyElement" />
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="genericParameters">
    <xsl:param name="params" />

    <xsl:for-each select="$params">
      <xsl:element name="GenericParameter">
        <xsl:attribute name="index" select="position() - 1" />
        <xsl:attribute name="name" select="normalize-space(.)" />
      </xsl:element>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="element">
    <xsl:attribute name="metadataToken" select="@metadataToken" />
    <xsl:attribute name="name" select="@name" />
    <xsl:attribute name="isSpecialName" select="'false'" />
    <xsl:choose>
      <xsl:when test="count(Modifiers//Keyword[text() = 'static']) > 0">
        <xsl:attribute name="isStatic" select="'true'" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:attribute name="isStatic" select="'false'" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="typeAttribute">
    <xsl:param name="typeSource" />
    <xsl:variable name="typeName" select="if ($typeSource/@genericParameter) then $typeSource/@genericParameter else $typeSource" />
    <xsl:attribute name="type" select="replace($typeName, ', ', ',')" />
  </xsl:template>

  <xsl:template name="variableElement">
    <xsl:call-template name="element" />
    <xsl:call-template name="typeAttribute">
      <xsl:with-param name="typeSource" select="Signature/Type[1]" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="parametersElement">
    <xsl:if test="count(Signature/ParameterName) > 0">
      <xsl:element name="Parameters">
        <xsl:for-each select="Signature/ParameterName">
          <xsl:element name="Parameter">
            <xsl:attribute name="name" select="./text()" />
            <xsl:call-template name="typeAttribute">
              <xsl:with-param name="typeSource" select="preceding-sibling::Type[1]" />
            </xsl:call-template>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:if>
  </xsl:template>

  <xsl:template name="methodElement">
    <xsl:element name="Method">
      <xsl:call-template name="variableElement" />
      <xsl:choose>
        <xsl:when test="@type = 'Constructor'">
          <xsl:attribute name="isSpecialName" select="'true'" />
          <xsl:attribute name="name" select="'.ctor'" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="isSpecialName" select="'false'" />
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="count(Signature/GenericMethodParameter) > 0">
        <xsl:element name="GenericParameters">
          <xsl:call-template name="genericParameters">
            <xsl:with-param name="params" select="Signature/GenericMethodParameter" />
          </xsl:call-template>
        </xsl:element>
      </xsl:if>
      <xsl:call-template name="parametersElement" />
    </xsl:element>
  </xsl:template>

  <xsl:template name="eventElement">
    <xsl:element name="Event">
      <xsl:call-template name="variableElement" />
    </xsl:element>
  </xsl:template>

  <xsl:template name="propertyElement">
    <xsl:element name="Property">
      <xsl:call-template name="variableElement" />
      <xsl:call-template name="parametersElement" />
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>