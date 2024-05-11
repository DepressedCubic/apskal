using System;

class InvalidDigitException : Exception {
    char symbol;

    public InvalidDigitException(char symbol) {
        this.symbol = symbol;
    }
}

class DivisionByZeroException : Exception {

}