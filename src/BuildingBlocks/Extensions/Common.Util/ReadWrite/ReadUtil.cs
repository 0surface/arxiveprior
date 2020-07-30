using System;
using System.IO;
using System.Reflection;

namespace Common.Util.ReadWrite
{
    public class ReadUtil
    {
        public static string ReadDocumentFromExecutingAssembly(string resourceName)
        {
            string data = "";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }

            return data;
        }
    }
}
