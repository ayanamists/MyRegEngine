using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    class DisjointSet
    {
        private List<int> set;
        public DisjointSet()
        {
            set = new List<int>();
        }
        public void AddNew()
        {
            set.Add(set.Count);
        }
        public int Find(int what)
        {
            if(set[what] == what) { return what; }
            else
            {
                set[what] = Find(set[what]);
                return set[what];
            }
        }
        public int Merge(int x, int y)
        {
            var i = Find(x);
            var j = Find(y);
            if(i != j)
            {
                set[i] = j;
            }
            return j;
        }
    }
}
