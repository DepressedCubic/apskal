using System;

class Apskal {

    public static bool Exit = false;

    static void Main() {
        Console.WriteLine("APSKAL v1.0");
        Console.WriteLine("Enter EXIT to exit.");
        Console.WriteLine("-------------------");

        while (!Exit) {
            Console.Write("> ");
            if (Console.ReadLine() is string command) {
                try {
                    Runtime.ExecuteCommand(command);
                }
                catch (Exception e) {
                    if (!(e is InvalidSyntaxException i)) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}