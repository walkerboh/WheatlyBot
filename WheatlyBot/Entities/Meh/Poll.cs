using System;
using System.Collections.Generic;

namespace WheatlyBot.Entities.Meh
{
    public class Poll
    {
        public IEnumerable<Answer> Answers { get; set; }
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public string Title { get; set; }
        public PollTopic Topic { get; set; }
    }

    public class PollTopic
    {
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public int ReplyCount { get; set; }
        public string Url { get; set; }
        public int VoteCount { get; set; }
    }

    public class Answer
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public int VoteCount { get; set; }
    }
}