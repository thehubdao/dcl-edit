using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockEventActionListener
    {
        public int CalledCount = 0;
        private readonly List<MockActionParams> _params = new List<MockActionParams>();

        public IEnumerable<T> GetParams<T>()
        {
            return _params.Select(p => ((MockActionParam<T>) p).Param);
        }

        public T LastParam<T>()
        {
            return ((MockActionParam<T>) (_params.FindLast(_ => true))).Param;
        }

        public void Called()
        {
            CalledCount++;
        }

        public void Called<T>(T param)
        {
            CalledCount++;
            _params.Add(new MockActionParam<T>(param));
        }

        public IEnumerable WaitForNextAction()
        {
            yield return WaitForActionCount(CalledCount + 1);
        }

        public IEnumerator WaitForActionCount(int count)
        {
            while (CalledCount < count)
            {
                if (Input.GetKeyDown(KeyCode.L))
                    yield break;

                yield return null;
            }
        }
    }

    public class MockActionParams
    {
    }

    public class MockActionParam<T> : MockActionParams
    {
        public MockActionParam(T param)
        {
            Param = param;
        }

        public T Param;
    }
}
