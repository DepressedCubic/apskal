using System;

/* Exception thrown any time the construction
of a Natural has been attempted with an unknown
digit character. */
class InvalidDigitException : Exception {
    char symbol;

    public InvalidDigitException(char symbol) {
        this.symbol = symbol;
    }
}

/* Exception thrown any time division by zero
has been attempted. */
class DivisionByZeroException : Exception {

}

/* Exception thrown any time it has been attempted
to construct an Integer using an invalid textual
representation. */
class InvalidIntegerException : Exception {
}

/* Exception thrown during parsing any time it has
been attempted to create an element of Z_p where
p is not prime. */ 
class CompositeModuloException : Exception {
    uint modulo;

    public CompositeModuloException(uint modulo) {
        this.modulo = modulo;
    }
}

/* Exception thrown any time it has been attempted
to perform arithmetic with members of two distinct fields
Z_p and Z_q. */
class IncompatibleModuloException : Exception {
    uint modulo1;
    uint modulo2;

    public IncompatibleModuloException(uint modulo1, uint modulo2) {
        this.modulo1 = modulo1;
        this.modulo2 = modulo2;
    }
}

/* Exception thrown any time it has been attempted
to perform arithmetic with matrices of incompatible dimensions.
For addition, this means not having the exact same width and height;
For multiplication, this means the width of the first matrix is not
the same as the height of the second matrix. */
class IncompatibleDimensionsException : Exception {
}

/* Exception thrown any time an unknown character is found
during the parsing. */
class UnknownCharacterException : Exception {
    char c;

    public UnknownCharacterException(char c) {
        this.c = c;
    }
}

/* Exception thrown any time invalid syntax is encountered. */
class InvalidSyntaxException : Exception {
}

/* Exception thrown any time it has been attempted to store
a number as a matrix or a matrix as a number. */
class IncompatibleTypeException : Exception {
}
