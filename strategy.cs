using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LAB_2._2
{
    public interface IXmlStrategy
    {
        List<Student> Search(string filePath, Student filter);
    }

        public class Student
        {
            public string FullName { get; set; }
            public string Faculty { get; set; }
            public string Department { get; set; }
            public string Year { get; set; }
        }

        public class DomStrategy : IXmlStrategy
        {
            public List<Student> Search(string filePath, Student filter)
            {
                var results = new List<Student>();
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNodeList nodes = doc.SelectNodes("//Student");
                foreach (XmlNode node in nodes)
                {
                    string name = node.Attributes["FullName"]?.Value;
                    string fac = node.Attributes["Faculty"]?.Value;
                    string year = node.Attributes["Year"]?.Value;

                    if (Match(name, fac, year, filter))
                    {
                        results.Add(new Student { FullName = name, Faculty = fac, Year = year });
                    }
                }
                return results;
            }

            private bool Match(string n, string f, string y, Student filter) =>
                (string.IsNullOrEmpty(filter.FullName) || n == filter.FullName) &&
                (string.IsNullOrEmpty(filter.Faculty) || f == filter.Faculty) &&
                (string.IsNullOrEmpty(filter.Year) || y == filter.Year);
        }

        public class SaxStrategy : IXmlStrategy
        {
            public List<Student> Search(string filePath, Student filter)
            {
                var results = new List<Student>();
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                        {
                            string name = reader.GetAttribute("FullName");
                            string fac = reader.GetAttribute("Faculty");
                            string year = reader.GetAttribute("Year");

                            if ((string.IsNullOrEmpty(filter.FullName) || name == filter.FullName) &&
                                (string.IsNullOrEmpty(filter.Faculty) || fac == filter.Faculty) &&
                                (string.IsNullOrEmpty(filter.Year) || year == filter.Year))
                            {
                                results.Add(new Student { FullName = name, Faculty = fac, Year = year });
                            }
                        }
                    }
                }
                return results;
            }
        }

        public class LinqToXmlStrategy : IXmlStrategy
        {
            public List<Student> Search(string filePath, Student filter)
            {
                XDocument doc = XDocument.Load(filePath);
                return doc.Descendants("Student")
                    .Where(s => (string.IsNullOrEmpty(filter.FullName) || (string)s.Attribute("FullName") == filter.FullName))
                    .Where(s => (string.IsNullOrEmpty(filter.Faculty) || (string)s.Attribute("Faculty") == filter.Faculty))
                    .Where(s => (string.IsNullOrEmpty(filter.Year) || (string)s.Attribute("Year") == filter.Year))
                    .Select(s => new Student
                    {
                        FullName = (string)s.Attribute("FullName"),
                        Faculty = (string)s.Attribute("Faculty"),
                        Year = (string)s.Attribute("Year")
                    }).ToList();
            }
        }

        public class Analyzer
        {
            private IXmlStrategy _strategy;
            public void SetStrategy(IXmlStrategy strategy) => _strategy = strategy;
            public List<Student> PerformSearch(string path, Student filter) => _strategy.Search(path, filter);
        }
    }
