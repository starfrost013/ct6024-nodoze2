

/// <summary>
/// DominoVisualTypes
/// 
/// Types for 3D Dominoes (static)
/// </summary>
static class DominoVisualTypes
{
    private const int DOMINO_WIDTH = 175;
    private const int DOMINO_HEIGHT = 256;
    private const int BETWEEN_DOMINO_LINES = 257;

    // used for calculating indices
    public const int DOMINO_NUMCOLUMNS = 6, DOMINO_NUMROWS = 6, DOMINO_TOTAL = 36;

    // these are a pain so let's just do these manually
    private const int DOMINO_COLUMN0 = 44, DOMINO_COLUMN1 = 300,
        DOMINO_COLUMN2 = 556, DOMINO_COLUMN3 = 812,
        DOMINO_COLUMN4 = 1024;

    /// <summary>
    /// Defines generic texture extents.
    /// Move somewhere else if we need it
    /// </summary>
    internal struct TextureExtents
    {
        int dominoTop, dominoBotom;
        int x, y;
        int width, height;

        // flip veritcally
        bool flipVert;
    
        public TextureExtents(int domino1, int domino2, int x, int y, int width, int height, bool flipVert = false)
        {
            this.dominoTop = domino1;
            this.dominoBotom = domino2;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.flipVert = flipVert;   
        }
    };

    // badly set up so let's do this shit. nobody here knows how to use any of the tools they are trained on apparently! (4/27/2026)
    internal static TextureExtents[] dominoTextures =
    {
        // 0s
        new(0, 0, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 1, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 2, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 3, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 4, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 5, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(0, 6, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT),

        // 1s
        new(1, 0, DOMINO_COLUMN0, 0, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(1, 1, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(1, 2, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(1, 3, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(1, 4, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(1, 5, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(1, 6, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT),

        // 2s
        new(2, 1, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(2, 2, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(2, 3, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(2, 4, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(2, 5, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(2, 6, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT),

        // 3s
        new(3, 1, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(3, 2, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(3, 3, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(3, 4, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(3, 5, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(3, 6, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT),

        // 4S
        new(4, 1, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(4, 2, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(4, 3, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(4, 4, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(4, 5, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(4, 6, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT),

        // 5s
        new(5, 1, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(5, 2, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(5, 3, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(5, 4, DOMINO_COLUMN3, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(5, 5, DOMINO_COLUMN0, BETWEEN_DOMINO_LINES * 5, DOMINO_WIDTH, DOMINO_HEIGHT),
        new(5, 6, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 5, DOMINO_WIDTH, DOMINO_HEIGHT),
        
        // 6s
        new(6, 1, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 2, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(6, 2, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 3, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(6, 3, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(6, 4, DOMINO_COLUMN4, BETWEEN_DOMINO_LINES * 4, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(6, 5, DOMINO_COLUMN1, BETWEEN_DOMINO_LINES * 5, DOMINO_WIDTH, DOMINO_HEIGHT, true),
        new(6, 6, DOMINO_COLUMN2, BETWEEN_DOMINO_LINES * 5, DOMINO_WIDTH, DOMINO_HEIGHT),
    };


};
