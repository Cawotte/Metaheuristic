using System;
using System.Collections.Generic;
using System.Text;

namespace Metaheuristic
{
    public class RandomSingleton
    {
        private int seed;
        private Random rand;
        private Random currentAlgoRand;

        private static RandomSingleton _instance = null;

        public static RandomSingleton Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new RandomSingleton(0);
                }
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
                this.currentAlgoRand = new Random(value);
            }
        }

        public Random Rand
        {
            get => rand;
        }

        public Random CurrentAlgoRand
        {
            get => currentAlgoRand;
            set => currentAlgoRand = value;
        }

        private RandomSingleton(int seed)
        {
            this.seed = seed;
            this.rand = new Random(seed);
            this.currentAlgoRand = new Random(seed);
        }

        public Random GetNewSeededRandom()
        {
            return new Random(seed);
        }

        public Random ResetCurrentAlgoRandom()
        {
            this.currentAlgoRand = new Random(seed);
            return this.currentAlgoRand;
        }


    }
}
