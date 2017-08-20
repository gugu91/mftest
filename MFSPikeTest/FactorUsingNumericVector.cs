using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MFSPikeTest
{
    public struct FactorUsingNumericVector
    {
        private readonly Vector<float>[] _values;
        private static int VectorSize => Vector<float>.Count;
        public int Length { get; }
        private int ValuesCount { get; }

        public FactorUsingNumericVector(float[] values)
        {
            Length = values.Length;
            ValuesCount = (int) Math.Ceiling((decimal) values.Length / VectorSize);

            _values = new Vector<float>[ValuesCount];
            for (var i = 0; i < ValuesCount; i++)
            {
                var subArray = ExtractFixedLengthSubArray(values, i * VectorSize);
                _values[i] = new Vector<float>(subArray);
            }
        }

        private static float[] ExtractFixedLengthSubArray(float[] values, int at)
        {
            var floats = new float[VectorSize];
            var dataLength = at + VectorSize > values.Length ? values.Length - at : VectorSize;
            for (var i = 0; i < dataLength; i++)
            {
                floats[i] = values[i + at];
            }
            return floats;
        }

        public float DotProduct(FactorUsingNumericVector right)
        {
            var result = 0f;
            for (var i = 0; i < ValuesCount; i++)
            {
                result += Vector.Dot(_values[i], right._values[i]);
            }

            return result;
        }
    }
}
