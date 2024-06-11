using System;
using System.Collections.Generic;

/* Interface representing any operations you may want to do with
members of a field (in the mathematical sense of the word). */
interface IField<F> {

    F Add(F x);
    F Subtract(F x);
    F Multiply(F x);
    F Divide(F x);
    F Reciprocal();
    F Negation();

    /* Given an element of a field, outputs the additive identity
    of this field. */
    F GetZero();

    /* Given an element of a field, outputs the multiplicative
    identity of this field. */
    F GetOne();

    /* Returns whether this element is either equal to zero or not. */    
    bool IsZero { get; }

    /* Returns the textual representation of this element of the field. */
    string GetString();

}

/* Implementation of natural numbers (starting from 0) with arbitrary
precision. Uses an array of 32-bit uints (blocks) to achieve this. */
class Natural {

    /* To speed things up a bit, given that naturals up to 10 are
    heavily used in most applications, we initialize these from the
    beginning. */
    static Natural[] Initialized = {
        new Natural(),
        new Natural(1),
        new Natural(2),
        new Natural(3),
        new Natural(4),
        new Natural(5),
        new Natural(6),
        new Natural(7),
        new Natural(8),
        new Natural(9),
        new Natural(10)
    };

    uint[] blocks;

    /* Performs the usual addition of two uints (blocks), which
    is stored in the out parameter 'sum', and returns 'true'
    if there was an overflow (i.e. we must carry to the next
    block). */
    static bool BlockAddition(uint a, uint b, out uint sum) {
        sum = a + b;
        return (sum < a);
    }

    /* Performs the usual subtraction of two uints (blocks), which
    is stored in the out parameter 'sub', and returns 'true'
    if there was an underflow (i.e. we must borrow from the next
    block). */
    static bool BlockSubtraction(uint a, uint b, out uint sub) {
        sub = a - b;
        return (sub > a);
    }


    /* Corrects the size of the object's array of uints, so that
    no leading blocks equal to 0 are stored; thus obtaining a
    'canonical' representation of any natural. */
    void CorrectSize() {
        int k = this.blocks.Length - 1;
        while ((k >= 0) && (this.blocks[k] == 0)) {
            k--;
        }
        int size = (k == -1) ? 1 : (k + 1);
        uint[] new_blocks = new uint[size];

        for (int i = 0; i < size; ++i) {
            new_blocks[i] = this.blocks[i];
        }

        this.blocks = new_blocks;
    }

    /* Property that determines whether a given natural is equal to
    zero. */
    public bool IsZero {
        get {
           return (this.blocks.Length == 1) && (blocks[0] == 0); 
        }
    }

    /* Property that determines whether a given natural is equal to
    one. */
    public bool IsOne {
        get {
            return (this.blocks.Length == 1) && (blocks[0] == 1);
        }
    }

    /* Most basic constructor: returns zero. */
    public Natural() {
        this.blocks = new uint[1];
        this.blocks[0] = 0;
    }

    /* Given an unsigned integer 'n' (i.e. up to 2^32 - 1),
    returns a natural exactly equal to 'n'. */
    public Natural(uint n) {
        this.blocks = new uint[1];
        this.blocks[0] = n;
    }

    /* Given a character (which must be a digit), returns the
    natural number corresponding to this digit. */
    public Natural(char digit) {
        this.blocks = new uint[1];
        uint offset = (uint)(digit - '0');

        if (offset <= 9) {
            this.blocks[0] = offset;
        }
        else {
            throw new InvalidDigitException(digit);
        }
    }

    /* For integers 'n' between 0 and 10, returns the
    pre-initialized natural corresponding to them. For
    integers outside this range, just returns zero. */
    public Natural Precomputed(int n) {
        if ((n >= 0) && (n <= 10)) {
            return Natural.Initialized[n];
        }
        else {
            return Natural.Initialized[0];
        }
    }

    /* Performs addition of natural numbers 'this' and 'N'.
    To do so, it performs BlockAddition block-by-block, taking
    care of the carry if necessary (and in some cases, the
    extension of the array). */
    public Natural Add(Natural N) {
        Natural max, min;
        if (this.blocks.Length >= N.blocks.Length) {
            max = this;
            min = N;
        }
        else {
            max = N;
            min = this;
        }
        
        uint[] truncated_sum = new uint[max.blocks.Length];
        bool overflow = false;

        for (int i = 0; i < max.blocks.Length; ++i) {
            bool incoming = overflow;
            overflow = BlockAddition(
                max.blocks[i], 
                (i < min.blocks.Length) ? min.blocks[i] : 0,
                out uint block_sum
            );
            if (incoming) {
                overflow |= BlockAddition(block_sum, 1, out block_sum);
            }
            truncated_sum[i] = block_sum;
        }

        Natural sum = new Natural();

        if (overflow) {
            sum.blocks = new uint[max.blocks.Length + 1];
            for (int i = 0; i < truncated_sum.Length; ++i) {
                sum.blocks[i] = truncated_sum[i];
            }
            sum.blocks[max.blocks.Length] = 1;
        }
        else {
            sum.blocks = truncated_sum;
        }

        return sum;
    }


