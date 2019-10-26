namespace Blackcat.EventBus
{
    public class PostHandled
    {
        public object Data { get; set; }

        public bool Canceled { get; set; }
    }
}