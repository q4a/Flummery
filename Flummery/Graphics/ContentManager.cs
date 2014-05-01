﻿using System;
using System.Collections.Generic;
using System.IO;
using Flummery.ContentPipeline;

namespace Flummery
{
    public class ContentManager
    {
        Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
        public static string Hints = "";

        public static bool LoadOrDefaultFile(string Filename, string FileExtension, out string FilePath)
        {
            string[] hints = Hints.Split(';');
            var fileNames = Filename.Split(';');
            var extensions = FileExtension.Split(';');

            foreach (string file in fileNames)
            {
                foreach (string extension in extensions)
                {
                    foreach (string hint in hints)
                    {
                        FilePath = hint + (hint.EndsWith("\\") ? "" : "\\") + file + "." + extension;

                        if (File.Exists(FilePath)) { return true; }
                    }
                }
            }

            FilePath = null;
            return false;
        }

        public static void AddHint(string hint)
        {
            var list = new List<string>(Hints.Split(';'));
            int index = list.IndexOf(hint);

            if (index > -1) { list.RemoveAt(index); }
            list.Insert(0, hint);

            Hints = string.Join(";", list.ToArray());
        }

        public T Load<T, T2>(string assetName, string assetPath = null, bool bAddToScene = false) where T : Asset, new()
                                                                                                  where T2 : ContentImporter, new()
        {
            string key = typeof(T).ToString() + assetName;

            if (assets.ContainsKey(key)) { return (T)assets[key]; }

            var importer = new T2();
            var path = importer.Find(assetName, assetPath);
            Asset content = null;

            if (path != null)
            {
                content = importer.Import(path);
            }
            else
            {
                content = new T();
                content.Name = assetName;
            }

            if (content != null)
            {
                assets[key] = content;
                if (bAddToScene) { SceneManager.Scene.Add(content); }

                return (T)content;
            }

            return default(T);
        }
    }
}
