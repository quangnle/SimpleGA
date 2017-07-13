using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGA
{
    public class GA
    {
        private static Random _rnd = new Random();
        private char[] _target;
        private char[] _materials;
        private int _nPopulation;
        private double _mutationRate;

        private DNA[] _population;

        public GA(string target, int nPopulation, double mutationRate)
        {
            _target = target.ToCharArray();
            _nPopulation = nPopulation;
            _mutationRate = mutationRate;
        }

        /// <summary>
        ///create materials extracted from target 
        // just to minimize search space
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private char[] CreateMaterials()
        {
            char[] materials = new char[27];
            materials[26] = ' ';
            for (int i = 0; i < 26; i++)
            {
                materials[i] = (char)(i + 'a');
            }
            return materials;
        }

        private DNA[] InititalizePopulation(int len, int nPopulation, char[] materials)
        {
            var population = new DNA[nPopulation];

            for (int i = 0; i < nPopulation; i++)
            {
                var content = new char[len];
                for (int j = 0; j < len; j++)
                {
                    var r = _rnd.Next(0, materials.Length);
                    var c = materials[r];
                    content[j] = c;
                }
                population[i] = new DNA(content);
            }
            return population;
        }

        private List<int> CreateMatingPool(char[] target , DNA[] population)
        {
            List<int> fProportion = new List<int>();
            for (int i = 0; i < population.Length; i++)
            {
                // calculate distance to the target
                var fitness = _population[i].CompareTo(target);

                // add its portion into the sum porportion
                for (int j = 0; j < fitness; j++)
                {
                    fProportion.Add(i);
                }
            }

            return fProportion;
        }

        private DNA EvolOnce(char[] target, DNA[] population, double mutationRate, char[] materials)
        {
            // update fitness
            var pool = CreateMatingPool(target, population);

            for (int i = 0; i < population.Length; i++)
            {
                // randomly select 2 parents in the polulation
                // based on the fitness pool
                var p1 = population[pool[_rnd.Next(pool.Count)]];
                var p2 = population[pool[_rnd.Next(pool.Count)]];

                // to make sure the split point 
                // will always fall in valid range 
                var splitPoint = _rnd.Next(_target.Length - 2) + 1;

                // crossover to create new DNA
                var newDNA = p1.CrossOver(p2, splitPoint);
                newDNA.Mutate(mutationRate, materials);
                population[i] = newDNA;
                if (newDNA.CompareTo(target) == target.Length)
                    return newDNA;
            }

            return null;
        }

        public void Evolution()
        {
            _materials = CreateMaterials();
            _population = InititalizePopulation(_target.Length, _nPopulation, _materials);
            DNA dna = null;
            var i = 0;
            do
            {
                i++;
                dna = EvolOnce(_target, _population, _mutationRate, _materials);
            } while (dna == null);

            Console.WriteLine("Evolution {0}: New DNA = '{1}'", i, string.Join("", dna.Content));
        }

        class DNA
        {
            public char[] Content { get; private set; }

            public DNA(char[] content)
            {
                Content = new char[content.Length];
                for (int i = 0; i < content.Length; i++)
                {
                    Content[i] = content[i];
                }
            }

            public DNA CrossOver(DNA dna, int splitPoint)
            {
                var newContent = new char[this.Content.Length];
                for (int i = 0; i < newContent.Length; i++)
                {
                    if (i < splitPoint)
                        newContent[i] = this.Content[i];
                    else newContent[i] = dna.Content[i];
                }
                return new DNA(newContent);
            }

            public void Mutate(double mutationRate, char[] materials)
            {
                // mutation
                for (int i = 0; i < Content.Length; i++)
                {
                    var rate = _rnd.NextDouble();
                    if (rate < mutationRate)
                    {
                        var r = _rnd.Next(0, materials.Length);
                        var c = materials[r];
                        Content[i] = c;
                    }
                }
            }

            public int CompareTo(char[] content)
            {
                var count = 0;
                for (int i = 0; i < Content.Length; i++)
                {
                    if (Content[i] == content[i])
                        count++;
                }

                return count;
            }
        }
    }
}
