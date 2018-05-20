using System.IO;
using System.Xml.Serialization;

namespace Engine.Shared
{
    public static class Serialization
    {
        public static string Serialize<T>(T objectToSerialize)
        {
            using(StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(objectToSerialize.GetType());
                xmlSerializer.Serialize(stringWriter, objectToSerialize);

                return stringWriter.ToString();
            }
        }

        public static T Deserialize<T>(string serializedObject)
        {
            using(StringReader stringReader = new StringReader(serializedObject))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}