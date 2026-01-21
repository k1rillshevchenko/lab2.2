<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<body>
				<h2>Список студентів гуртожитку</h2>
				<table border="1">
					<tr bgcolor="#9acd32">
						<th>ПІП</th>
						<th>Факультет</th>
						<th>Курс</th>
					</tr>
					<xsl:for-each select="Dormitory/Student">
						<tr>
							<td>
								<xsl:value-of select="@FullName"/>
							</td>
							<td>
								<xsl:value-of select="@Faculty"/>
							</td>
							<td>
								<xsl:value-of select="@Year"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
