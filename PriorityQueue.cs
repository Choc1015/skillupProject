using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{
    class PriorityQueue<T> where T : IComparable<T> // 비교 가능한 T여야 한다. 
    {
        List<T> _heap = new List<T>();
        // O(logN)
        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다. 
            _heap.Add(data);

            int now = _heap.Count - 1;
            // 도장깨기를 시작
            while (now > 0)
            {
                // 도장꺠기를 시도 
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break; // 현재 값이 부모의 값보다 작기에 끝남.

                // 두 값을 교체한다. 
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp; // 대각선 법칙!

                // 검사 위치를 이동한다. 
                now = next; // 그리고 다시 포문 돌면서 이동가능한지
            }

        }
        // O(logN)
        public T Pop()
        {
            // 반환할 데이터를 따로 저장 
            T ret = _heap[0];

            // 마지막 데이터를 루트노드로 이동한다.
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex); // 마지막 데이터를 루트 노드로 옮겼기에 빈방이 되어 삭제합니다.
            lastIndex--;

            // 역으로 내려가는 도장깨기 시작
            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                // 오른값이 현재값(왼쪽이동을 포함한)보다 크면, 오른쪽으로 이동 
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right; // 이렇게 하는 이유는 왼쪽이랑 비교해서 만약 왼쪽이 커 그러면 그 왼쪽이 값이 넥스트값이되고
                                  // 그 값과 오른쪽 값을 비교해서 왼쪽 오른쪽 중 제일 큰 값이 넥스트 값이 되는 것이여

                // 예외 왼쪽 오른쪽이 현재 보다 작다면 종료
                if (next == now)
                    break;

                // 두 값을 교체한다.  
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                // 검사 위치를 이동한다. 
                now = next;
            }


            return ret;
        }
        public int Count
        {
            get { return _heap.Count; }
        }
    }

}
