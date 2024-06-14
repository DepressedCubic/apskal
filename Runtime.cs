using System.Collections.Generic;
using System;

/* The Runtime class is in charge of anything that happens in runtime:
storing variables, printing and executing commands by calling
the appropriate methods from the Parser and Evaluation classes. */
class Runtime {

    /* These four dictionaries contain the values of the currently defined
    variables, of the four possible types (namely: rationals, elements of
    some field Z_p, matrices with rational entries, and matrices with
    entries in some field Z_p). */
    public static Dictionary<string,Rational> Rationals = new();
    public static Dictionary<string,Residue> Residues = new();
    public static Dictionary<string,Matrix<Rational>> RationalMatrices = new();
    public static Dictionary<string,Matrix<Residue>> ResidueMatrices = new();

    /* Runtime.StoreNumber(type, name, p) stores a number of type 'type'
    (namely: Rational or Residue), and, if type = Residue, stores the number
    as a member of the field Z_p.
    NOTE: This includes prompting the user. */
    public static void StoreNumber(Parser.ValueType type, string name, uint modulo=0) {

        string literal = Console.ReadLine()!;

        if (type == Parser.ValueType.Rational) {
            Rationals[name] = new Rational(literal);
        }
        // WARNING: Make sure to point out that
        // this only works for literals up to 2^31 - 1.
        else if (type == Parser.ValueType.Residue) {
            int l = int.Parse(literal);
            uint u = (l < 0) ? (uint)(modulo + l) : (uint)l;
            Residues[name] = new Residue(u, modulo);
        }
        else {
            throw new IncompatibleTypeException();
        }
    }

    /* Runtime.StoreMatrix(t, h, w, n, p) stores a matrix of type 't'
    (namely: RationalMatrix or ResidueMatrix) of height 'h' and
    width 'w'. If type = ResidueMatrix, ensures the entries are in the
    the field Z_p. 
    NOTE: This includes prompting the user. */
    public static void StoreMatrix(
        Parser.ValueType type,
        int height,
        int width, 
        string name, 
        uint modulo=0) {

        if (type == Parser.ValueType.RationalMatrix) {

            Rational[,] entries = new Rational[height, width];

            for (int row = 0; row < height; ++row) {
                string line = Console.ReadLine()!;
                Lexer l = new Lexer(line);
                for (int col = 0; col < width; ++col) {
                    entries[row, col] = new Rational(l.Next(out Lexer.Token x));
                }
            }

            RationalMatrices[name] = new Matrix<Rational>(entries);

        }
        else if (type == Parser.ValueType.ResidueMatrix) {

            Residue[,] entries = new Residue[height, width];
            
            for (int row = 0; row < height; ++row) {
                string line = Console.ReadLine()!;
                Lexer l = new Lexer(line);
                for (int col = 0; col < width; ++col) {
                    string token = l.Next(out Lexer.Token x);
                    entries[row, col] = new Residue(uint.Parse(token), modulo);
                }
            }

            ResidueMatrices[name] = new Matrix<Residue>(entries);

        }
        else {
            throw new IncompatibleTypeException();
        }
    }

    /* Runtime.PrintMatrix<F>(X) prints the matrix X with entries in
    the field F, taking care of all necessary formatting to make it
    readable. */
    public static void PrintMatrix<F>(Matrix<F> M) where F : IField<F> {
        string[,] text = new string[M.Height, M.Width];

        int max_length = 0;
        for (int row = 0; row < M.Height; ++row) {
            for (int col = 0; col < M.Width; ++col) {
                text[row, col] = M.entries[row, col]!.GetString()!;
                if (text[row, col].Length > max_length) {
                    max_length = text[row, col].Length;
                }
            }
        }

        int width = (max_length + 1) * M.Width + 1;

        Console.WriteLine("RESULT: ");
        Console.WriteLine("╭" + new string(' ', width) + "╮");

        for (int row = 0; row < M.Height; ++row) {
            string row_text = "│ ";
            for (int col = 0; col < M.Width; ++col) {
                string entry = text[row, col];
                int entry_width = max_length - entry.Length + 1;
                row_text += text[row, col] + new string(' ', entry_width);
            }
            row_text += "│";

            Console.WriteLine(row_text);
        }

        Console.WriteLine("╰" + new string(' ', width) + "╯");
    }

    /* Runtime.PrintNumber<F>(x) prints the number x from field F. */
    public static void PrintNumber<F>(F x) where F : IField<F> {
        Console.Write("RESULT: ");
        Console.WriteLine(x.GetString());
    }

    /* Runtime.ExecuteCommand(c) takes command 'c', asks the Parser class
    to parse it, and executes it. In particular:
    - If the keyword is 'EXIT', lets the program know that it's ready to exit.
    - If the keyword is 'DEF', either calls StoreNumber or StoreMatrix accordingly.
    - If the keyword is 'EVAL', prompts the user for the expression to
    to be evaluated, calls the appropriate functions from the Evaluation class,
    and prints the result using either PrintMatrix or PrintNumber. */
    public static void ExecuteCommand(string command) {
        Parser.ParseCommand(
            command, 
            out string keyword, 
            out Parser.ValueType type,
            out uint modulo,
            out string name,
            out int height,
            out int width);

        switch (keyword) {
            case "EXIT":
                Apskal.Exit = true;
                break;
            case "DEF":
                switch (type) {
                    case Parser.ValueType.Rational:
                    case Parser.ValueType.Residue:
                        StoreNumber(type, name, modulo);
                        break;
                    case Parser.ValueType.RationalMatrix:
                        StoreMatrix(type, height, width, name);
                        break;
                    case Parser.ValueType.ResidueMatrix:
                        StoreMatrix(type, height, width, name, modulo);
                        break;
                    default:
                        throw new InvalidSyntaxException();
                }
                break;

            case "EVAL":
                string expression = Console.ReadLine()!;
                switch (type) {
                    case Parser.ValueType.Rational:
                        Rational rat = Evaluation.EvaluateRational(
                            expression);
                        PrintNumber<Rational>(rat);
                        break;
                    
                    case Parser.ValueType.Residue:
                        Residue res = Evaluation.EvaluateResidue(
                            expression, 
                            modulo);
                        PrintNumber<Residue>(res);
                        break;

                    case Parser.ValueType.RationalMatrix:
                        Matrix<Rational> rat_matrix = 
                        Evaluation.EvaluateRationalMatrix(expression);
                        PrintMatrix<Rational>(rat_matrix);
                        break;

                    case Parser.ValueType.ResidueMatrix:
                        Matrix<Residue> res_matrix =
                        Evaluation.EvaluateResidueMatrix(expression);
                        PrintMatrix<Residue>(res_matrix);
                        break;

                }
                break;
            default:
                throw new InvalidSyntaxException();
                
        }


    }
}