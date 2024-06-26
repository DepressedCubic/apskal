# APSKAL: Arbitrary-Precision Symbolic KALculator

A rudimentary CLI arbitrary-precision prefix-notation symbolic calculator written in C#, to be submitted as a semester project for the Programming 2 class in Charles University.

## Current Features

- Capable of performing arbitrary precision calculations in $\mathbb{Q}$. Also capable of performing calculations in $\mathbb{Z}_p$ (for prime $p$ below $2^{32}$).
- Capable of performing calculations with matrices using entries from the fields mentioned above: namely, $\mathbb{Q}$ and $\mathbb{Z}_p$ for prime $p$, including computation of the reduced row echelon form (RREF).
- The 'symbolic' capabilities are somewhat limited -- currently, it is only possible to store variables which are literals (either numbers or matrices).
- The calculator's main capability is the evaluation of expressions in prefix notation using any (valid) combination of literals and variables (note, however, that matrices can only be handled in these expressions as variables).

## How to Run

- Clone this repository and, inside it, run the following command in the terminal:
```
dotnet run
```
- By default, you must have .NET 8.0 installed; for other versions you may have to modify ```apskal.csproj``` accordingly (in that case, optimal results are not guaranteed).

## User Instructions

- You'll be greeted by the prompt ```>```. After it, you must write a command. A **command** is any of the following three:
    1. ```EXIT```: As indicated at the start of the program, this lets you exit the program gracefully.
    2. ```DEF <type> <name>```: This lets you define a variable called ```<name>``` of type ```<type>```.
    3. ```EVAL <type>```: This lets you evaluate an expression written in prefix (aka Polish) notation of type ```<type>```.

- **Type Syntax**: There are three valid types:
    1. ```Q```: Rational numbers.
    2. ```Zp```: Here, ```p``` is any prime number (so, for instance, ```Z13``` is a valid type). If this ```p``` isn't prime, the program will complain immediately.
    3. ```Matrix[<type>][<height>][<width>]```: A matrix with entries of type ```<type>``` (where ```<type>``` is one of the two types mentioned above), height ```<height>``` and width ```<width>```.

- **Regarding Names**: Currently, all names must only include lowercase or uppercase letters in the English alphabet (but they can be of any length!).

### Defining Variables

- Recall that the syntax is ```DEF <type> <name>```.
- After entering this command, one of two things can happen:
    1. If the type is either ```Q``` or ```Zp```, in the next line you must write a **literal** of this type (this means a number and only a number -- no expressions or variables). For rationals, this means writing something of the form ```x``` where ```x``` is an integer, or ```p/q``` for integers ```p``` and ```q```. For elements of some $\mathbb{Z}_p$, this means writing unsigned integers.
    2. If the type is a matrix, you'll be prompted to write the matrix's entries over ```<height>``` lines, and for each line, you must write ```<width>``` literals separated by whitespace.
- Nothing will happen afterwards. To view a variable, you must use the ```EVAL``` command.

### Evaluating Expressions

- Recall that the syntax is ```EVAL <type>```.
- After entering this command, in the next line you must write an expression in prefix notation with type ```<type>```.
- An **expression** may contain any combination of (valid) operations, literals and previously defined variables. Having entered the expression, the program will print the result of evaluating this expression (which may be either a number or a matrix).
- Note that currently, only integer literals can appear in expressions; therefore, to include a fraction in an expression you must either define it before the evaluation, or write ```/ p q``` for ```p/q```.
- For numbers (i.e. elements of $\mathbb{Q}$ and $\mathbb{Z}_p$) the currently allowed operations are: ```*``` (multiplication), ```+``` (addition), ```/``` (division) and ```-``` (subtraction). All of these are binary, meaning they take two arguments.
- For matrices, the currently allowed operations are ```*``` (multiplication -- but remember it's not commutative!), ```+``` (addition), ```-``` (subtraction), and ```rref``` (reduced row echelon form). The first three operations are binary and the last one is unary.

- **Note:** In the source code, the ```Matrix``` class (found in ```Matrix.cs```) also implements the computation of the rank and determinant of a matrix. However, currently these have not been included in the parsing since they mess with the types of the expressions.

## Usage Examples

1. We may define rational variables ```pi = 355/113``` and ```e = 193/71``` as follows:
    ```
    > DEF Q pi
    355/113
    > DEF Q e
    193/71
    ```

    Then, if we wanted to evaluate the rational expressions ```e - pi``` or ```e + pi```, we would do
    ```
    > EVAL Q
    - e pi
    RESULT: -3396/8023
    > EVAL Q
    + e pi
    RESULT: 47014/8023
    ```
    and we would indeed get the correct values (try it!)


2. Suppose we have $F \in \mathbb{Q}^{2 \times 2}$ as defined below and we want to compute $F^5$. We would proceed as follows:
    ```
    > DEF Matrix[Q][2][2] F
    1 1
    1 0
    > EVAL Matrix[Q][2][2]
    * * * * F F F F F
    RESULT: 
    ╭     ╮
    │ 8 5 │
    │ 5 3 │
    ╰     ╯
    ```


3. Consider the following row reductions:
    ```
    > DEF Matrix[Q][3][6] A
    0 -3 -6 3 4 -1
    2 1 -4 13 -4 3
    2 3 0 11 -6 5
    > EVAL Matrix[Q][3][6] 
    rref A
    RESULT: 
    ╭                   ╮
    │ 1  0  -3 7  0  4  │
    │ 0  1  2  -1 0  3  │
    │ 0  0  0  0  1  2  │
    ╰                   ╯
    ```
    or
    ```
    > DEF Matrix[Z3][4][5] B
    0 1 1 0 2
    2 1 0 1 1
    2 1 1 1 1
    1 0 2 2 1
    > EVAL Matrix[Z3][4][5]  
    rref B
    RESULT: 
    ╭           ╮
    │ 1 0 0 2 1 │
    │ 0 1 0 0 2 │
    │ 0 0 1 0 0 │
    │ 0 0 0 0 0 │
    ╰           ╯
    ```

    (for context, read Example 1.6.1 from [here](https://iuuk.mff.cuni.cz/~ipenev/LALectureNotes.pdf#lemma.1.6.1)).

4. Consider the following computations with large numbers:

    ```
    > DEF Q large
    48756348587349587349857349857349857394857349857349958324099548293854769
    > DEF Q small
    384753948574938573498573498573498573498
    > EVAL Q
    + large small
    RESULT: 48756348587349587349857349857350242148805924795923456897598121792428267
    > EVAL Q
    * large large
    RESULT: 2377181527571146101951623160053109027446137279970228859363057882268345628379673049246890866974534302989232496845994924780103707439449264043361
    > EVAL Q
    * * * * large large large large large
    RESULT: 275521736548911733113646957340869183956081713832441068909289840635974461842713345150853553629258744444902435462887649249712160367187863353051133823086430839838047837760896498399044818445440397201525898873028260781771941764194831265582323542889603671866655491656957342780465730338731383043369600482721912765572344042687181681182132392223697043754038724849
    ```

5. Or the computation of approximations of $e$:

    ```
    > EVAL Q
    + + + + + + + 1 1 / 1 2 / 1 6 / 1 24 / 1 120 / 1 720 / 1 5040
    RESULT: 685/252
    ```
