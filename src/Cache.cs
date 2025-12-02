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
            StringBuilder data = new StringBuilder();

            string full = GetFullPath(t.gameObject);
            data.Append(full);
            data.Append($"{t.gameObject.tag}{t.gameObject.layer}{t.gameObject.activeSelf}");
            data.Append($"{t.position.x}{t.position.y}{t.position.z}");
            data.Append($"{t.localScale.x}{t.localScale.y}{t.localScale.z}");
            data.Append($"{t.rotation.x}{t.rotation.y}{t.rotation.z}{t.rotation.w}");

            for (int i = 0; i < t.childCount; i++) {
                Transform child = t.GetChild(i);
                data.Append(child.name);
            }

            foreach (Component c in t.GetComponents<Component>()) {
                data.Append($"{c.name}{c.GetType()}");
            }

            E_ForceZone zone = t.gameObject.GetComponent<E_ForceZone>();
            if (zone != null) {
                data.Append($"{zone.addPushForceOnEnter}{zone.constantPushForce}");
                data.Append($"{zone.resetVelocity}{zone.force}");
            }

            string fullName = data.ToString();

            return Encoding.UTF8.GetBytes(fullName.ToString());
        }

        private static string BytesToString(byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
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

            List<UniqueID> ordered = Resources
                .FindObjectsOfTypeAll<UniqueID>()
                .ToList();

            ordered.Sort((a, b) => String.Compare(a.name, b.name));

            using (MemoryStream stream = new MemoryStream()) {
                foreach (UniqueID obj in ordered) {
                    byte[] bytes = ToBytes(obj.transform);
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
