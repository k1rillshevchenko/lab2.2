using System.Xml.Xsl;

public class XslService
{
    public void TransformToHtml(string xmlPath, string xslPath, string htmlPath)
    {
        XslCompiledTransform transform = new XslCompiledTransform();
        transform.Load(xslPath);
        transform.Transform(xmlPath, htmlPath);
    }
}
