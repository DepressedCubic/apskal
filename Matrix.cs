using System.Collections.Generic;


/* Generic class implementing matrices with
entries in a field F, internally represented
as a rectangular array. */
class Matrix<F> where F : IField<F> {
    int width;
    int height;

    public F[,] entries;

    /* The property X.Height returns the height of
    matrix X */
    public int Height {
        get { 
            return height;
        }
    }

    /* The property X.Width returns the width of
    matrix X. */
    public int Width {
        get {
            return width;
        }
    }

    /* X.SwapRows(i,j) swaps rows i and j
    of X. 
    In other words: R_i <-> R_j. */
    private void SwapRows(int i, int j) {
        for (int k = 0; k < this.width; ++k) {
            F temp = this.entries[i, k];
            this.entries[i, k] = this.entries[j, k];
            this.entries[j, k] = temp;
        }
    }

    /* X.MultiplyRow(a, i) multiplies row i by the
    scalar a, which is a member of the same field as
    the entries of X. 
    In other words: R_i -> a * R_i.*/
    private void MultiplyRow(F factor, int i) {
        for (int k = 0; k < this.width; ++k) {
            this.entries[i, k] = this.entries[i, k].Multiply(factor);
        }
    }

    /* X.AddScalarMultiply(i, a, j) adds the result of
    multiplying row j by scalar a to row i.
    In other words: R_i -> R_i + a * R_j. */
    private void AddScalarMultiple(int i, F factor, int j) {
        for (int k = 0; k < this.width; ++k) {
            F add = this.entries[j, k].Multiply(factor);
            this.entries[i, k] = this.entries[i, k].Add(add);
        }
    }

    /* Constructor: Given a rectangular array
    of elements of some field F, returns the
    corresponding matrix. */
    public Matrix(F[,] entries) {
        this.entries = entries;
        this.height = entries.GetLength(0);
        this.width = entries.GetLength(1);
    }

    /* X.Clone() creates a shallow clone of
    matrix X. */
    public Matrix<F> Clone() {
        F[,] clone = new F[this.height, this.width];
        for (int i = 0; i < this.height; ++i) {
            for (int j = 0; j < this.width; ++j) {
                clone[i, j] = this.entries[i, j];
            }
        }
        return new Matrix<F>(clone);
    }

    /* X.Add(Y) returns X + Y, provided that they
    have compatible dimensions. */
    public Matrix<F> Add(Matrix<F> M) {
        if ((this.width != M.width) || (this.height != M.height)) {
            throw new IncompatibleDimensionsException();
        }
        else {
            F[,] sum = new F[this.height, this.width];
            for (int i = 0; i < this.height; ++i) {
                for (int j = 0; j < this.width; ++j) {
                    sum[i, j] = 
                    (this.entries[i, j]).Add(M.entries[i, j]);
                }
            }
            return new Matrix<F>(sum);
        }
    }

    /* X.Multiply(Y) returns X * Y (note: not commutative),
    provided that they have compatible dimensions. */
    public Matrix<F> Multiply(Matrix<F> M) {
        if (this.width != M.height) {
            throw new IncompatibleDimensionsException();
        }
        else {
            F[,] product = new F[this.height, M.width];
            for (int i = 0; i < this.height; ++i) {
                for (int j = 0; j < M.width; ++j) {

                    F sum = this.entries[0,0].GetZero();
                    for (int k = 0; k < this.width; ++k) {
                        F a = this.entries[i, k];
                        F b = M.entries[k, j];
                        sum = sum.Add(a.Multiply(b));
                    }

                    product[i,j] = sum;
                }
            }

            return new Matrix<F>(product);
        }
    }

    /* X.MultiplyScalar(a) returns the matrix
    that results from multiplying each entry of X
    by scalar a, which must be in the same field as
    the entries of X. */
    public Matrix<F> MultiplyScalar(F a) {
        Matrix<F> m = this.Clone();
        for (int row = 0; row < height; ++row) {
            m.MultiplyRow(a, row);
        }

        return m;
    }

    /* X.Negation() returns the matrix -X;
    i.e. the one that results from negating each
    entry of X. */    
    public Matrix<F> Negation() {
        Matrix<F> negation = this.Clone();
        negation = negation.MultiplyScalar(
            this.entries[0,0].GetOne().Negation());
        return negation;
    }

    /* X.Subtract(Y) returns X - Y, provided
    that they have compatible dimensions. */
    public Matrix<F> Subtract(Matrix<F> M) {
        Matrix<F> negation = this.Negation();
        return this.Add(negation);
    }

    /* X.RREF(...,...) returns the reduced row echelon
    form of X, computed through the row reduction
    algorithm. The out parameter 'rank' provides rank(X),
    and the out parameter 'determinant' provides det(X). */
    public Matrix<F> RREF(out int rank, out F determinant) {
        int row = 0;
        rank = 0;

        determinant = this.entries[0,0].GetOne();

        Matrix<F> rref = this.Clone();
        List<(int, int)> pivots = new();

        for (int col = 0; col < rref.width; ++col) {
            for (int i = row; i < rref.height; ++i) {
                if (!rref.entries[i, col].IsZero) {

                    rref.SwapRows(row, i);
                    if (row != i) {
                        determinant = determinant.Negation();
                    }

                    F pivot = rref.entries[row, col];
                    for (int j = row + 1; j < rref.height; ++j) {
                        F entry = rref.entries[j, col];
                        F factor = (entry.Divide(pivot)).Negation();
                        rref.AddScalarMultiple(j, factor, row);
                    }
                    rank++;
                    pivots.Add((row, col));
                    row++;
                    break;
                }
            }
        }

        pivots.Reverse();
        foreach ((int r, int c) in pivots) {
            F entry = rref.entries[r, c];

            determinant = determinant.Multiply(entry);
            rref.MultiplyRow(entry.Reciprocal(), r);

            for (int i = 0; i < r; ++i) {
                rref.AddScalarMultiple(i, rref.entries[i, c].Negation(), r);
            }
        }

        if (!((this.height == rank) && (this.width == rank))) {
            determinant = determinant.GetZero();
        }

        return rref;
    }
}