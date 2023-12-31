using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    //재귀적 락 허용O     WirteLock->WriteLock O, WriteLock->ReadLock O, ReadLock->WriteLock X
    //스핀락 정책(5000번->Yield)
    class Lock
    {
        private const int EMPTY_FLAG = 0x00000000;
        private const int WRITE_MASK = 0x7FFF0000;
        private const int READ_MASK = 0x0000FFFF;
        private const int MAX_SPIN_COUNT = 5000;

        //[Unused(1)] [WriteThredId(15)] [ReadCount(16)]
        int _flag=EMPTY_FLAG;
        private int _writeCount = 0;

        public void WriteLock()
        {
            //동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId=(_flag & WRITE_MASK)>>16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId) 
            {
                _writeCount++;
                return;
            }


            //아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을얻는다
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if ((Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG))
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield();
            }
        }

        public void WriteUnLock()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
            {
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
            }
        }

        public void ReadLock()
        {
            //동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }


            //아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
                

                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}
