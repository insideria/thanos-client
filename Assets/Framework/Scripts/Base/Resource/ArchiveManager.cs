using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Thanos.Resource
{
    class ArchiveManager : Singleton<ArchiveManager>
    {
        internal Dictionary<string, Archive> mArchives;

        public ArchiveManager()
        {
            mArchives = new Dictionary<string, Archive>();
        }

        public void Init()
        {
            StreamReader sr = ResourcesManager.OpenText("Resource");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            XmlElement root = doc.DocumentElement;
            IEnumerator iter = root.GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement child_root = iter.Current as XmlElement;
                IEnumerator child_iter = child_root.GetEnumerator();
                if (!mArchives.ContainsKey(child_root.Name))
                {
                    Archive arh = new Archive();
                    mArchives.Add(child_root.Name, arh);
                }
                while (child_iter.MoveNext())
                {
                    XmlElement file = child_iter.Current as XmlElement;
                    string name = file.GetAttribute("name");
                    string type = file.GetAttribute("type");
                    mArchives[child_root.Name].Add(name, type);
                }
            }
            sr.Close();
        }

        public string getPath(string archiveName, string fileName)
        {
            if (mArchives.ContainsKey(archiveName))
                return mArchives[archiveName].GetPath(fileName);
            else
                DebugEx.LogError("can not find " + archiveName, ResourceCommon.DEBUGTYPENAME);
            return null;
        }
    }
}
   
