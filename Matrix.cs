using System.Collections.Generic;

class Matrix<F> where F : IField<F> {
    int width;
    int height;

    F[,] entries;

    private void SwapRows(int i, int j) {
        for (int k = 0; k < this.width; ++k) {
            F temp = this.entries[i, k];
            this.entries[i, k] = this.entries[j, k];
            this.entries[j, k] = temp;
        }
    }

    private void MultiplyRow(F factor, int i) {
        for (int k = 0; k < this.width; ++k) {
            this.entries[i, k] = this.entries[i, k].Multiply(factor);
        }
    }

    private void AddScalarMultiple(int i, F factor, int j) {
        for (int k = 0; k < this.width; ++k) {
            F add = this.entries[j, k].Multiply(factor);
            this.entries[i, k] = this.entries[i, k].Add(add);
        }
    }

    public Matrix(F[,] entries) {
        this.entries = entries;
    }

    public Matrix<F> Clone() {
        F[,] clone = new F[this.height, this.width];
        for (int i = 0; i < this.height; ++i) {
            for (int j = 0; j < this.width; ++j) {
                clone[i, j] = this.entries[i, j];
            }
        }
        return new Matrix<F>(clone);
    }

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

    // computes reduced row echelon form of a matrix
    public Matrix<F> RREF(out int rank) {
        int row = 0;
        rank = 0;

        Matrix<F> rref = this.Clone();
        List<(int, int)> pivots = new();

        for (int col = 0; col < rref.width; ++col) {
            for (int i = row; i < rref.height; ++i) {
                if (!rref.entries[i, col].IsZero) {
                    rref.SwapRows(row, i);
                    F pivot = rref.entries[row, col];
                    for (int j = row + 1; j < rref.height; ++j) {
                        F entry = rref.entries[j, col];
                        F factor = (entry.Divide(pivot)).Negation();
                        rref.AddScalarMultiple(j, factor, row);
                    }
                    rank++;
                    pivots.Add((row, col));
                    break;
                }
            }
        }

        pivots.Reverse();
        foreach ((int r, int c) in pivots) {
            F entry = rref.entries[r, c];
            rref.MultiplyRow(entry.Reciprocal(), r);
            for (int i = 0; i < r; ++i) {
                rref.AddScalarMultiple(i, rref.entries[i, c].Negation(), r);
            }
        }

        return rref;
    }
}