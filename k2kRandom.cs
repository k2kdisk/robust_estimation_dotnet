using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace k2kLib
{
    public class k2kRandom
    {
        protected Random Rand = new Random();

        public int[] RandomSampling(int Count, int MaxIndex)
        {
            if (MaxIndex < Count) throw new ArgumentException();

            var ret = new int[Count];
            int i, j,inc;

            i = 0;
            while (i < Count)
            {
                ret[i] = Rand.Next(MaxIndex);
                
                inc = 1;
                for (j = 0; j < i; ++j)if (ret[i] == ret[j]) inc = 0;
                i += inc;
            }

            return ret;
        }
        public T[] RandomSampling<T>(int Count, T[] array)
        {
            var ret = new T[Count];
            var idx = RandomSampling(Count, array.Length);

            for (int i = 0; i < Count; ++i) ret[i] = array[idx[i]];

            return ret;
        }

        public T[] RandomSampling<T>(int Count, IEnumerable<T> array)
        {
            return RandomSampling<T>(Count, array.ToArray<T>());
        }


    }
}

