namespace Barriot.Entities.Polls
{
    public class PollOption
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int Votes { get; set; }

        public PollOption(int index, string label)
        {
            Id = index;
            Label = label;
            Votes = 0;
        }
    }
}
