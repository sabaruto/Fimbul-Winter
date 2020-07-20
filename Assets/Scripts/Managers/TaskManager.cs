using System.Collections;
using UnityEngine;

namespace Managers
{
	/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
	/// It is an error to attempt to start a task that has been stopped or which has
	/// naturally terminated.
	public class Task
  {
	  /// Delegate for termination subscribers.  manual is true if and only if
	  /// the coroutine was stopped with an explicit call to Stop().
	  public delegate void FinishedHandler(bool manual);

    private readonly TaskManager.TaskState task;

    /// Creates a new Task object for the given coroutine.
    /// 
    /// If autoStart is true (default) the task is automatically started
    /// upon construction.
    public Task(IEnumerator c, bool autoStart = true)
    {
      task = TaskManager.CreateTask(c);
      task.Finished += TaskFinished;
      if (autoStart)
        Start();
    }

    /// Returns true if and only if the coroutine is running.  Paused tasks
    /// are considered to be running.
    public bool Running => task.Running;

    /// Returns true if and only if the coroutine is currently paused.
    public bool Paused => task.Paused;

    /// Termination event.  Triggered when the coroutine completes execution.
    public event FinishedHandler Finished;

    /// Begins execution of the coroutine
    public void Start() { task.Start(); }

    /// Discontinues execution of the coroutine at its next yield.
    public void Stop() { task.Stop(); }

    public void Pause() { task.Pause(); }

    public void Unpause() { task.Unpause(); }

    private void TaskFinished(bool manual)
    {
      FinishedHandler handler = Finished;
      handler?.Invoke(manual);
    }
  }

  internal class TaskManager : MonoBehaviour
  {
    private static TaskManager singleton;

    public static TaskState CreateTask(IEnumerator coroutine)
    {
      if (singleton == null)
      {
        GameObject go = new GameObject("TaskManager");
        singleton = go.AddComponent<TaskManager>();
      }

      return new TaskState(coroutine);
    }

    public class TaskState
    {
      public delegate void FinishedHandler(bool manual);

      private readonly IEnumerator coroutine;
      private bool stopped;

      public TaskState(IEnumerator c) { coroutine = c; }

      public bool Running { get; private set; }

      public bool Paused { get; private set; }

      public event FinishedHandler Finished;

      public void Pause() { Paused = true; }

      public void Unpause() { Paused = false; }

      public void Start()
      {
        Running = true;
        singleton.StartCoroutine(CallWrapper());
      }

      public void Stop()
      {
        stopped = true;
        Running = false;
      }

      private IEnumerator CallWrapper()
      {
        yield return null;
        IEnumerator e = coroutine;
        while (Running)
          if (Paused)
          {
            yield return null;
          }
          else
          {
            if (e != null && e.MoveNext())
              yield return e.Current;
            else
              Running = false;
          }

        FinishedHandler handler = Finished;
        handler?.Invoke(stopped);
      }
    }
  }
}