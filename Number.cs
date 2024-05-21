using System;
using System.Collections.Generic;

interface IField<F> {

    F Add(F x);
    F Subtract(F x);
    F Multiply(F x);
    F Divide(F x);
    F Reciprocal();
    bool IsZero { get; }
    string GetString();
    
}

class Natural {

    uint[] blocks;

    static bool BlockAddition(uint a, uint b, out uint sum) {
        sum = a + b;
        return (sum < a);
    }

    static bool BlockSubtraction(uint a, uint b, out uint sub) {
        sub = a - b;
        return (sub > a);
    }

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

    Natural MultiplyByTwo() {
        return this.Add(this);
    }

    public bool IsZero {
        get {
           return (this.blocks.Length == 1) && (blocks[0] == 0); 
        }
    }

    public Natural() {
        this.blocks = new uint[1];
        this.blocks[0] = 0;
    }

    public Natural(uint n) {
        this.blocks = new uint[1];
        this.blocks[0] = n;
    }

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


    public Natural(string expansion) {
        Natural result = new Natural();
        Natural TEN = new Natural(10);

        foreach (char c in expansion) {
            result = result.Multiply(TEN);
            result = result.Add(new Natural(c));
        }

        this.blocks = result.blocks;
    }

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

        Natural sum = new();

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

    // Computes the monus A.Monus(B) of natural numbers
    // A and B; that is, if A >= B, it returns A - B,
    // otherwise it returns 0.

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

    public bool GreaterThan(Natural N) {
        N.Monus(this, out bool negative);
        return negative;
    }

    public Natural Multiply(Natural N) {
        Natural result = new Natural();

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

    public string GetString() {
        Natural TEN = new Natural(10);
        string s = "";

        Natural n = this;
        while (!n.IsZero) {
            n = n.Divide(TEN, out Natural r);
            char digit = (char)((int)'0' + (int)(r.blocks[0]));
            s = digit + s;
        }

        return s;
    }
}

class Integer {

    Natural magnitude;
    bool is_negative;

    public Integer() {
        this.magnitude = new Natural();
        is_negative = false;
    }

    public Integer(bool negative, Natural N) {
        this.magnitude = N;
        this.is_negative = negative;
    }

    public Integer(string s) {
        if (s.Length == 0 || ((s.Length == 1) && s == "-")) {
            throw new InvalidExpressionException();
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

    public bool IsZero {
        get {
            return (this.magnitude.IsZero);
        }
    }

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

    public Integer Subtract(Integer N) {
        Integer difference = new Integer();
        Integer negation = new Integer();

        negation.magnitude = N.magnitude;
        negation.is_negative = !N.is_negative;

        difference = this.Add(negation);

        return difference;
    }

    public Integer Multiply(Integer N) {
        Integer product = new Integer();

        product.magnitude = this.magnitude.Multiply(N.magnitude);
        product.is_negative = this.is_negative ^ N.is_negative;

        return product;
    }

    public Integer Divide(Natural N) {
        Integer quotient = new Integer();

        quotient.magnitude = this.magnitude.Divide(N, out Natural x);
        quotient.is_negative = this.is_negative;

        return quotient;
    }

    public Natural AbsoluteValue {
        get {
            return this.magnitude;
        }
    }

    public bool IsNegative {
        get {
            return this.is_negative;
        }
    }

    public string GetString() {
        string s = this.magnitude.GetString();

        if (this.is_negative) {
            s = '-' + s;
        }

        return s;
    }
}

class Rational : IField<Rational> {

    Integer numerator;
    Integer denominator;

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

    public bool IsZero {
        get {
            return (numerator.IsZero);
        }
    }

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

    public Rational Reciprocal() {
        Integer p = this.numerator;
        Integer q = this.denominator;

        return new Rational(q, p);
    }

    public string GetString() {
        string num = this.numerator.GetString();
        string denom = this.denominator.GetString();

        return $"{num} / {denom}";
    }
}

class Residue : IField<Residue> {

    uint value;
    uint modulo;

    static bool IsPrime(uint n) {
        for (uint d = 2; d * d <= n; ++d) {
            if (n % d == 0) {
                return false;
            }
        }

        return true;
    }

    public bool IsZero {
        get {
            return this.value == 0;
        }
    }

    public Residue(uint value, uint modulo) {
        if (!IsPrime(modulo)) {
            throw new CompositeModuloException(modulo);
        }
        else {
            this.value = value % modulo;
            this.modulo = modulo;
        }
    }

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

    public Residue Reciprocal() {
        if (this.IsZero) {
            throw new DivisionByZeroException();
        }
        else {
            return this.Power(this.modulo - 2);
        }
    }

    public string GetString() {
        return this.value.ToString();
    }
}