using System.Collections.Generic;
using System.IO;

namespace GamePackages.Localization
{
    public class LocalizedFile
    {
        public string      name;
        public List<Block> blocks;
 
        public string assetFilePath; 

        public List<LocalizedSet> ExtractSets(string langKey)
        {
            List<LocalizedSet> sets = new List<LocalizedSet>(16);

            foreach (var b in blocks)
            {
                if (b is LocalizedTextBlock line)
                {
                    LocalizedSet s = new LocalizedSet();
                    s.key        = line.fullKey;
                    s.tags       = line.tags;
                    s.originText = "";

                    foreach (var r in line.texts)
                    {
                        if (r.languageKey == langKey)
                            s.originText = LocalizationSettings.DecodeTagsToText(r.text);
                    }

                    sets.Add(s);
                }
            }

            return sets;
        }

        public void Write(TextWriter w)
        {
            foreach (var b in blocks)
            {
                b.Write(w);
            }
        }

#if UNITY_EDITOR
        public void RewriteToFile()
        {
            FileInfo fileInfo = new FileInfo(assetFilePath);
            fileInfo.Delete();

            StreamWriter s = new StreamWriter(assetFilePath);
            Write(s);
            s.Flush();
            s.Close();
        }
#endif
    }

    public class LocalizedLineBlockInfo
    { 
        public LocalizedTextBlock block;
    }
}