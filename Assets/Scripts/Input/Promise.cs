namespace CrossInput.ExtensionMethods
{
    using System;
    using System.Threading.Tasks;
    public static class Promise
    {
        public static Task<TResult> Then<TResult>(this Task<TResult> task, Func<Task<TResult>, TResult> continuationFunction) {
            return task.ContinueWith(continuationFunction);
        }

        public static Task Then(this Task task, Action<Task> continuationAction) {
            return task.ContinueWith(continuationAction);
        }
    }
}