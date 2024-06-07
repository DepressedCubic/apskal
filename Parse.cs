class Lexer {

    static string special = "+*/[]()";

    string text;
    int position;

    public Lexer(string text) {
        this.text = text;
        this.position = 0;
    }

    public string? Next() {
        
        while ((position < text.Length) && (text[position] == ' ')) {
            position++;
        }

        if (position >= text.Length) {
            return null;
        }

        char c = text[position];

        if (special.Contains(c)) {
            position++;

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

                return token;
            }
            else {
                position++;
                return "-";
            }
        }
        else if (char.IsDigit(c)) {
            string token = "";
            
            while ((position < text.Length) && char.IsDigit(text[position])) {
                token += text[position++];
            }

            return token;
        }
        else if (char.IsLetter(c)) {
            string token = "";
            while ((position < text.Length) && char.IsLetter(text[position])) {
                token += text[position++];
            }
            
            return token;
        }
        else {
            throw new UnknownCharacterException(c);
        }

    }
}

class Parser {
    public abstract class Node {}

    class Literal : Node {
        string name;
        string type;

        public Literal(string name, string type) {
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

    static Node Tree(string type, string expression) {

        Lexer l = new Lexer(expression);

    }
}