using System;

namespace TwitterAuth {

    public static class TokenManagerManager {
        private static InMemoryTokenManager _instance = null;

        public static InMemoryTokenManager Create(string consumerKey, string consumerSecret) {
            if (_instance == null) {
                _instance = new InMemoryTokenManager(consumerKey, consumerSecret);
            }
            Console.WriteLine(_instance.GetHashCode());
            return _instance;
        }
    }
}