using System;
using System.IO;

namespace MainUtility
{
    public class SearchArguments
    {
        public String DirPath { get; private set; }
        public bool IsSearchRecursive { get; private set; }
        public FileAttributes Attributes { get; private set; } 

        public SearchArguments() { }

        public SearchArguments(string dirPath, bool isSearchRecursive, FileAttributes attributes)
        {
            DirPath = dirPath;
            IsSearchRecursive = isSearchRecursive;
            Attributes = attributes;
        }
    }
}