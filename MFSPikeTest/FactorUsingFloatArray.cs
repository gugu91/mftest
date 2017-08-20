using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MFSPikeTest
{
    public class FactorUsingFloatArray
    {
        private readonly float[] _values;

        public int Length => _values.Length;

        public FactorUsingFloatArray(float[] values)
        {
            _values = values;
        }

        public float DotProduct(FactorUsingFloatArray right)
        {
            var sum = 0.0f;
            for (var j = 0; j < Length; j++)
            {
                sum += _values[j] * right._values[j];
            }

            return sum;
        }
    }
}
