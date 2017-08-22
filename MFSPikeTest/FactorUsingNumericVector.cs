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
        private readonly int _valuesCount;

        private static int VectorSize => Vector<float>.Count;

        public int Length { get; }

        public float[] Values {
            get
            {
                var retValue = new float[Length];
                for (var i = 0; i < _valuesCount; i++)
                {
                    for (var j = 0; j < VectorSize; j++)
                    {
                        retValue[j] = _values[i][j];
                    }
                }

                return retValue;
            }
        }

        public FactorUsingNumericVector(float[] values)
        {
            Length = values.Length;
            _valuesCount = (int) Math.Ceiling((decimal) values.Length / VectorSize);

            _values = new Vector<float>[_valuesCount];
            for (var i = 0; i < _valuesCount; i++)
            {
                _values[i] = new Vector<float>(values, i * VectorSize);
            }
        }

        public float DotProduct(FactorUsingNumericVector right)
        {
            var result = 0f;
            for (var i = 0; i < _valuesCount; i++)
            {
                result += Vector.Dot(_values[i], right._values[i]);
            }

            return result;
        }
    }
}
