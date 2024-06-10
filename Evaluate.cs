class Evaluation {

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

        return Evaluate();
    }

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

        return Evaluate();
    } 

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

        return Evaluate();
    }

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

        return Evaluate();
    }
}