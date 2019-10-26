namespace Blackcat.EventBus
{
    public enum ThreadMode
    {
        /// <summary>
        /// Invokes subscribers immediately
        /// </summary>
        Post,

        /// <summary>
        /// Invokes subscribers in background thread if needed
        /// </summary>
        Thread,

        /// <summary>
        /// Always invokes subsribers in a new background thread
        /// </summary>
        Async,

        /// <summary>
        /// Invokes subscribers in main thread (UI thread) in blocking mode
        /// </summary>
        Main,

        /// <summary>
        /// Invokes subsribers in main thread (UI thread) in non-blocking mode
        /// </summary>
        MainOrder,
    }
}