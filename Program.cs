using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonitorDemo01
{
    // Tips:
    // A thread can enter a Monitor any number of times.
    //
    // If a thread enters a Monitor x times, it must also exit 
    // the same number of times.
    //
    // If a thread enters a Monitor, it must exit in order for
    // another thread to enter.

    class Program
    {
        static void Main(string[] args)
        {
            Thread[] thd_array = new Thread[2];

            int i = 0;

            for (i = 0; i < thd_array.Length; i++)
            {
                StartThread(ref thd_array[i]);
            }

            for (i = 0; i < thd_array.Length; i++)
            {
                WaitThreadEnd(thd_array[i]);
            }
        }

        static void StartThread(ref Thread thread)
        {
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

        static void ThreadMethod()
        {
            bool bEntered = false;

            try
            {
                // This while loop will keep trying to enter the Monitor
                // until it succeeds.
                //
                // The Monitor class provides a mechanism that synchronizes access
                // to resources or code (much like a Mutex).
                //
                // For more information, please refer to:
                // https://msdn.microsoft.com/en-us/library/system.threading.monitor(v=vs.110).aspx
                while (true)
                {
                    // Using Monitor.TryEnter(), the current thread attempts to
                    // access a set of code (labeled below as "GUARDED CODE").
                    //
                    // It returns a bool value indicating whether or not the
                    // thread has successfully acquired an exclusive lock on
                    // objForThreadSync (i.e. if a lock is available), which
                    // allows it to enter the GUARDED CODE.
                    //
                    // Calling TryEnter() marks the beginning of a critical
                    // section. objForThreadSync is like a key that is used for accessing
                    // a critical section. Its actual type or value is unimportant.
                    //
                    // For more information, please refer to:
                    // https://msdn.microsoft.com/en-us/library/4tssbxcw(v=vs.110).aspx
                    if ((bEntered = Monitor.TryEnter(objForThreadSync)) == true)
                    {
                        // GUARDED CODE.
                        // If the current thread successfully acquires the lock, 
                        // the thread will go to sleep for 5 seconds, before breaking out
                        // of the loop.
                        Console.WriteLine("Thread [{0:D}] has entered the Monitor.",
                            Thread.CurrentThread.ManagedThreadId);
                        Thread.Sleep(5000);
                        break;
                    }
                    else
                    {
                        // If the current thread fails to acquire the lock (e.g. if another
                        // thread has the lock), the thread will go to sleep for 1 second
                        // before trying again when the while-loop reiterates.
                        Console.WriteLine("Thread [{0:D}] unable to enter Monitor. Trying again after 1 second.",
                            Thread.CurrentThread.ManagedThreadId);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on Thread [{0:D}] : [{1:S}].",
                    Thread.CurrentThread.ManagedThreadId,
                    ex.Message);
            }
            finally
            {
                Console.WriteLine("Exiting Thread [{0:D}].",
                    Thread.CurrentThread.ManagedThreadId);

                if (bEntered == true)
                {
                    // A thread must exit the Monitor after
                    // entering so as to allow other threads
                    // to enter.
                    Monitor.Exit(objForThreadSync);
                }
            }
        }

        static void WaitThreadEnd(Thread thread)
        {
            thread.Join();
        }

        // The Monitor class works with various objects
        // to control synchronization.
        //
        // objForThreadSync is the object to be contested between
        // the two threads. Access to this object is controlled
        // via the Monitor class.
        private static object objForThreadSync = new object();
    }
}
