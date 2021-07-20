using System.Numerics;

namespace RayTracerLib.Sampler
{
    public class NRooksSampler : AbstractSampler
    {
        public NRooksSampler(int numSamples, int numSteps, float hStep, float vStep)
            : base(numSamples, numSteps, hStep, vStep)
        {
            GenerateSamples();
        }

        public override void GenerateSamples()
        {
            // Generate samples among main diagonal
            for (int p = 0; p < _numSets; ++p)
            {
                for (int j = 0; j < _numSamples; ++j)
                {
                    float hRnd = Util.RandomNumberGenerator.RandFloatInRange(0f, _hStep - 1e-5f);
                    float vRnd = Util.RandomNumberGenerator.RandFloatInRange(0f, _vStep - 1e-5f);
                    //float x = (j + hRnd) / (float)_numSamples;
                    //float y = (j + vRnd) / (float)_numSamples;
                    float x = (j * _hStep + hRnd) / (float)_numSamples;
                    float y = (j * _vStep + vRnd) / (float)_numSamples;

                    _samples.Add(new Vector2(x, y));
                }
            }

            ShuffleXCoordinates();
            ShuffleYCoordinates();
        }

        private void ShuffleXCoordinates()
        {
            for (int p = 0; p < _numSets; ++p)
            {
                for (int i = 0; i < _numSamples - 1; ++i)
                {
                    // Calculate current index in samples collection
                    int currentIndex = i + p * _numSamples + 1;

                    // Determine random index in current set
                    int target = Util.RandomNumberGenerator.RandIntInRange(0, int.MaxValue) % _numSamples + p * _numSamples;

                    //Debug.Log("ShuffleXCoordinates - SamplesSize: " + _samples.Count);
                    //Debug.Log("ShuffleXCoordinates - SamplesColl[Idx]: " + currentIndex);

                    // Cache old x value of current index
                    float temp = _samples[currentIndex].X;

                    // Write x value of target index in current index
                    _samples[currentIndex] = new Vector2(_samples[target].X, _samples[currentIndex].Y); // .Set(_samples[target].x, _samples[currentIndex].y);

                    // Write cached x value into target index
                    _samples[target] = new Vector2(temp, _samples[target].Y); //.Set(temp, _samples[target].y);

                }
            }
        }

        private void ShuffleYCoordinates()
        {
            for (int p = 0; p < _numSets; ++p)
            {
                for (int i = 0; i < _numSamples - 1; ++i)
                {
                    // Calculate current index in samples collection
                    int currentIndex = i + p * _numSamples + 1;

                    // Determine random index in current set
                    int target = Util.RandomNumberGenerator.RandIntInRange(0, int.MaxValue) % _numSamples + p * _numSamples;

                    // Cache old x value of current index
                    float temp = _samples[currentIndex].Y;

                    // Write x value of target index in current index
                    _samples[currentIndex] = new Vector2(_samples[currentIndex].X, _samples[target].Y);
                    //.Set(_samples[currentIndex].x, _samples[target].y);

                    // Write cached x value into target index
                    _samples[target] = new Vector2(_samples[target].X, temp); //.Set(_samples[target].x, temp);
                }
            }
        }


    }

}
