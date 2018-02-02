using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

// Custom namespaces
using GF.Components.ErrorHandling;

namespace GF.Service.FandP.PriceCalc.Helpers
{
    /// <summary>
    /// An utility class used for XML serialization entirely.
    /// The SerializationHelper can XML serialize to string and deserialize back to the object.
    /// In case of an exception, the ErrorInfo object encapsulating the error details will be outed.
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// XML serializes a generic object to an XML string,
        /// In case of an exception, the ErrorInfo object encapsulating the error details will be outed.
        /// </summary>
        /// <typeparam name="T">The type of the object to XML serialize</typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="xml">string with XML serialized object; null if an exception occures.</param>
        /// <param name="error">the ErrorInfo object encapsulating the error details in case of an error; null otherwise</param>
        /// <returns>True if no er
        public static bool Serialize<T>(T objectToSerialize, out string xml, out ErrorInfo error)
        {
            xml = null;
            XmlSerializer serializer = null;
            StringWriter writer = null;

            try
            {
                serializer = new XmlSerializer(typeof(T));
                writer = new StringWriter(new StringBuilder(xml));
                serializer.Serialize(writer, objectToSerialize);
                xml = writer.ToString();
            }
            catch (Exception serExc)
            {
                error = new ErrorInfo("{661072A2-3DF4-452F-A53E-5ED8DDAE9D1C}", "Error when trying to serialize file: ", serExc);
                xml = null;
                return false;
            }
            finally
            {
                // Clean up
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (serializer != null)
                {
                    serializer = null;
                }
            }

            error = null;
            return true;
        }

        /// <summary>
        /// XML deserializes an XML string to the object T.
        /// In case of an exception, the ErrorInfo object encapsulating the error details will be outed.
        /// </summary>
        /// <typeparam name="T">The type of the object to XML serialize</typeparam>
        /// <param name="xml">XML string we will try to XML deserialize to an object T</param>
        /// <param name="deserializedObject">Deserialized object of T type</param>
        /// <param name="error">the ErrorInfo object encapsulating the error details in case of an error; null otherwise</param>
        /// <returns>True if no error occures; false otherwise</returns>
        public static bool Deserialize<T>(string xml, out T deserializedObject, out ErrorInfo error)
        {
            XmlSerializer serializer = null;
            TextReader reader = null;

            try
            {
                serializer = new XmlSerializer(typeof(T));
                reader = new StringReader(xml);
                deserializedObject = (T)serializer.Deserialize(reader);
            }
            catch (Exception deserExc)
            {
                error = new ErrorInfo("{878CDE09-D130-4E40-90C3-06A8FBFBB8DC}", "Error when trying to deserialize xml file", deserExc);
                deserializedObject = default(T);
                return false;
            }
            finally
            {
                // Clean up
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (serializer != null)
                {
                    serializer = null;
                }
            }

            error = null;
            return true;
        }
    }
}