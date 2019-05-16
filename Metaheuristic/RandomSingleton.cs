using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic
{
    class RandomSingleton
    {
        private int seed;
        private Random rand;

        private static RandomSingleton _instance = null;

        public static RandomSingleton Instance {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = new RandomSingleton(0);
                return _instance;
            }
        }

        public int Seed
        {
            get => seed;
            set
            {
                this.seed = value;
                this.rand = new Random(value);
            }
        }

        public Random Rand
        {
            get => rand;
        }

        private RandomSingleton(int seed)
        {
            this.seed = seed;
            this.rand = new Random(seed);
        }

        public Random GetNewSeededRandom()
        {
            return new Random(seed);
        }


    }
}
