using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datos.ServiceOperation
{
    public static class TaskExtension
    {
        /// <summary>
        /// Extension Method : Task with stop async task. Generic Type
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
                var taskCompleted = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (taskCompleted == task) {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;
                }
                else {
                    throw new TimeoutException("The operation has been timed out.");
                }
            }
        }
        /// <summary>
        /// Extension Method : Task with stop async task, void example
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
                var taskCompleted = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (taskCompleted == task) {
                    timeoutCancellationTokenSource.Cancel();
                    await task;
                }
                else {
                    throw new TimeoutException("The operation has been timed out.");
                }
            }
        }

    }
}
