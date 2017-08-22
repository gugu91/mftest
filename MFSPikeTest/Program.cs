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
        private const float Maximum = 1.0f;
        private const float Minimum = 0.001f;

        public static void Main(string[] args)
        {
            var nProducts = int.Parse(args[1]);
            var numberOfTests = int.Parse(args[0]);

            //var linqTestResults = new List<long>();
            //var forTestResults = new List<long>();
            var forASTestResults = new List<long>();
            var numericsVectorResults = new List<long>();

            Console.WriteLine("Running with the following settings:");
            Console.WriteLine($"- Number of tests: {numberOfTests}");
            Console.WriteLine($"- Number of product factors: {nProducts}");
            Console.WriteLine($"- Factor Size: {FactorSize}");
            Console.WriteLine($"- Vector.IsHardwareAccelerated = {Vector.IsHardwareAccelerated}");
            Console.WriteLine($"- Vector.Count = {Vector<float>.Count}");
            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();

            for (var i = 0; i < numberOfTests; i++)
            {
                var testData = GenerateTestDataSingle(FactorSize, nProducts);
                var floatTestData = 
                    CastTestData(testData, array => new FactorUsingFloatArray(array));
                var vectorTestData =
                    CastTestData(testData, array => new FactorUsingNumericVector(array));

                Console.Write("\rRunning test {0}", i + 1);

                //var linqResult = RunTest(testData.Key, testData.Value, GetScoresWithLinq);
                //var forResult = RunTest(testData.Key, testData.Value, GetScoresWithNestedFors);
                var forASResult = 
                    RunTest(floatTestData.Key, floatTestData.Value, GetScoresWithNestedForsArraySort);
                var numericsVectorResult = 
                    RunTest(vectorTestData.Key, vectorTestData.Value, GetScoresWithSystemNumericsVector);

                //linqTestResults.Add(linqResult.Value);
                //forTestResults.Add(forResult.Value);
                forASTestResults.Add(forASResult.Value);
                numericsVectorResults.Add(numericsVectorResult.Value);
            }

            //Console.WriteLine("GetScoresWithLinq (ns): " + ((float)linqTestResults.Average() / Stopwatch.Frequency) * 1000000);
            //Console.WriteLine("GetScoresWithNestedFors (ns): " + ((float)forTestResults.Average() / Stopwatch.Frequency) * 1000000);
            Console.WriteLine("\r\nGetScoresWithNestedForsArraySort (ms): " + ((float)forASTestResults.Average() / Stopwatch.Frequency) * 1000);
            Console.WriteLine("GetScoresWithSystemNumericsVector (ms): " + ((float)numericsVectorResults.Average() / Stopwatch.Frequency) * 1000);
        }

        private static KeyValuePair<T[], T> CastTestData<T>(KeyValuePair<float[][], float[]> testData, Func<float[], T> factoryMethod)
        {
            return new KeyValuePair<T[], T>(testData.Key.Select(factoryMethod).ToArray(),
                factoryMethod(testData.Value));
        }

        private static KeyValuePair<float[], long> RunTest<T>(
            T[] productFactors, T customerFactor, Func<T[], T, float[]> testMethod)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = testMethod(productFactors, customerFactor);
            stopwatch.Stop();
            return new KeyValuePair<float[], long>(result, stopwatch.ElapsedTicks);
        }

        private static KeyValuePair<float[][], float[]> GenerateTestDataSingle(int factorSize, int nProducts)
        {
            var random = new Random();

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
                var x = random.NextDouble()/(Maximum - Minimum) + Minimum;
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
