using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using FluentAssertions;

namespace MFSPikeTest
{
    public class Program
    {
        private const int FactorSize = 200;
        private const float Maximum = 10.0f;
        private const float Minimum = -10.0f;

        public static void Main(string[] args)
        {
            var nProducts = int.Parse(args[1]);
            var numberOfTests = int.Parse(args[0]);

            //var linqTestResults = new List<long>();
            //var forTestResults = new List<long>();
            var forASTestResults = new List<long>();
            var numericsVectorResults = new List<long>();


            for (var i = 0; i < numberOfTests; i++)
            {
                var testData = GenerateTestDataSingle(i, FactorSize, nProducts);

                Console.Write("\rRunning test {0}", i + 1);

                //var linqResult = RunTest(testData.Key, testData.Value, GetScoresWithLinq);
                //var forResult = RunTest(testData.Key, testData.Value, GetScoresWithNestedFors);
                var forASResult = RunTest<FactorUsingFloatArray>(
                    testData.Key.Select(x => new FactorUsingFloatArray(x)).ToArray(), 
                    new FactorUsingFloatArray(testData.Value), GetScoresWithNestedForsArraySort);
                var numericsVectorResult = 
                    RunTest(
                        testData.Key.Select(x => new FactorUsingNumericVector(x)).ToArray(), 
                        new FactorUsingNumericVector(testData.Value), 
                        GetScoresWithSystemNumericsVector);

                numericsVectorResult.Key.Should().ContainInOrder(forASResult.Key);
                //linqTestResults.Add(linqResult.Value);
                //forTestResults.Add(forResult.Value);
                forASTestResults.Add(forASResult.Value);
                numericsVectorResults.Add(numericsVectorResult.Value);
            }

            //Console.WriteLine("GetScoresWithLinq (ns): " + ((float)linqTestResults.Average() / Stopwatch.Frequency) * 1000000);
            //Console.WriteLine("GetScoresWithNestedFors (ns): " + ((float)forTestResults.Average() / Stopwatch.Frequency) * 1000000);
            Console.WriteLine("\r\nGetScoresWithNestedForsArraySort (ms): " + ((float)forASTestResults.Average() / Stopwatch.Frequency) * 1000);
            Console.WriteLine("\r\nGetScoresWithSystemNumericsVector (ms): " + ((float)numericsVectorResults.Average() / Stopwatch.Frequency) * 1000);
        }

        private static KeyValuePair<float[], long> RunTest<T>(
            T[] productFactors, T customerFactor, Func<T[], T, float[]> testMethod)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = testMethod(productFactors, customerFactor);
            stopwatch.Stop();
            return new KeyValuePair<float[], long>(result, stopwatch.ElapsedTicks);
        }

        private static KeyValuePair<float[][], float[]> GenerateTestDataSingle(int seed, int factorSize, int nProducts)
        {
            var random = new Random(seed);

            var customerFactor = GeneratefloatArray(random, factorSize);

            var productFactors = new float[nProducts][];
            for (var i = 0; i < nProducts; i++)
            {
                productFactors[i] = GeneratefloatArray(random, factorSize);
            }

            return new KeyValuePair<float[][], float[]>(productFactors, customerFactor);
        }

        private static float[] GeneratefloatArray(Random random, int factorSize)
        {
            var result = new float[factorSize];

            for (var i = 0; i < factorSize; i++)
            {
                var x = random.NextDouble() * (Maximum - Maximum) + Minimum;
                result[i] = (float)x;
            }

            return result;
        }

        public static float[] GetScoresWithLinq(float[][] productFactors, float[] customerFactor)
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

        public static float[] GetScoresWithNestedFors(float[][] productFactors, float[] customerFactor)
        {
            var sums = new float[productFactors.Length];
            for (var i = 0; i < productFactors.Length; i++)
            {
                var productFactor = productFactors[i];

                var sum = 0.0;
                for (var j = 0; j < productFactor.Length; j++)
                {
                    sum += productFactor[j] * customerFactor[j];
                }
                sums[i] = (float)sum;
            }

            return sums.OrderByDescending(x => x).ToArray();
        }

        public static float[] GetScoresWithNestedForsArraySort(FactorUsingFloatArray[] productFactors, FactorUsingFloatArray customerFactor)
        {
            var scores = new float[productFactors.Length];
            for (var i = 0; i < productFactors.Length; i++)
            {
                scores[i] = customerFactor.DotProduct(productFactors[i]);
            }

            Array.Sort(scores);
            Array.Reverse(scores);

            return scores;
        }

        public static float[] GetScoresWithSystemNumericsVector(FactorUsingNumericVector[] productVectors, FactorUsingNumericVector customerVector)
        {
            var scores = new float[productVectors.Length];
            for (var i = 0; i < productVectors.Length; i++)
            {
                scores[i] = productVectors[i].DotProduct(customerVector);
            }

            Array.Sort(scores);
            Array.Reverse(scores);

            return scores;
        }
    }
}
