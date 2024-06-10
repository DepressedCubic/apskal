using System;

class Lexer {

    public enum Token {
        Special, Literal, Name, None
    }

    static string special = "+*/[]()";

    string text;
    int position;

    public Lexer(string text) {
        this.text = text;
        this.position = 0;
    }

    public string Next(out Token type) {
        
        while ((position < text.Length) && (text[position] == ' ')) {
            position++;
        }

        if (position >= text.Length) {
            type = Token.None;
            return "";
        }

        char c = text[position];

        if (special.Contains(c)) {
            position++;

            type = Token.Special;
            return c.ToString();
        }
        else if (c == '-') {
            if ((position < text.Length - 1) && 
            char.IsDigit(text[position + 1])) {
                string token = "-";
                position++;

                while ((position < text.Length) && 
                char.IsDigit(text[position])) {
                    token += text[position++];
                }

                type = Token.Literal;
                return token;
            }
            else {
                position++;

                type = Token.Special;
                return "-";
            }
        }
        else if (char.IsDigit(c)) {
            string token = "";
            
            while ((position < text.Length) && char.IsDigit(text[position])) {
                token += text[position++];
            }

            type = Token.Literal;
            return token;
        }
        else if (char.IsLetter(c)) {
            string token = "";
            while ((position < text.Length) && char.IsLetter(text[position])) {
                token += text[position++];
            }

            if (token == "rref") {
                type = Token.Special;
            }
            else {
                type = Token.Name;
            }

            return token;
        }
        else {
            throw new UnknownCharacterException(c);
        }

    }
}

class Parser {

    public enum ValueType {
        Rational, Residue, RationalMatrix, ResidueMatrix, None
    }

    static bool IsPrime(uint n) {
        for (uint d = 2; d * d <= n; ++d) {
            if (n % d == 0) {
                return false;
            }
        }

        return true;
    }

    public static void ParseCommand(
        string command, 
        out string keyword, 
        out ValueType type, 
        out uint modulo, 
        out string name,
        out int height,
        out int width) {
            if (command.Length == 0) {
                throw new InvalidSyntaxException();
            }

            modulo = 0;
            height = 0;
            width = 0;
            
            Lexer l = new Lexer(command);
            keyword = l.Next(out Lexer.Token x);

            if (keyword == "EXIT") {
                type = ValueType.None;
                name = "";
                return;
            }

            if (l.Next(out x) is string type_keyword) {
                switch (type_keyword) {
                    case "Q":

                        type = ValueType.Rational;
                        break;

                    case "Z":

                        type = ValueType.Residue;
                        string m1 = l.Next(out x);
                        if (x == Lexer.Token.Literal) {
                            modulo = uint.Parse(m1);

                            if (!IsPrime(modulo)) {
                                throw new CompositeModuloException(modulo);
                            }
                            
                            break;
                        }
                        throw new InvalidSyntaxException();

                    case "Matrix":

                        l.Next(out x);
                        string field = l.Next(out x);
                        switch (field) {
                            case "Q":

                                type = ValueType.RationalMatrix;
                                break;

                            case "Z":

                                type = ValueType.ResidueMatrix;
                                string m2 = l.Next(out x);
                                if (x == Lexer.Token.Literal) {
                                    modulo = uint.Parse(m2);

                                    if (!IsPrime(modulo)) {
                                        throw new CompositeModuloException(modulo);
                                    }
                                    break;
                                }
                                throw new InvalidSyntaxException();

                            default:

                                throw new InvalidSyntaxException();

                        }

                        l.Next(out x);
                        l.Next(out x);
                        string h = l.Next(out Lexer.Token type_h);
                        l.Next(out x);
                        l.Next(out x);
                        string w = l.Next(out Lexer.Token type_w);
                        l.Next(out x);

                        if ((type_h == Lexer.Token.Literal) && 
                        (type_w == Lexer.Token.Literal)) {
                            height = int.Parse(h);
                            width = int.Parse(w);
                        }
                        else {
                            throw new InvalidSyntaxException();
                        }
                        break;

                    default:

                        throw new InvalidSyntaxException();
                        
                }
            }
            else {
                throw new InvalidSyntaxException();
            }

            if (keyword == "EVAL") {
                name = "";
                return;
            }

            name = l.Next(out x);
            if (x != Lexer.Token.Name) {
                throw new InvalidSyntaxException();
            }
        }
}