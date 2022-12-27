namespace Irc.Bot.Console.Models
{

    public class CompletionsRequest
    {
        public string model { get; set; }
        public string prompt { get; set; }
        public int max_tokens { get; set; }
        public double temperature { get; set; }
        public double top_p { get; set; }
        public int n { get; set; }
        public bool stream { get; set; }
        public object logprobs { get; set; }
        public string stop { get; set; }
    }

}
