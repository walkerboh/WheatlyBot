using System;

namespace WheatlyBot.Entities.Meh
{
    public class Video
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public Topic Topic { get; set; }
    }

    public class Topic
    {
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public int ReplyCount { get; set; }
        public string Url { get; set; }
        public int VoteCount { get; set; }
    }
}