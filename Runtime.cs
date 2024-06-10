using System.Collections.Generic;
using System;

class Runtime {

    public static Dictionary<string,Rational> Rationals = new();
    public static Dictionary<string,Residue> Residues = new();
    public static Dictionary<string,Matrix<Rational>> RationalMatrices = new();
    public static Dictionary<string,Matrix<Residue>> ResidueMatrices = new();

    public static void StoreNumber(Parser.ValueType type, string name, uint modulo=0) {

        string literal = Console.ReadLine()!;

        if (type == Parser.ValueType.Rational) {
            string[] q = literal.Split('/');
            Integer num = new Integer(q[0]);
            Integer denom = new Integer(q[1]);
            Rationals[name] = new Rational(num, denom);
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

    public static void PrintNumber<F>(F x) where F : IField<F> {
        Console.Write("RESULT: ");
        Console.WriteLine(x.GetString());
    }

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

                throw new InvalidSyntaxException();
                
        }


    }
}