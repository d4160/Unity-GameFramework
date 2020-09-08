namespace d4160.Core
{
    using System;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;

    public static class AwaitExtensions
    {
        public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan).GetAwaiter();
        }

        /// <summary>
        /// Allow Unity wrap errors on TAP
        /// </summary>
        /// <param name="task"></param>
        public static async void WrapErrors(this Task task)
        {
            await task;
        }
    }
}