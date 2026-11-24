using System;
using System.Threading.Tasks;

namespace DailyDuty.Extensions;

/// <summary>
/// Extensions for handling IAsyncDispose a little easier.
/// </summary>
public static class AsyncDisposableExtensions {
    extension(IAsyncDisposable? asyncDisposable) {

        /// <summary>
        /// Safe wrapper around async dispose for nullables.
        /// Checks if the provided reference is null before attempting to dispose.
        /// </summary>
        public async Task DisposeAsyncSafe() {
            if (asyncDisposable is null) return;

            await asyncDisposable.DisposeAsync();
        }
    }
}
