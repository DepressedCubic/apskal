using System;

class Apskal {

    /* Stores 'true' iff the program is ready
    to exit. */
    public static bool Exit = false;

    static void Main() {

        Console.WriteLine("APSKAL v1.0");
        Console.WriteLine("Enter EXIT to exit.");
        Console.WriteLine("-------------------");

        /* Event loop */
        while (!Exit) {
            Console.Write("> ");
            if (Console.ReadLine() is string command) {
                try {
                    Runtime.ExecuteCommand(command);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}