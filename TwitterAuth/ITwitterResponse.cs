namespace TwitterAuth
{
    public interface ITwitterResponse {

        string Logon { get; set; }

        string AccessToken { get; set; }

        string AccessSecret { get; set; }
    }
}