namespace Barriot
{
    public class SelfAssignMessage
    {
        public ulong MessageId { get; set; }

        public ulong ChannelId { get; set; }

        public List<SelfAssignRole> AssignRoles { get; set; }

        internal SelfAssignMessage(RestUserMessage message)
        {
            MessageId = message.Id;
            ChannelId = message.Channel.Id;
            AssignRoles = new();
        }
    }
}