    /* Returns the result of multiplying 'this' by two.
    Possibly achievable with bitwise operations, but for
    simplicity the already-implemented 'Add' method was used. */
    Natural MultiplyByTwo() {
        return this.Add(this);
    }

    /* Given that subtraction of natural numbers is not always
    defined, returns the monus A.Monus(B) of
    natural numbers A and B; that is, if A >= B, it returns
    A - B, and 0 otherwise. The out parameter 'negative'
    indicates whether A >= B (false) or A < B (true). */
    public Natural Monus(Natural N, out bool negative) {
        Natural max, min;
        if (this.blocks.Length >= N.blocks.Length) {
            max = this;
            min = N;
        }
        else {
            max = N;
            min = this;
        }

        uint[] extended_sub = new uint[max.blocks.Length];
        bool borrow = false;

        for (int i = 0; i < max.blocks.Length; ++i) {
            bool incoming = borrow;
            borrow = BlockSubtraction(
                max.blocks[i],
                (i < min.blocks.Length) ? min.blocks[i] : 0,
                out uint block_sub
            );
            if (incoming) {
                borrow |= BlockSubtraction(block_sub, 1, out block_sub);
            }
            extended_sub[i] = block_sub;
        }

        Natural sub = new Natural();

        if (borrow) {
            negative = true;
        }
        else {
            negative = false;
            sub.blocks = extended_sub;
            sub.CorrectSize();
        }
        
        return sub;
    }

    /* A.GreaterThan(B) returns 'true' iff
    A > B; uses A.Monus(B) to determine this. */
    public bool GreaterThan(Natural N) {
        N.Monus(this, out bool negative);
        return negative;
    }

    /* A.Multiply(B) returns the product of
    naturals A and B. */   
    public Natural Multiply(Natural N) {
        Natural result = Precomputed(0);

        for (int block = N.blocks.Length - 1; block >= 0; --block) {
            for (int offset = 31; offset >= 0; --offset) {

                result = result.MultiplyByTwo();
                if (((N.blocks[block] >> offset) & 1) == 1) {
                    result = result.Add(this);
                }
            }
        }

        return result;
    }

    /* A.Divide(B) returns the integer division of A by B
    (which, for naturals A and B, will also be a natural).
    The out parameter 'remainder' returns the remainder of
    this division (as a natural number as well). */
    public Natural Divide(Natural N, out Natural remainder) {
        if (N.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            Natural quotient = new Natural();
            quotient.blocks = new uint[this.blocks.Length];
            remainder = new Natural();

            for (int block = this.blocks.Length - 1; block >= 0; --block) {
                for (int offset = 31; offset >= 0; --offset) {
                    uint bit = (this.blocks[block] >> offset) & 1;
                    remainder = remainder.MultiplyByTwo();
                    remainder.blocks[0] |= bit;

                    if (!(N.GreaterThan(remainder))) {
                        remainder = remainder.Monus(N, out bool x);
                        quotient.blocks[block] |= ((uint)1 << offset);
                    }
                }
            }

            quotient.CorrectSize();

            return quotient;
        }
    }

    /* The most general constructor: given an arbitrarily long
    string of digits, returns the natural number corresponding
    to this string of digits. */
    public Natural(string expansion) {
        Natural result = Precomputed(0);

        foreach (char c in expansion) {
            result = result.Multiply(Precomputed(10));
            result = result.Add(Precomputed(c - '0'));
        }

        this.blocks = result.blocks;
    }

    /* Natural.GCD(A,B) returns the greatest common divisor of
    naturals A and B, using Euclid's algorithm as usual. */
    public static Natural GCD(Natural N, Natural M) {
        Natural P = N;
        Natural Q = M;

        while (!Q.IsZero) {
            P.Divide(Q, out Natural R);
            P = Q;
            Q = R;
        }

        return P;
    }

