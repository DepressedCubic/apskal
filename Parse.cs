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

            type = Token.Name;           
            return token;
        }
        else {
            throw new UnknownCharacterException(c);
        }

    }
}

class Parser {

    public enum ValueType {
        Rational, Residue, RationalMatrix, ResidueMatrix
    }

    static string[] Functions = { "rref", "det" };
    public abstract class Node {}

    class Literal : Node {
        string name;
        ValueType type;

        public Literal(string name, ValueType type) {
            this.name = name;
            this.type = type;
        }
    }

    class Unary : Node {
        string op;
        Node child;

        public Unary(string op, Node child) {
            this.op = op;
            this.child = child;
        }
    }

    class Binary : Node {
        string op;
        Node left, right;

        public Binary(string op, Node left, Node right) {
            this.op = op;
            this.left = left;
            this.right = right;
        }
    }

    static void ParseCommand(
        string command, 
        out string keyword, 
        out ValueType type, 
        out uint modulo, 
        out string name) {
            if (command.Length == 0) {
                throw new InvalidSyntaxException();
            }

            modulo = 0;
            
            Lexer l = new Lexer(command);
            keyword = l.Next(out Lexer.Token x);

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
                            break;
                        }
                        throw new InvalidSyntaxException();
                    case "Matrix":
                        l.Next(out x);
                        string field = l.Next(out x);
                        switch (field) {
                            case "Q":
                                type = ValueType.RationalMatrix;
                                l.Next(out x);
                                break;
                            case "Z":
                                type = ValueType.ResidueMatrix;
                                string m2 = l.Next(out x);
                                if (x == Lexer.Token.Literal) {
                                    modulo = uint.Parse(m2);
                                    l.Next(out x);
                                    break;
                                }
                                throw new InvalidSyntaxException();
                            default:
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

            name = l.Next(out x);
            if (x != Lexer.Token.Name) {
                throw new InvalidSyntaxException();
            }
        }

    static Node GenerateTree(ValueType type, string expression, uint modulo=0) {
        Lexer l = new Lexer(expression);

        Node Parse(ValueType current_type) {
            string? token = l.Next(out Lexer.Token type_t);

            if (type_t == Lexer.Token.Literal) {
                string name = Runtime.StoreNumber(current_type, token, modulo);
                return new Literal(name, current_type);
            }

            if (type_t == Lexer.Token.Name) {
                switch (token) {
                    case "det":
                        if (current_type == ValueType.Rational) {
                            Node c = Parse(ValueType.RationalMatrix);
                            return new Unary("det", c);
                        }
                        else if (current_type == ValueType.Residue) {
                            Node c = Parse(ValueType.ResidueMatrix);
                            return new Unary("det", c);
                        }
                        else {
                            throw new IncompatibleTypeException();
                        }
                    case "rref":
                        Node child = Parse(current_type);
                        return new Unary("rref", child);
                    default:
                        return new Literal(token, current_type);

                }
            }

            if (token == "(") {
                Node left = Parse(current_type);
                string op = l.Next(out Lexer.Token x);
                Node right = Parse(current_type);
                if (l.Next(out x) != ")") {
                    throw new InvalidSyntaxException();
                }

                return new Binary(op, left, right);
            }

            throw new InvalidSyntaxException();
        }

        return Parse(type);
    }
}