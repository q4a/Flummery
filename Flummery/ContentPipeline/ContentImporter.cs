﻿using System.IO;

namespace Flummery.ContentPipeline
{
    public abstract class ContentImporter
    {
        public virtual string GetExtension() { return ""; }
        public virtual string GetHints(string currentPath) { return null; }

        public string Find(string assetName, string currentPath = null)
        {
            if (currentPath != null && File.Exists(currentPath + assetName)) { return currentPath + assetName; }

            if (currentPath != null) { ContentManager.AddHint(currentPath); }
            if (assetName.IndexOf(".") > -1) { assetName = assetName.Substring(0, assetName.LastIndexOf(".")); }

            string hints = GetHints(currentPath);
            if (hints != null) { foreach (string hint in hints.Split(';')) { ContentManager.AddHint(hint); } }

            if (ContentManager.LoadOrDefaultFile(assetName, GetExtension(), out string path))
            {
                return path;
            }
            else
            {
                return null;
            }
        }

        public virtual Asset Import(string path)
        {
            return default(Asset);
        }

        public virtual AssetList ImportMany(string path)
        {
            return default(AssetList);
        }
    }
}