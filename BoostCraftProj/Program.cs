// See https://aka.ms/new-console-template for more information
using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using AntrlXMLParser;
using AntlrCSharp;



class CheckFileFormat
{
    static bool checkFileFormat(FileInfo file)
    {
        //If the input not even in XML node format, it simply raise a exeception 
        try
        {

            //If the input format is a valid XML node, check it's Symmetry, ex: <person> </person>
            System.Console.WriteLine("===================================================================");
            System.Console.WriteLine("Execute the validator on " + file.Name);
            System.Console.WriteLine("===================================================================");
            string Input_string1 = System.IO.File.ReadAllText(file.FullName);
            System.Console.WriteLine("Contents of text file = {0}", Input_string1);

            AntlrInputStream inputStream = new AntlrInputStream(Input_string1);
            XMLLexer inputLexer = new XMLLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(inputLexer);
            XMLParser inputParser = new XMLParser(commonTokenStream);

            //Parse the file with XML node parser
            XMLParser.ElementContext elementContext = inputParser.element();

            //Create visitor to parse the node on the abstract syntax tree.
            BasicXMLVisitor visitor = new BasicXMLVisitor();
            visitor.Visit(elementContext);
            if (visitor.bIsValidXMLNode)
            {
                Console.WriteLine(file.Name +" is a valid XML node\n");
                return true;
            }
            else
            {
                Console.WriteLine(file.Name + " is not a valid XML node\n");
                return false;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("This is not even a valid xml node, fail in parsing" + ex);
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
