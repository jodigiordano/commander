namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public delegate void ThreadTaskDelegate();
    public delegate void TaskFinishedDelegate(int taskId);

    
    public struct ThreadTask
    {
        public ThreadTaskDelegate Task;
        public TaskFinishedDelegate TaskFinished;
        public int TaskId;


        public ThreadTask(ThreadTaskDelegate task)
            : this(task, null, -1)
        { }


        public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished) :
            this(task, taskFinished, -1)
        { }

        
        public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished, int taskId)
        {
            this.Task = task;
            this.TaskFinished = taskFinished;
            this.TaskId = taskId;
        }
    }


    public class ManagedThread
    {
        public bool Working { get; private set; }

        private Thread Thread;
        private int ProcessorAffinity;
        private bool KillThread;
        private Queue<ThreadTask> Tasks;
        private int TimeToWaitWhenNoTasksMs;


        public ManagedThread(int processorAffinity, int timeToWaitWhenNoTasksMs)
        {
            ProcessorAffinity = processorAffinity;

            TimeToWaitWhenNoTasksMs = timeToWaitWhenNoTasksMs;
            Working = false;

            Init();
        }

        
        public void Kill()
        {
            KillThread = true;
        }


        public void KillImmediately()
        {
            KillThread = true;
            Thread.Abort();
        }


        public void AddTask(ThreadTask task)
        {
            Working = true;

            if (KillThread)
                Init();

            lock (Tasks)
            {
                Tasks.Enqueue(task);
            }
        }


        private void TaskRunner()
        {
            ThreadTask task;

            while (!KillThread)
            {
                if (Tasks.Count > 0)
                {
                    lock (Tasks)
                    {
                        task = Tasks.Dequeue();
                    }

                    task.Task();
                    
                    if (task.TaskFinished != null)
                    {
                        task.TaskFinished(task.TaskId);
                    }
                }

                else
                {
                    Working = false;

                    Thread.Sleep(TimeToWaitWhenNoTasksMs);
                }
            }

            Tasks.Clear();
            Tasks = null;
        }


        private void Init()
        {
            KillThread = false;
            Tasks = new Queue<ThreadTask>();

            ThreadStart Starter = new ThreadStart(TaskRunner);
            Thread = new Thread(Starter);
            Thread.Start();

#if XBOX360
            // Ensure processor affinity is within range (1, 3, 4 or 5)
            if (ProcessorAffinity > 0 && ProcessorAffinity < 6 && ProcessorAffinity != 2)
                Thread.SetProcessorAffinity(new int[] { ProcessorAffinity });
#endif
        }
    }
}
