using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MFSPikeTest
{
    public class Program
    {
        private const int FactorSize = 200;
        private const double Maximum = 10.0;
        private const double Minimum = -10.0;

        public static void Main(string[] args)
        {
            var nProducts = int.Parse(args[1]);
            var numberOfTests = int.Parse(args[0]);

            //var linqTestResults = new List<long>();
            //var forTestResults = new List<long>();
            var forASTestResults = new List<long>();


            for (var i = 0; i < numberOfTests; i++)
            {
                var testData = GenerateTestDataSingle(i, FactorSize, nProducts);

                Console.Write("\rRunning test {0}", i + 1);

                //var linqResult = RunTest(testData.Key, testData.Value, GetScoresWithLinq);
                //var forResult = RunTest(testData.Key, testData.Value, GetScoresWithNestedFors);
                var forASResult = RunTest(testData.Key, testData.Value, GetScoresWithNestedForsArraySort);

                //linqTestResults.Add(linqResult.Value);
                //forTestResults.Add(forResult.Value);
                forASTestResults.Add(forASResult.Value);
            }

            //Console.WriteLine("GetScoresWithLinq (ns): " + ((double)linqTestResults.Average() / Stopwatch.Frequency) * 1000000);
            //Console.WriteLine("GetScoresWithNestedFors (ns): " + ((double)forTestResults.Average() / Stopwatch.Frequency) * 1000000);
            Console.WriteLine("\r\nGetScoresWithNestedForsArraySort (ms): " + ((double)forASTestResults.Average() / Stopwatch.Frequency) * 1000);
        }

        private static KeyValuePair<double[], long> RunTest(
            double[][] productFactors, double[] customerFactor, Func<double[][], double[], double[]> testMethod)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = testMethod(productFactors, customerFactor);
            stopwatch.Stop();
            return new KeyValuePair<double[], long>(result, stopwatch.ElapsedTicks);
        }

        private static KeyValuePair<double[][], double[]> GenerateTestDataSingle(int seed, int factorSize, int nProducts)
        {
            var random = new Random(seed);

            var customerFactor = GenerateDoubleArray(random, factorSize);

            var productFactors = new double[nProducts][];
            for (var i = 0; i < nProducts; i++)
            {
                productFactors[i] = GenerateDoubleArray(random, factorSize);
            }

            return new KeyValuePair<double[][], double[]>(productFactors, customerFactor);
        }

        private static double[] GenerateDoubleArray(Random random, int factorSize)
        {
            var result = new double[factorSize];

            for (var i = 0; i < factorSize; i++)
            {
                var x = random.NextDouble() * (Maximum - Maximum) + Minimum;
                result[i] = x;
            }

            return result;
        }

        public static double[] GetScoresWithLinq(double[][] productFactors, double[] customerFactor)
        {
            return
                productFactors
                    .Select(
                        productFactor => productFactor
                            .Zip(customerFactor, (f1, f2) => f1 * f2)
                            .Sum())
                    .OrderByDescending(x => x)
                    .ToArray();
        }

        public static double[] GetScoresWithNestedFors(double[][] productFactors, double[] customerFactor)
        {
            var sums = new double[productFactors.Length];
            for (var i = 0; i < productFactors.Length; i++)
            {
                var productFactor = productFactors[i];

                var sum = 0.0;
                for (var j = 0; j < productFactor.Length; j++)
                {
                    sum += productFactor[j] * customerFactor[j];
                }
                sums[i] = sum;
            }

            return sums.OrderByDescending(x => x).ToArray();
        }

        public static double[] GetScoresWithNestedForsArraySort(double[][] productFactors, double[] customerFactor)
        {
            var scores = new double[productFactors.Length];
            for (var i = 0; i < productFactors.Length; i++)
            {
                var productFactor = productFactors[i];

                var sum = 0.0;
                for (var j = 0; j < productFactor.Length; j++)
                {
                    sum += productFactor[j] * customerFactor[j];
                }
                scores[i] = sum;
            }

            Array.Sort(scores);
            Array.Reverse(scores);

            return scores;
        }
    }
}
