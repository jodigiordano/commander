namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    #region Delegates

    /// <summary>
    /// A threaded task
    /// </summary>
    public delegate void ThreadTaskDelegate();

    /// <summary>
    /// A method to be called when a task finishes
    /// </summary>
    /// <param name="taskId">The task identifier for this task (-1 by default)</param>
    public delegate void TaskFinishedDelegate(int taskId);

    #endregion

    /// <summary>
    /// A task to thread
    /// </summary>
    public struct ThreadTask
    {
        #region Members

        /// <summary>
        /// The task this thread runs
        /// </summary>
        private ThreadTaskDelegate task;

        /// <summary>
        /// Gets the task this thread runs
        /// </summary>
        public ThreadTaskDelegate Task { get { return task; } }

        /// <summary>
        /// The method called when the task is finished
        /// </summary>
        private TaskFinishedDelegate taskFinished;

        /// <summary>
        /// Gets the task finished method
        /// </summary>
        public TaskFinishedDelegate TaskFinished { get { return taskFinished; } }

        /// <summary>
        /// A task identifier (-1 by default)
        /// </summary>
        private int taskId;

        /// <summary>
        /// Gets the task identifier of this task (-1 by default)
        /// </summary>
        public int TaskId { get { return taskId; } }

        #endregion

        /// <summary>
        /// Initializes a thread task
        /// </summary>
        /// <param name="task">the method to call</param>
        /// <param name="taskFinished">The method to call when finished</param>
        /// <param name="taskId">An identifier for this task</param>
        public ThreadTask(
            ThreadTaskDelegate task,
            TaskFinishedDelegate taskFinished,
            int taskId)
        {
            this.task = task;
            this.taskFinished = taskFinished;
            this.taskId = taskId;
        }

        /// <summary>
        /// Initializes a thread task
        /// </summary>
        /// <param name="task">the method to call</param>
        /// <param name="taskFinished">The method to call when finished</param>
        public ThreadTask(
            ThreadTaskDelegate task,
            TaskFinishedDelegate taskFinished) :
            this(task, taskFinished, -1)
        {
        }

        /// <summary>
        /// Initializes a thread task
        /// </summary>
        /// <param name="task">the method to call</param>
        public ThreadTask(ThreadTaskDelegate task)
            : this(task, null, -1)
        {
        }
    }

    /// <summary>
    /// A managed thread
    /// </summary>
    public class ManagedThread
    {
        #region Members

        /// <summary>
        /// The thread managed by this object
        /// </summary>
        private Thread thread;

        /// <summary>
        /// The processor this thread should run on
        /// </summary>
        private int processorAffinity;

        /// <summary>
        /// True when this thread should be killed
        /// </summary>
        private bool killThread;

        /// <summary>
        /// Tasks to be run by this thread
        /// </summary>
        private Queue<ThreadTask> tasks;

        /// <summary>
        /// Vrai quand le thread a des taches à effectuer.
        /// </summary>
        private bool travaille = false;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the thread with a specified processor affinity
        /// </summary>
        public ManagedThread(int processorAffinity)
        {
            this.processorAffinity = processorAffinity;

            init();
        }

        /// <summary>
        /// Initializes the thread
        /// </summary>
        public ManagedThread()
            : this(-1)
        { }

        /// <summary>
        /// Runs tasks when there are any to run
        /// </summary>
        private void taskRunner()
        {
            #region Set processor
#if XBOX360
            // Ensure processor affinity is within range (1, 3, 4 or 5)
            if (processorAffinity > 0 && processorAffinity < 6 &&
                processorAffinity != 2)
            {
                // Set the processor affinity
                thread.SetProcessorAffinity(new int[] { processorAffinity });
            }	
#endif
            #endregion

            #region Run Tasks

            // The task to run
            ThreadTask task;

            while (!killThread)
            {
                if (tasks.Count > 0)
                {
                    // Run the task
                    lock (tasks)
                    {
                        task = tasks.Dequeue();
                    }
                    task.Task();
                    if (task.TaskFinished != null)
                    {
                        task.TaskFinished(task.TaskId);
                    }
                }
                else
                {
                    travaille = false;

                    // Wait for more tasks to run
                    Thread.Sleep(0);
                }
            }

            #endregion

            // Clean up
            tasks.Clear();
            tasks = null;
        }

        /// <summary>
        /// Kills this thread, stopping at next safe point
        /// </summary>
        public void Kill()
        {
            killThread = true;
        }

        /// <summary>
        /// Kills the thread immediately
        /// </summary>
        public void KillImmediately()
        {
            killThread = true;
            thread.Abort();
        }

        /// <summary>
        /// Adds a task to this thread
        /// </summary>
        /// <param name="task">The task to perform</param>
        public void AddTask(ThreadTask task)
        {
            travaille = true;

            if (killThread)
                init();

            lock (tasks)
            {
                tasks.Enqueue(task);
            }
        }

        /// <summary>
        /// Est-ce que le thread a de la job ?
        /// </summary>
        /// <returns></returns>
        public bool Travaille()
        {
            return travaille;
        }

        #endregion

        private void init()
        {
            killThread = false;
            tasks = new Queue<ThreadTask>();

            // Create the starter object
            ThreadStart starter = new ThreadStart(taskRunner);
            thread = new Thread(starter);
            thread.Start();
        }
    }
}
