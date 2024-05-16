using System;

class InvalidDigitException : Exception {
    char symbol;

    public InvalidDigitException(char symbol) {
        this.symbol = symbol;
    }
}

class DivisionByZeroException : Exception {

}

class InvalidExpressionException : Exception {

}

class CompositeModuloException : Exception {
    uint modulo;

    public CompositeModuloException(uint modulo) {
        this.modulo = modulo;
    }
}

class IncompatibleModuloException : Exception {
    uint modulo1;
    uint modulo2;

    public IncompatibleModuloException(uint modulo1, uint modulo2) {
        this.modulo1 = modulo1;
        this.modulo2 = modulo2;
    }
}