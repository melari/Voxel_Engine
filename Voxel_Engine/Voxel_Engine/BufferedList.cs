using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxel_Engine
{
    class BufferedList<T> : List<T>
    {
        public List<T> toAdd { get; private set; }
        public List<T> toRemove { get; private set; }

        public BufferedList()
            : base()
        {
            toAdd = new List<T>();
            toRemove = new List<T>();
        }

        public void BufferedAdd(T Object)
        {
            toAdd.Add(Object);
        }

        public void BufferedRemove(T Object)
        {
            toRemove.Add(Object);
        }

        public void ApplyChanges()
        {
            AddRange(toAdd);
            foreach (T Object in toRemove)
                Remove(Object);
        }
    }
}
