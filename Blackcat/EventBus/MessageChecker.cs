using System;

namespace Blackcat.EventBus
{
    internal class MessageChecker
    {
        private readonly MatchedMode matchedMode;

        public MessageChecker(MatchedMode matchedMode)
        {
            this.matchedMode = matchedMode;
        }

        public bool IsMatched(object message, Type checkType)
        {
            return matchedMode == MatchedMode.ExactlyType
                ? checkType == message.GetType()
                : checkType.IsInstanceOfType(message);
        }
    }
}