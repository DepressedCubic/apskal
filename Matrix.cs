class Matrix<F> where F : IField<F> {
    int width;
    int height;

    F[,] entries;

    public Matrix() {
        entries = new F[height, width];
        for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
                
            }
        }
    }
}