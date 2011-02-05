namespace TwitterAuth {
    public class TwitterResponse : ITwitterResponse {

        public string Logon { get; set; }

        public string AccessToken { get; set; }

        public string AccessSecret { get; set; }
    }
}