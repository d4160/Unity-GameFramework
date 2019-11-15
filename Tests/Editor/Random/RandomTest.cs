using NUnit.Framework;
using System.Diagnostics;

namespace d4160.ECS.Tests
{
    public class RandomTest
    {
        private Stopwatch _sw;
        private int _n;

        /* Called before every Test */
        [SetUp]
        public void SetUp()
        {
            _sw = new Stopwatch();

            _n = 1000;
        }

        /* Called after every Test */
        [TearDown]
        public void TearDown()
        {
            _sw = null;
        }

        [Test]
        public void GetUnityRandomInt()
        {
            string s = "";
            int value = -1;
            bool maxReached = false;

            _sw.Restart();

            while (_n > 0)
            {
                value = UnityEngine.Random.Range(0, 10 + 1);
                s += $"{value} ";
                _n--;

                maxReached = !maxReached ? value == 10 : true;
            }

            _sw.Stop();

            UnityEngine.Debug.Log(s);
            UnityEngine.Debug.Log($"{_sw.ElapsedMilliseconds}ms, {_sw.ElapsedTicks}t");

            Assert.True(maxReached);
            Assert.LessOrEqual(value, 10);
            Assert.GreaterOrEqual(value, 0);
        }

        [Test]
        public void GetUnityRandomFloat()
        {
            string s = "";
            float value = -1;

            _sw.Restart();

            while (_n > 0)
            {
                value = UnityEngine.Random.Range(0f, 10f);
                s += $"{value} ";
                _n--;
            }

            _sw.Stop();

            UnityEngine.Debug.Log(s);
            UnityEngine.Debug.Log($"{_sw.ElapsedMilliseconds}ms, {_sw.ElapsedTicks}t");

            Assert.LessOrEqual(value, 10f);
            Assert.GreaterOrEqual(value, 0f);
        }

        [Test]
        public void GetSystemRandomInt()
        {
            System.Random rd = new System.Random();

            string s = "";
            int value = -1;
            bool maxReached = false;

            _sw.Restart();

            while (_n > 0)
            {
                value = rd.Next(0, 10 + 1);
                s += $"{value} ";
                _n--;

                maxReached = !maxReached ? value == 10 : true;
            }

            _sw.Stop();

            UnityEngine.Debug.Log(s);
            UnityEngine.Debug.Log($"{_sw.ElapsedMilliseconds}ms, {_sw.ElapsedTicks}t");

            Assert.True(maxReached);
            Assert.LessOrEqual(value, 10);
            Assert.GreaterOrEqual(value, 0);
        }

        [Test]
        public void GetSystemRandomFloat()
        {
            System.Random rd = new System.Random();

            string s = "";
            float value = -1;

            _sw.Restart();

            while (_n > 0)
            {
                value = (float)rd.NextDouble() * (10 - 0) + 0;
                s += $"{value} ";
                _n--;
            }

            _sw.Stop();

            UnityEngine.Debug.Log(s);
            UnityEngine.Debug.Log($"{_sw.ElapsedMilliseconds}ms, {_sw.ElapsedTicks}t");

            Assert.LessOrEqual(value, 10f);
            Assert.GreaterOrEqual(value, 0f);
        }
    }
}
