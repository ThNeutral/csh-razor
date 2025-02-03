namespace razor {
    struct GameMetadata (int width, int height, int numberOfPlayers) {
        public int Width {get; set;} = width;
        public int Height {get; set;} = height;
        public int NumberOfPlayers {get; set;} = numberOfPlayers;
    }
}