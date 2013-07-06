using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;
using log4net;

namespace MHM.WinFlexOne.Services.Integration.ConfigModel
{
    public class IntegrationConfigSection : ConfigurationSection
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IntegrationConfigSection));
        private string m_TypeName;
        private object m_Data;
        private string m_FileName;


        protected override object GetRuntimeObject()
        {
            return m_Data;
        }

        protected override void DeserializeSection(XmlReader reader)
        {
            if (!reader.Read() || (reader.NodeType != XmlNodeType.Element))
            {
                throw new ConfigurationErrorsException("Configuration reader expected to find an element", reader);
            }
            DeserializeElement(reader, false);
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            reader.MoveToContent();

            // Check for invalid usage
            if (reader.AttributeCount > 1)
                throw new ConfigurationErrorsException("Only a single type or fileName attribute is allowed.");
            if (reader.AttributeCount == 0)
                throw new ConfigurationErrorsException("A type or fileName attribute is required.");

            // Determine if we need to get the section from the inline XML or from an external file.
            m_FileName = reader.GetAttribute("fileName");
            if (m_FileName == null)
            {
                DeserializeData(reader);
                reader.ReadEndElement();
            }
            else
            {
                if (!reader.IsEmptyElement)
                {
                    throw new ConfigurationErrorsException(
                        "The section element must be empty when using the fileName attribute.");
                }

                using (FileStream file = new FileStream(m_FileName, FileMode.Open, FileAccess.Read))
                {
                    XmlReader rdr = new XmlTextReader(file);
                    rdr.MoveToContent();
                    DeserializeData(rdr);
                    rdr.Close();
                }
            }
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            StringWriter sWriter = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xWriter = new XmlTextWriter(sWriter);
            xWriter.Formatting = Formatting.Indented;
            xWriter.Indentation = 4;
            xWriter.IndentChar = ' ';
            SerializeToXmlElement(xWriter, name);
            xWriter.Flush();
            return sWriter.ToString();
        }

        protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
        {
            if (writer == null)
                return false;

            writer.WriteStartElement(elementName);

            bool success;

            if (m_FileName == null || m_FileName == string.Empty)
            {
                success = SerializeElement(writer, false);
            }
            else
            {
                writer.WriteAttributeString("fileName", m_FileName);

                using (FileStream file = new FileStream(m_FileName, FileMode.Create, FileAccess.Write))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ("\t");
                    settings.OmitXmlDeclaration = false;
                    XmlWriter wtr = XmlWriter.Create(file, settings);
                    wtr.WriteStartElement(elementName);
                    success = SerializeElement(wtr, false);
                    wtr.WriteEndElement();
                    wtr.Flush();
                    wtr.Close();
                }
            }

            writer.WriteEndElement();
            return success;
        }

        /// <summary>
        /// Deserializes the data from the reader.
        /// </summary>
        /// <param name="reader">The XmlReader containing the serilized data.</param>
        private void DeserializeData(XmlReader reader)
        {
            m_TypeName = reader.GetAttribute("type");
            Type t = Type.GetType(m_TypeName);
            Logger.Debug(Environment.CurrentDirectory);
            Logger.Debug(string.Format("Config TypeName: '{0}' was {1}resolved.", m_TypeName, (t == null ? "not " : "")));
            reader.Read();
            reader.MoveToContent();
            XmlSerializer serializer = new XmlSerializer(t);
            m_Data = serializer.Deserialize(reader);
        }
    }
}
