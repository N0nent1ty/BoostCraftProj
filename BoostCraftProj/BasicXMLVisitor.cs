using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using AntlrCSharp;
using AntrlXMLParser;

namespace AntlrCSharp
{
    public class BasicXMLVisitor : AntrlXMLParser.XMLParserBaseVisitor<object>  
    {

        public  List<string> strList_names = new List<string>();
        private bool bHasElementContent=false;
        public bool bIsValidXMLNode=false;

        private List<string> GetTheNameOfSubElement(XMLParser.ElementContext elemetcontext) { 
            List<string> strSubElementName = new List<string>();
            if (elemetcontext.content().element().Length != 0)
            {
                this.bHasElementContent = true;
            }
            else { 
                this.bHasElementContent= false;
            }
            foreach (ITerminalNode terminalNode in elemetcontext.Name()) {
                Console.WriteLine(terminalNode.GetText());
                strSubElementName.Add(terminalNode.GetText());
            }
            return strSubElementName;   
        }

        private bool checkedTheSymmetry(List<string> pair_string) {

            bool isAllEqual = pair_string.Distinct().Count() == 1;

            return isAllEqual;
        }


        public override object VisitElement(XMLParser.ElementContext context)
        {
            List<string> strList_NamePair= new List<string>();
            XMLParser.ElementContext tempContext = context;
            do
            {
                strList_NamePair=this.GetTheNameOfSubElement(tempContext);
                if (checkedTheSymmetry(strList_NamePair) == false) {
                    Console.WriteLine("Symmetry error detected: " + strList_NamePair[1] + " should be " + strList_NamePair[0]);
                    this.bIsValidXMLNode = false;
                    return 0;            
                }

                if (this.bHasElementContent) {
                    //Console.WriteLine(this.bHasElementContent);
                    tempContext = tempContext.content().element()[0];                
                }

            } while (this.bHasElementContent);
            this.bIsValidXMLNode = true;
            return 0;

        }

    }
}