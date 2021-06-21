using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AillieoUtils
{
    public static class SerializeHelper
    {
        public static T Load<T>(string filename)
        {
            if (!File.Exists(filename))
            {
                return default;
            }
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                T obj = (T)formatter.Deserialize(stream);
                stream.Close();
                return obj;
            }
        }

        public static bool Save<T>(T obj, string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
                stream.Close();
                return true;
            }
        }
    }
}
