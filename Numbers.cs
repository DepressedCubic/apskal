using System;
using System.Collections.Generic;

interface INumber {
    INumber Add(INumber N);
    INumber Subtract(INumber N);
    INumber Multiply(INumber N);
    INumber Divide(INumber N);
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

    public Natural() {
        this.blocks = new uint[1];
        this.blocks[0] = 0;
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

        Natural sub = new();

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

    public Natural Multiply(Natural N) {
        
    }

    public string GetString() {

    }
}

class Integer : INumber {

}

class Residue : INumber {

}

class Rational : INumber {

}