<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method='text'/>

<xsl:template match="/">
	<xsl:apply-templates/>
</xsl:template>

<xsl:template match="test-results">
<xsl:if test="@failures > 0 or @errors > 0">
<xsl:text>Test Suite: </xsl:text>
<xsl:value-of select="@name"/>
<xsl:text>
Failures: </xsl:text>
<xsl:value-of select="@failures"/>
<xsl:text>
Errors: </xsl:text>
<xsl:value-of select="@errors"/>
<xsl:text>
</xsl:text>

<xsl:apply-templates select=".//test-case[failure]"/>
</xsl:if>
</xsl:template>

<xsl:template match="//test-case">
	<xsl:value-of select="position()"/><xsl:text>) </xsl:text>
	<xsl:value-of select="@name"/>
	<xsl:text> : </xsl:text>
	<xsl:value-of select="child::node()/message"/>
<xsl:text disable-output-escaping='yes'>&#xD;&#xA;</xsl:text>
	<xsl:if test="failure">
		<xsl:value-of select="failure/stack-trace"/>
<xsl:text>
</xsl:text>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
