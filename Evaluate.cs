/* The Evaluation class is in charge of the real-time evaluation
of expressions in prefix notation. When variables are encountered,
it attempts to extract their value from the dictionaries in the
Runtime class. */
class Evaluation {

    /* Evaluation.EvaluateRational(e) returns the rational
    number that the string 'e' evaluates to. */
    public static Rational EvaluateRational(string expression) {
        Lexer l = new Lexer(expression);

        Rational Evaluate() {
            string? token = l.Next(out Lexer.Token type_t);

            if (type_t == Lexer.Token.Literal) {
                return new Rational(token);
            }

            if (type_t == Lexer.Token.Name) {
                return Runtime.Rationals[token];
            }

            if (type_t == Lexer.Token.Special) {
                string op = token;
                Rational left = Evaluate();
                Rational right = Evaluate();

                switch (op) {
                    case "+":
                        return left.Add(right);
                    case "-":
                        return left.Subtract(right);
                    case "*":
                        return left.Multiply(right);
                    case "/":
                        return left.Divide(right);
                    default:
                        throw new InvalidSyntaxException();
                }
            }

            throw new InvalidSyntaxException();
        }

        Rational eval = Evaluate();
        l.Next(out Lexer.Token type);
        if (type != Lexer.Token.None) {
            throw new InvalidSyntaxException();
        }

        return eval;

    }

    /* Evaluation.EvaluateResidue(e, p) returns the element in the field
    Z_p that the string 'e' evaluates to. */
    public static Residue EvaluateResidue(string expression, uint modulo) {
        Lexer l = new Lexer(expression);

        Residue Evaluate() {
            string? token = l.Next(out Lexer.Token type_t);

            if (type_t == Lexer.Token.Literal) {
                return new Residue(uint.Parse(token), modulo);
            }

            if (type_t == Lexer.Token.Name) {
                return Runtime.Residues[token];
            }

            if (type_t == Lexer.Token.Special) {
                string op = token;
                Residue left = Evaluate();
                Residue right = Evaluate();

                switch (op) {
                    case "+":
                        return left.Add(right);
                    case "-":
                        return left.Subtract(right);
                    case "*":
                        return left.Multiply(right);
                    case "/":
                        return left.Divide(right);
                    default:
                        throw new InvalidSyntaxException();
                }
            }

            throw new InvalidSyntaxException();
        }

        Residue eval = Evaluate();
        l.Next(out Lexer.Token type);
        if (type != Lexer.Token.None) {
            throw new InvalidSyntaxException();
        }

        return eval;
    } 

    /* Evaluation.EvaluateRationalMatrix(e) returns the matrix of
    rational entries that the string 'e' evaluates to. */
    public static Matrix<Rational> EvaluateRationalMatrix(string expression) {
        Lexer l = new Lexer(expression);

        Matrix<Rational> Evaluate() {
            string? token = l.Next(out Lexer.Token type_t);

            if (type_t == Lexer.Token.Name) {
                return Runtime.RationalMatrices[token];
            }

            if (type_t == Lexer.Token.Special) {
                string op = token;

                if (token == "rref") {
                    Matrix<Rational> child = Evaluate();
                    return child.RREF(out int r, out Rational det);
                }

                Matrix<Rational> left = Evaluate();
                Matrix<Rational> right = Evaluate();

                switch (op) {
                    case "+":
                        return left.Add(right);
                    case "-":
                        return left.Subtract(right);
                    case "*":
                        return left.Multiply(right);
                    default:
                        throw new InvalidSyntaxException();
                }
            }

            throw new InvalidSyntaxException();
        }

        Matrix<Rational> eval = Evaluate();
        l.Next(out Lexer.Token type);
        if (type != Lexer.Token.None) {
            throw new InvalidSyntaxException();
        }

        return eval;
    }

    /* Evaluation.EvaluateResidueMatrix(e) returns the matrix of entries
    in a field Z_p that the string 'e' evaluates to. Note that it doesn't ask
    for the particular p, since the entries of the matrix itself hold this
    information. */
    public static Matrix<Residue> EvaluateResidueMatrix(string expression) {
        Lexer l = new Lexer(expression);

        Matrix<Residue> Evaluate() {
            string? token = l.Next(out Lexer.Token type_t);

            if (type_t == Lexer.Token.Name) {
                return Runtime.ResidueMatrices[token];
            }

            if (type_t == Lexer.Token.Special) {
                string op = token;

                if (token == "rref") {
                    Matrix<Residue> child = Evaluate();
                    return child.RREF(out int r, out Residue det);
                }

                Matrix<Residue> left = Evaluate();
                Matrix<Residue> right = Evaluate();

                switch (op) {
                    case "+":
                        return left.Add(right);
                    case "-":
                        return left.Subtract(right);
                    case "*":
                        return left.Multiply(right);
                    default:
                        throw new InvalidSyntaxException();
                }
            }

            throw new InvalidSyntaxException();
        }

        Matrix<Residue> eval = Evaluate();
        l.Next(out Lexer.Token type);
        if (type != Lexer.Token.None) {
            throw new InvalidSyntaxException();
        }

        return eval;
    }
}