using System;
using System.IO;

namespace MainUtility
{
    public class SearchArguments
    {
        public String DirPath { get;  set; }
        public bool IsSearchRecursive { get;  set; }
        public FileAttributes Attributes { get;  set; } 
        public long FileSize { get; set;  }
        public DateTime LastTime { get; set; }


        public SearchArguments() { }

        public SearchArguments(string dirPath, bool isSearchRecursive, FileAttributes attributes, long fileSize, DateTime lastTime)
        {
            DirPath = dirPath;
            IsSearchRecursive = isSearchRecursive;
            Attributes = attributes;
            FileSize = fileSize;
            LastTime = lastTime;
        }
    }
}