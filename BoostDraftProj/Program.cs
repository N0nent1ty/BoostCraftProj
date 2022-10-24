// See https://aka.ms/new-console-template for more information
using System;
using Lexer_and_parser;


class CCheckFileFormat
{



    static bool CheckedTheSymmetry(List<string> pair_string)
    {

        bool isAllEqual = pair_string.Distinct().Count() == 1;
        return isAllEqual;
    }



    static void PrintAndCheckNode(CElementNode node, ref bool bFlag) {


        foreach (string identifier in node.strList_Identifiers)
        {
            //One false in node, return as invalid, ex <test1><test2><test3> </lol></test2></test3>, one error return as invalid
            if (CheckedTheSymmetry(node.strList_Identifiers)==false) { 
                bFlag=false;
            }
            Console.WriteLine(identifier);
        }
        if (node.ChildNodes != null && (node.ChildNodes.Any())) {
            foreach (Lexer_and_parser.CElementNode ChildNode in node.ChildNodes) {
                PrintAndCheckNode(ChildNode, ref bFlag);
            }
        }
        return;
    }



    static bool CheckFileFormat(FileInfo file)
    {
        //If the input not even in XML node format, it simply raise a exeception 

       //If the input format is a valid XML node, check it's Symmetry, ex: <person> </person>
        System.Console.WriteLine("===================================================================");
        System.Console.WriteLine("Execute the validator on " + file.Name);
        System.Console.WriteLine("===================================================================");
        string Input_string1 = System.IO.File.ReadAllText(file.FullName);
        System.Console.WriteLine("Contents of text file = {0}", Input_string1);
            
        Lexer_and_parser.Parser parser = new Lexer_and_parser.Parser(Input_string1);
        bool bFlag = true;
        CElementNode node= parser.Element();
        if (node.strList_Identifiers.Count < 2) {
            bFlag = false;
        }
        PrintAndCheckNode(node, ref bFlag);
        if (bFlag == true) {
            Console.WriteLine("It is a valid XML node input");
        }
        else
        {
            Console.WriteLine("It is not a valid XML node input");
        }


        //print tokens for debugging
        /* 
        List<CBaseTokenNode> tokens = parser.get_tokens();
        foreach (CBaseTokenNode token in tokens) {
            Console.WriteLine(token.getText());
        }
        */
        return false;
    }



    static void Main()
    {


        string Folder_path = Path.Combine(Directory.GetCurrentDirectory(), "test_inputs");

        DirectoryInfo di = new DirectoryInfo(Folder_path);
        //Will test all text file under the "test_inputs" folder
        FileInfo[] files = di.GetFiles("*.txt");
        foreach (FileInfo file in files)
        {
            CheckFileFormat(file);
        }
    }
}
