namespace razor {
    static class Constants {
        public static string BASE_HTTP_URL {get;} = "http://localhost:5168";
        public static string BASE_WS_URL {get;} = "ws://localhost:5168";
        public static string GAME_PAGE_PATH {get;} = "/";
        public static string NOT_FOUND_PAGE_PATH {get;} = "/not-found";
        public static List<string> PAGE_PATHS {get;} = [GAME_PAGE_PATH, NOT_FOUND_PAGE_PATH];
        public static string WS_HUB_PATH {get;} = "/ws";
        public static Uri WS_HUB_URI {get => new(BASE_WS_URL + WS_HUB_PATH);}
        public static string REGISTRATION_PATH {get;} = "/registration";
        public static Uri REGISTRATION_URI {get => new(BASE_HTTP_URL + REGISTRATION_PATH);} 
        public static string GAME_METADATA_PATH {get;} = "/metadata";
        public static Uri GAME_METADATA_URI {get => new(BASE_HTTP_URL + GAME_METADATA_PATH);} 
        public static string GAME_STATE_PATH {get;} = "/state";
        public static Uri GAME_STATE_URI {get => new(BASE_HTTP_URL + GAME_STATE_PATH);}
        public static string TOKEN_HEADER_NAME {get;} = "token";
        public static string TOKEN_ITEM_NAME {get;} = "token";
        public static int NUMBER_OF_PLAYERS {get;} = 2;
        public static int GAME_WIDTH {get;} = 800;
        public static int GAME_HEIGHT {get;} = 600;
        public static float TARGET_LOOP_DELAY {get;} = 16.6f;
    }
}