    /* A.GetString() returns the textual decimal representation of
    the natural number A (i.e. a string of decimal digits). */
    public string GetString() {
        Natural TEN = Precomputed(10);
        string s = "";

        Natural n = this;
        if (n.IsZero) {
            return "0";
        }

        while (!n.IsZero) {
            n = n.Divide(TEN, out Natural r);
            char digit = (char)((int)'0' + (int)(r.blocks[0]));
            s = digit + s;
        }


        return s;
    }
}

/* Implementation of integers with arbitrary precision;
to do so, stores the magnitude as a Natural and the sign
as a boolean. */
class Integer {

    Natural magnitude;
    bool is_negative;

    /* Most basic constructor: returns the Integer 0. */
    public Integer() {
        this.magnitude = new Natural();
        is_negative = false;
    }

    /* Constructor: Given a natural N, returns +N if
    negative = false, and -N if negative = true. */
    public Integer(bool negative, Natural N) {
        this.magnitude = N;
        this.is_negative = negative;
    }

    /* Constructor: Given a textual decimal representation of an
    integer of arbitrary length, returns the given integer.
    This representation is a string of decimal digits, optionally
    preceded by a minus sign. */
    public Integer(string s) {
        if (s.Length == 0 || ((s.Length == 1) && s == "-")) {
            throw new InvalidIntegerException();
        }
        else {
            if (s[0] == '-') {
                magnitude = new Natural(s[1..]);
                is_negative = true;
            }
            else {
                magnitude = new Natural(s);
                is_negative = false;
            }
        }
    }

    /* Property that determines whether a given integer
    is zero. */
    public bool IsZero {
        get {
            return (this.magnitude.IsZero);
        }
    }

    /* Property that determines whether a given integer
    is one. */
    public bool IsOne {
        get {
            return (
                (!this.is_negative) &&
                (this.magnitude.IsOne)
            );
        }
    }

    /* A.Add(B) returns the integer A + B, using the
    'Add' and 'Monus' methods from the Natural class and
    taking care of the sign accordingly. */
    public Integer Add(Integer N) {
        Integer sum = new Integer();

        if (!this.is_negative && !N.is_negative) {
            sum.magnitude = this.magnitude.Add(N.magnitude);
            sum.is_negative = false;
        }
        else if (!this.is_negative && N.is_negative) {
            sum.magnitude = this.magnitude.Monus(N.magnitude, out bool negative);
            if (negative) {
                sum.magnitude = N.magnitude.Monus(this.magnitude, out bool x);
                sum.is_negative = true;
            }
            else {
                sum.is_negative = false;
            }
        }
        else if (this.is_negative && !N.is_negative) {
            sum.magnitude = this.magnitude.Monus(N.magnitude, out bool negative);
            if (negative) {
                sum.magnitude = N.magnitude.Monus(this.magnitude, out bool x);
                sum.is_negative = false;
            }
            else {
                sum.is_negative = true;
            }
        }
        else {
            sum.magnitude = this.magnitude.Add(N.magnitude);
            sum.is_negative = true;
        }

        return sum;
    }

    /* A.Subtract(B) returns the integer A - B, using the already
    implemented 'Add' method for Integers. */
    public Integer Subtract(Integer N) {
        Integer difference = new Integer();
        Integer negation = new Integer();

        negation.magnitude = N.magnitude;
        negation.is_negative = !N.is_negative;

        difference = this.Add(negation);

        return difference;
    }

    /* A.Product(B) returns the integer A * B, using the already
    implemented 'Multiply' method in the Natural class, taking
    care of the sign accordingly. */
    public Integer Multiply(Integer N) {
        Integer product = new Integer();

        product.magnitude = this.magnitude.Multiply(N.magnitude);
        product.is_negative = this.is_negative ^ N.is_negative;

        return product;
    }

    /* A.Divide(B) returns the integer division of A by B, where
    A is an integer and B a natural (which means that the sign
    will not change). */
    public Integer Divide(Natural N) {
        Integer quotient = new Integer();

        quotient.magnitude = this.magnitude.Divide(N, out Natural x);
        quotient.is_negative = this.is_negative;

        return quotient;
    }

    /* A.Negation() returns the integer -A. */
    public Integer Negation() {
        Integer n = new Integer();

        n.magnitude = this.magnitude;
        n.is_negative = !this.is_negative;

        return n;
    }

    /* The property A.AbsoluteValue returns |A|, as a natural. */
    public Natural AbsoluteValue {
        get {
            return this.magnitude;
        }
    }

    /* The property A.IsNegative returns 'true' iff A is negative. */
    public bool IsNegative {
        get {
            return this.is_negative;
        }
    }

