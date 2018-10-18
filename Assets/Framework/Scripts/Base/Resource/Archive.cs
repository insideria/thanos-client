using System.Collections.Generic;

namespace Thanos.Resource
{
    public class Archive
    {
        private Dictionary<string, string> mFiles; //["name-type", "name1-type1", "name2-type2"]

        public Archive()
        {
            mFiles = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Files
        {
            get
            {
                return mFiles;
            }
        }

        public void Add(string filename, string type)
        {
            if (!mFiles.ContainsKey(filename))
            {
                mFiles.Add(filename, type);
            }
        }

        public string GetPath(string fileName)
        {
            if (mFiles.ContainsKey(fileName))
                return fileName + "." + mFiles[fileName];           //name+type
            else
                DebugEx.LogError("can not find " + fileName, ResourceCommon.DEBUGTYPENAME);
            return null;
        }
    }
}