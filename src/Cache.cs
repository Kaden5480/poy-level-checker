using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;

namespace LevelChecker {
    internal static class Cache {
        internal static Font gameFont { get; private set; }
        internal static string hash { get; private set; }

        private static string BytesToString(byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private static string GetFullPath(GameObject obj) {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null) {
                path = $"{parent.gameObject.name}/{path}";
                parent = parent.parent;
            }

            return path;
        }

        private static byte[] ToBytes(Transform t) {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{GetFullPath(t.gameObject)}");
            builder.Append($"{t.gameObject.tag}");
            builder.Append($"{t.gameObject.layer}");
            builder.Append($"{t.position}");
            builder.Append($"{t.localScale}");
            builder.Append($"{t.rotation}");

            E_ForceZone zone = t.gameObject.GetComponent<E_ForceZone>();
            if (zone != null) {
                builder.Append($"{zone.addPushForceOnEnter}{zone.constantPushForce}");
                builder.Append($"{zone.resetVelocity}{zone.force}");
            }

            string fullName = builder.ToString();

            return Encoding.UTF8.GetBytes(fullName);
        }

#region Caching

        private static void FindFont() {
            foreach (Font f in Resources.FindObjectsOfTypeAll<Font>()) {
                if ("roman-antique.regular".Equals(f.name) == true) {
                    gameFont = f;
                }
            }
        }

        internal static void FindObjects() {
            if (gameFont == null) {
                FindFont();
            }

            List<string> orderedNames = new List<string>();
            List<UniqueID> objects = Resources
                .FindObjectsOfTypeAll<UniqueID>()
                .ToList();

            foreach (UniqueID obj in objects) {
                byte[] bytes = ToBytes(obj.transform);

                using (SHA1 sha1 = SHA1.Create()) {
                    orderedNames.Add(BytesToString(
                        sha1.ComputeHash(bytes)
                    ));
                }
            }

            orderedNames.Sort((a, b) => String.Compare(a, b));

            using (MemoryStream stream = new MemoryStream()) {
                foreach (string name in orderedNames) {
                    byte[] bytes = Encoding.UTF8.GetBytes(name);
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (SHA1 sha1 = SHA1.Create()) {
                    hash = BytesToString(
                        sha1.ComputeHash(stream.ToArray())
                    );
                }
            }
        }

        internal static void Clear() {
            hash = null;
        }

#endregion

    }
}