    /* A.GetString() returns the textual decimal representation of
    the integer A (that is, a string of decimal digits, optionally
    preceded by a minus sign). */
    public string GetString() {
        string s = this.magnitude.GetString();

        if (this.IsZero) {
            return "0";
        }

        if (this.is_negative) {
            s = '-' + s;
        }

        return s;
    }
}

/* Implementation of rational numbers with arbitrary precision;
to do so, stores the numerator and denominator as Integers. */
class Rational : IField<Rational> {

    /* Some commonly used rational numbers are initialized
    from the very beginning to save time. */
    static Integer IntZERO = new Integer();
    static Integer IntONE = new Integer("1");

    static Rational ZERO = new Rational(IntZERO, IntONE);
    static Rational ONE = new Rational(IntONE, IntONE);


    Integer numerator;
    Integer denominator;

    /* A.Simplify() simplifies Rational A so that the
    numerator and denominator are coprime, and the denominator
    is strictly positive; thus obtaining a 'canonical'
    representation of any rational number. */
    void Simplify() {
        Natural gcd = Natural.GCD(
            numerator.AbsoluteValue,
            denominator.AbsoluteValue);

        bool negative = numerator.IsNegative ^ denominator.IsNegative;

        numerator = new Integer(
            negative, 
            numerator.AbsoluteValue.Divide(gcd, out Natural x)
        );
        denominator = new Integer(
            false,
            denominator.AbsoluteValue.Divide(gcd, out x)
        );
    }

    /* The property A.IsZero returns 'true' iff A is zero. */
    public bool IsZero {
        get {
            return (numerator.IsZero);
        }
    }

    /* Constructor: Rational(A,B) returns a rational number
    of numerator A and nonzero denominator B. Also takes
    care of simplifying. */
    public Rational(Integer numerator, Integer denominator) {
        if (denominator.AbsoluteValue.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            this.numerator = numerator;
            this.denominator = denominator;

            this.Simplify();
        }
    }

    /* Constructor: Given a textual representation of a
    rational number of either the form p or p/q, for integers
    p and q, returns the Rational corresponding to this
    representation. */
    public Rational(string s) {
        string[] q = s.Split('/');
        if (q.Length == 1) {
            this.numerator = new Integer(s);
            this.denominator = IntONE;
        }
        else {
            this.numerator = new Integer(q[0]);
            this.denominator = new Integer(q[1]);
        }
    }

    /* P.Add(Q) returns the rational P + Q. */
    public Rational Add(Rational Q) {
        Integer p = this.numerator;
        Integer q = this.denominator;
        Integer r = Q.numerator;
        Integer s = Q.denominator;
        
        return new Rational(
            (p.Multiply(s)).Add(q.Multiply(r)),
            q.Multiply(s)
        );
    }

    /* P.Subtract(Q) returns the rational P - Q. */
    public Rational Subtract(Rational Q) {
        Integer p = this.numerator;
        Integer q = this.denominator;
        Integer r = Q.numerator;
        Integer s = Q.denominator;

        return new Rational(
            (p.Multiply(s)).Subtract(q.Multiply(r)),
            q.Multiply(s)
        );
    }

    /* P.Multiply(Q) returns the rational P * Q. */
    public Rational Multiply(Rational Q) {
        Integer p = this.numerator;
        Integer q = this.denominator;
        Integer r = Q.numerator;
        Integer s = Q.denominator;

        return new Rational(
            p.Multiply(r),
            q.Multiply(s)
        );
    }

    /* For nonzero Q, P.Divide(Q) returns the
    rational P / Q. */
    public Rational Divide(Rational Q) {
        Integer p = this.numerator;
        Integer q = this.denominator;
        Integer r = Q.numerator;
        Integer s = Q.denominator;

        if (Q.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            return new Rational(
                p.Multiply(s),
                q.Multiply(r)
            );
        }
    }

    /* P.Reciprocal() returns the multiplicative
    inverse of P; i.e. P^-1. */
    public Rational Reciprocal() {
        Integer p = this.numerator;
        Integer q = this.denominator;

        return new Rational(q, p);
    }

    /* P.Negation() returns the additive
    inverse of P; i.e. -P. */
    public Rational Negation() {
        Integer p = this.numerator.Negation();
        Integer q = this.denominator;

        return new Rational(p, q);
    }

    /* P.GetZero() returns the rational
    zero, for any P. */
    public Rational GetZero() {
        return ZERO;
    }

    /* P.GetOne() returns the rational
    one, for any P. */
    public Rational GetOne() {
        return ONE;
    }

