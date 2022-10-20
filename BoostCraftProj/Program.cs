// See https://aka.ms/new-console-template for more information
using System;
using Lexer_and_parser;


class CheckFileFormat
{

    static bool checkFileFormat(FileInfo file)
    {
        //If the input not even in XML node format, it simply raise a exeception 

       //If the input format is a valid XML node, check it's Symmetry, ex: <person> </person>
        System.Console.WriteLine("===================================================================");
        System.Console.WriteLine("Execute the validator on " + file.Name);
        System.Console.WriteLine("===================================================================");
        string Input_string1 = System.IO.File.ReadAllText(file.FullName);
        System.Console.WriteLine("Contents of text file = {0}", Input_string1);
            
        Lexer_and_parser.Parser parser = new Lexer_and_parser.Parser(Input_string1);
        parser.Element();

        List<CBaseTokenNode> tokens = parser.get_tokens();
        foreach (CBaseTokenNode token in tokens) {
            Console.WriteLine(token.getText());
        }        
        return false;
    }



    static void Main()
    {


        string Folder_path = Path.Combine(Directory.GetCurrentDirectory(), "test_inputs");

        DirectoryInfo di = new DirectoryInfo(Folder_path);
        //Will execute all text file under the "test_inputs" folder
        FileInfo[] files = di.GetFiles("*.txt");
        foreach (FileInfo file in files)
        {
            checkFileFormat(file);
        }
    }
}
