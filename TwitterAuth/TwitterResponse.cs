namespace TwitterAuth {
    public interface ITwitterResponse {

        string Logon { get; set; }

        string AccessToken { get; set; }

        string AccessSecret { get; set; }
    }

    public class TwitterResponse : ITwitterResponse {

        public string Logon { get; set; }

        public string AccessToken { get; set; }

        public string AccessSecret { get; set; }
    }
}