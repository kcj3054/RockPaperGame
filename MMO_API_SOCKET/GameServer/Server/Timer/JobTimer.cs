using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Timer
{
    struct JObTimerElement : IComparable<JObTimerElement>
    {
        public int execTick; // 실행 시간 
        public Action action;

        public int CompareTo(JObTimerElement other) 
        {
            return other.execTick - this.execTick;
        }
    }

    class JobTimer 
    {
        PriorityQueue<JObTimerElement, int> pq = new();
        public object timerLock = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">수행할 action</param>
        /// <param name="tickAfter">몇초뒤에 실행할 것인지</param>
        public void Push(Action action, int tickAfter = 0)
        {
            JObTimerElement job;
            //System.Environment.TickCount 현재 시스템 시간 
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;


            lock(timerLock)
            {
                pq.Enqueue(job, 1);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JObTimerElement jOb;

                lock(timerLock)
                {
                    if(pq.Count == 0) 
                    {
                        break;
                    }

                    jOb = pq.Peek();
                    if(jOb.execTick > now) 
                    {
                        // 설명 추가 
                        break;
                    }

                    pq.Dequeue();
                }

                jOb.action.Invoke();
            }
        }
    }
}