    /* P.GetOne() returns a textual
    representation of P of the form
    p if P has denominator 1, or p/q
    otherwise. */
    public string GetString() {
        string num = this.numerator.GetString();
        string denom = this.denominator.GetString();

        if (this.denominator.IsOne) {
            return $"{num}";
        }
        else {
            return $"{num}/{denom}";
        }
    }
}

/* Implementation of the members of fields of the
form Z_p, where p is a prime number at most 2^32 - 1. */
class Residue : IField<Residue> {

    /* WARNING: Primality testing of the modulo has been offloaded
    to the parser (to save time). In case of independent usage of
    this class, make sure to ensure that 'modulo' is prime; otherwise,
    the methods 'Divide' and 'Reciprocal' will misbehave. */

    uint value;
    uint modulo;

    /* The property x.IsZero returns 'true' iff x is zero (in whichever
    field it resides in). */
    public bool IsZero {
        get {
            return this.value == 0;
        }
    }

    /* Constructor: Residue(x, p) returns x in the field Z_p. */
    public Residue(uint value, uint modulo) {
            this.value = value % modulo;
            this.modulo = modulo;
    }

    /* x.Add(y) returns x + y, but only if x and y are members
    of the same field Z_p. */
    public Residue Add(Residue R) {
        if (this.modulo != R.modulo) {
            throw new IncompatibleModuloException(this.modulo, R.modulo);
        }
        else {
            return this.Subtract(
                new Residue(this.modulo - R.value, this.modulo)
            );
        }
    }

    /* x.Subtract(y) returns x - y, but only if x and y are members
    of the same field Z_p. */
    public Residue Subtract(Residue R) {
        if (this.modulo != R.modulo) {
            throw new IncompatibleModuloException(this.modulo, R.modulo);
        }
        else if (this.value >= R.value) {
            return new Residue(
                this.value - R.value, 
                this.modulo
                );
        }
        else {
            return new Residue(
                this.modulo - (R.value - this.value), 
                this.modulo
                );
        }
    }

    /* x.Multiply(y) returns x * y, but only if x and y are members of
    of the same field Z_p. */
    public Residue Multiply(Residue R) {
        if (this.modulo != R.modulo) {
            throw new IncompatibleModuloException(this.modulo, R.modulo);
        }
        else {
            Residue p = this;
            Residue sum = new Residue(0, this.modulo);
            uint q = R.value;
            while (q >= 1) {
                if ((q & 1) == 1) {
                    sum = sum.Add(p);
                }
                p = p.Add(p);
                q >>= 1;
            }

            return sum;
        }
    }

    /* x.Power(n) returns x^n, also in the same field as x. */
    public Residue Power(uint N) {
        Residue exp = this;
        Residue prod = new Residue(1, this.modulo);
        uint q = N;
        while (q >= 1) {
            if ((q & 1) == 1) {
                prod = prod.Multiply(exp);
            }
            exp = exp.Multiply(exp);
            q >>= 1;
        }

        return prod;
    }

    /* x.Divide(y) returns x / y if y is non-zero. For this,
    it uses the previously implemented 'Power' method and
    Fermat's Little Theorem. Here, the fact that 'modulo' is
    prime becomes crucial. */
    public Residue Divide(Residue R) {
        if (this.modulo != R.modulo) {
            throw new IncompatibleModuloException(this.modulo, R.modulo);
        }
        else if (R.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            return this.Multiply(R.Power(this.modulo - 2));
        }
    }

    /* x.Reciprocal() returns the multiplicative inverse of x;
    i.e. x^-1, in the particular field. */
    public Residue Reciprocal() {
        if (this.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            return this.Power(this.modulo - 2);
        }
    }

    /* x.Negation() returns the additive inverse of x; i.e. -x,
    in the particular field. */
    public Residue Negation() {
        if (this.IsZero) {
            return new Residue(this.value, this.modulo);
        }
        else {
            return new Residue(
                this.modulo - this.value,
                this.modulo
            );
        }
    }

    /* x.GetZero() returns zero (the additive identity) in the
    same field as x. */    
    public Residue GetZero() {
        return new Residue(0, this.modulo);
    }

    /* x.GetOne() returns one (the multiplicative identity) in the
    same field as x. */
    public Residue GetOne() {
        return new Residue(1, this.modulo);
    }

    /* x.GetString() returns the textual representation of x. Given
    that the user is supposed to know in what field they're working on,
    the modulo is not included in this representation. */
    public string GetString() {
        return this.value.ToString();
    }
}