using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Schwer.WebGL {
    public static class WebGLHelperSaveHelper {
        [DllImport("__Internal")] private static extern void SetDownload(string base64, string fileName);
        [DllImport("__Internal")] public static extern void ImportEnabled(bool enabled);

        // References:
        // https://stackoverflow.com/questions/17845032/net-mvc-deserialize-byte-array-from-json-uint8array
        // https://stackoverflow.com/questions/4736155/how-do-i-convert-struct-system-byte-byte-to-a-system-io-stream-object-in-c
        public static SaveData SaveDataFromBase64String(string base64) {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(Convert.FromBase64String(base64))) {
                try {
                    return formatter.Deserialize(stream) as SaveData;
                }
                catch (System.Runtime.Serialization.SerializationException e) {
                    Debug.LogWarning(e);
                }
            }
            return null;
        }

        // Reference: https://forum.unity.com/threads/access-specific-files-in-idbfs.452168/
        public static void PushToDownload(string filePath, string fileName) {
            var bytes = File.ReadAllBytes(filePath);
            SetDownload(Convert.ToBase64String(bytes), fileName);
        }
    }
}
