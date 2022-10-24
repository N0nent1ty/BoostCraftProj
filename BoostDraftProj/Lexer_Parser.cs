namespace Lexer_and_parser
{

    class TryParseFailException : Exception
    {
        public TryParseFailException() { }
    }


    enum TOKEN_TYPES
    {
        OPEN,
        CLOSE,
        CHAR_DATA,
        SLASH,
        IDENTIFIER,
        ELEMENT
    }



    class CBaseTokenNode
    {
        private TOKEN_TYPES Token_type;
        private string text = "";
        public CBaseTokenNode(TOKEN_TYPES n)
        {
            this.Token_type = n;
        }


        public CBaseTokenNode(TOKEN_TYPES n, string s)
        {
            this.Token_type = n;
            this.text = s;
        }
        public TOKEN_TYPES get_type() { return this.Token_type; }
        public string getText() { return this.text; }
    }



    class CElementNode : CBaseTokenNode
    {

        public List<string> strList_Identifiers = new List<string>();
        public List<string> strList_Properties = new List<string>();
        public List<CElementNode> ChildNodes;

        public CElementNode(TOKEN_TYPES n) : base(n)
        {
            this.ChildNodes = new List<CElementNode>();
        }





    }


    //================================================================
    //Class Lexer
    //================================================================

    class Lexer
    {
        public string strInputString;
        private int nCurrentCharPosition;
        private int nInputLength;
        private List<CBaseTokenNode> tokens;
        private bool bHasIdentifier = false;
        //Constructor of Lexer
        public Lexer(string strInputString)
        {
            this.strInputString = strInputString;
            //error handling with input length 0 problem.
            if (string.IsNullOrEmpty(this.strInputString))
                throw new ArgumentException("Lexer constructor error, invalid input, string can not be empty or null.", nameof(strInputString));

            this.nCurrentCharPosition = 0;
            this.nInputLength = strInputString.Length;
            this.tokens = new List<CBaseTokenNode>();
        }

        public int GetCurrentCharPosition() { return nCurrentCharPosition; }
        public void SetCurrentCharPosition(int n) { nCurrentCharPosition = n; }

        public List<CBaseTokenNode> get_tokens() { return this.tokens; }



        //Return the character on the Index
        public char PeekOneChar()
        {
            return this.strInputString[nCurrentCharPosition];
        }

        //Specify the index
        public char PeekOneCharAt(int nIndex)
        {
            return this.strInputString[nIndex];
        }



        public bool Match_OPEN_TOKEN()
        {
            IgnoreSpace();
            char ch = PeekOneChar();
            int nRollbackpoint = nCurrentCharPosition;
            char chOpen = '<';
            //Console.WriteLine("ch is " + ch);
            //Console.WriteLine(chOpen.Equals(ch));
            if (chOpen.Equals(ch))
            {

                nCurrentCharPosition += 1;
                CBaseTokenNode node = new CBaseTokenNode(TOKEN_TYPES.OPEN, "<");
                this.tokens.Add(node);
                return true;
            }
            else
            {
                //handle not match error
                //Becase it will be use in try parsing, if turn on the error message, it can be a false positive.
                //it is a trade off between more message or make the message tidy.

                //string strErrorMessage = String.Format("Expect the  OPEN token but a {0}, not a valid XML node", ch);
                //throw new ApplicationException(strErrorMessage);
                this.nCurrentCharPosition = nRollbackpoint;
                //Console.WriteLine(strErrorMessage);
                return false;
            }
        }




        /*
         * Igonore the space while parsing the token.
         */
        public void IgnoreSpace()
        {
            char ch;
            // space or alphabet or digits
            while (true)
            {
                if (this.nCurrentCharPosition == this.nInputLength)
                {
                    throw new ApplicationException("Expect a token but reach EOF, not a valid XML node");
                }


                ch = PeekOneChar();
                if (Char.IsWhiteSpace(ch))
                {
                    nCurrentCharPosition++;
                }
                else
                {
                    break;
                }

            };

        }


        public CBaseTokenNode Match_IDENTIFY_TOKEN()
        {
            IgnoreSpace();
            char ch;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier
            while (true)
            {
                ch = PeekOneChar();
                if (Char.IsLetterOrDigit(ch))
                {
                    bHasIdentifier = true;
                    nTokenLengthCount++;
                    nCurrentCharPosition++;
                    if (nCurrentCharPosition == this.nInputLength)
                    {
                        string strErrorMessage = String.Format("Expect the  IDENTIFIER token but reach to EOF token \'>\' , not a valid XML node");
                        throw new ApplicationException(strErrorMessage);
                    }
                }
                else if (ch.Equals('>'))
                {
                    //Console.WriteLine(bHasIdentifier);
                    if (bHasIdentifier == false)
                    {
                        string strErrorMessage = String.Format("Expect the  IDENTIFIER token but a CLOSE token \'>\' , not a valid XML node");
                        throw new ApplicationException(strErrorMessage);
                    }

                    string s = strInputString.Substring(nTokenBeginPosition, nTokenLengthCount);
                    CBaseTokenNode Identifier_node = new CBaseTokenNode(TOKEN_TYPES.IDENTIFIER, s);
                    this.tokens.Add(Identifier_node);
                    return Identifier_node;

                }
                else if (Char.IsWhiteSpace(ch))
                {
                    //try_parse_optional_attr();
                    IgnoreSpace();
                    IgnoreAttribute();
                    //Console.WriteLine("Try parse attr");

                }
                else
                {
                    //handle invalid symbol or alpahbet in Identifier.
                    string strErrorMessage = String.Format("Expect the  IDENTIFIER token but a invalid character {0}, not a valid XML node", ch);
                    throw new ApplicationException(strErrorMessage);
                }

            }
        }




        public CBaseTokenNode TryMatch_IDENTIFY_TOKEN()
        {
            IgnoreSpace();
            // remember the current position, inoder to rollback
            int nRollBackPosition = this.nCurrentCharPosition;



            char ch;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier

            while (true)
            {
                ch = PeekOneChar();
                if (Char.IsLetterOrDigit(ch))
                {
                    bHasIdentifier = true;
                    nTokenLengthCount++;
                    nCurrentCharPosition++;
                }
                else if (ch.Equals('>'))
                {
                    //Console.WriteLine(bHasIdentifier);
                    if (bHasIdentifier == false)
                    {
                        this.nCurrentCharPosition = nRollBackPosition;
                        throw new TryParseFailException();
                    }

                    string s = strInputString.Substring(nTokenBeginPosition, nTokenLengthCount);
                    CBaseTokenNode Identifier_node = new CBaseTokenNode(TOKEN_TYPES.IDENTIFIER, s);
                    this.tokens.Add(Identifier_node);
                    return Identifier_node;

                }
                else if (Char.IsWhiteSpace(ch))
                {
                    //try_parse_optional_attr();
                    IgnoreSpace();
                    Console.WriteLine("Try parse attr");

                }
                else
                {
                    //handle invalid symbol or alpahbet in Identifier.
                    this.nCurrentCharPosition = nRollBackPosition;
                    throw new TryParseFailException();
                }

            }
        }



        public CBaseTokenNode TryMatch_CHARDATA_TOKEN()
        {
            IgnoreSpace();
            int nRollbackPosition = this.nCurrentCharPosition;
            int nCharDataLengthCount = 0;
            int nCharDataBeginPosition = this.nCurrentCharPosition;
            while (true)
            {
                char ch = PeekOneChar();

                //befre '<'
                if (ch.Equals('<'))
                {

                    if (nCharDataLengthCount == 0)
                    {
                        //No chardata data length==0
                        this.nCurrentCharPosition = nRollbackPosition;
                        throw new TryParseFailException();

                    }
                    else
                    {
                        string text = this.strInputString.Substring(nCharDataBeginPosition, nCharDataLengthCount);
                        CBaseTokenNode charData_node = new CBaseTokenNode(TOKEN_TYPES.CHAR_DATA, text);
                        this.tokens.Add(charData_node);
                        return charData_node;
                    }

                }
                else
                {
                    this.nCurrentCharPosition++;
                    nCharDataLengthCount++;
                    if (this.nCurrentCharPosition == this.nInputLength)
                    {
                        //reach EOF
                        this.nCurrentCharPosition = nRollbackPosition;
                        throw new TryParseFailException();
                    }
                }
            }//end while
        }





        public void Match_CLOSE_TOKEN()
        {
            IgnoreSpace();
            char ch = PeekOneChar();
            if (ch.Equals('>'))
            {
                CBaseTokenNode close_node = new CBaseTokenNode(TOKEN_TYPES.CLOSE, ">");
                this.tokens.Add(close_node);
            }
            else
            {
                string strErrorMessage = String.Format("Expect the  CLOSE token but a {0}, not a valid XML node", ch);
                throw new ApplicationException(strErrorMessage);
            }
        }




        public bool Match_SLASH_TOKEN()
        {
            IgnoreSpace();
            int nRollbackPoint = this.nCurrentCharPosition;
            char ch = PeekOneChar();
            char chOpen = '/';
            // Console.WriteLine("ch is " + ch);
            //Console.WriteLine(chOpen.Equals(ch));
            if (chOpen.Equals(ch))
            {

                nCurrentCharPosition += 1;
                CBaseTokenNode node = new CBaseTokenNode(TOKEN_TYPES.SLASH, "/");
                this.tokens.Add(node);
                return true;
            }
            else
            {
                //handle not match error
                //string strErrorMessage = String.Format("Expect the  SLASH token but a {0}, not a valid XML node", ch);
                //throw new ApplicationException(strErrorMessage);
                this.nCurrentCharPosition = nRollbackPoint;
                //Console.WriteLine(strErrorMessage);
                return false;
            }
        }


        public void IgnoreAttribute()
        {
            IgnoreSpace();
            while (true)
            {
                char ch = PeekOneChar();
                if (ch.Equals('>')) { return; }

                // legitimate input =, ", alphabet, numberm and white space
                else if (ch.Equals('\"') || ch.Equals('=') || Char.IsLetterOrDigit(ch) || Char.IsWhiteSpace(ch))
                {
                    this.nCurrentCharPosition++;
                }
                else
                {
                    string strErrorMessage = String.Format("Invalid charater detected while parsing attribute, invalid symbol {0}, not a valid XML node", ch);
                    throw new ApplicationException(strErrorMessage);
                }


                if (nCurrentCharPosition == this.nInputLength)
                {
                    string strErrorMessage = String.Format("Reach EOF while parsing attribute, not a valid XML node");
                    throw new ApplicationException(strErrorMessage);

                }
            }
        }

    }//end class lexer







    //================================================================
    //Class Parser
    //
    //================================================================
    class Parser
    {
        
        private Lexer lexer;


        public Parser(string strInput)
        {
            this.lexer = new Lexer(strInput);
        }





        /*
         * Definition of the element in EBNF form
         *   element : '<' Identifier attribute* '>' chardata '<' '/' Identifier '>'
         *   ;
         * 
         * 
         * To make it easier to descibe the components of element, we call <brabrabra> as BEGIN_ELEMENT, </brabrabra> as TERMINATE_ELEMENT
         * 
         * And each element here, we call it element node
         * Each element node has mutiple child node
         * ex: <root_node><child_node1></child_node2><child_node3></child_nod3></root_node>
         */

        public CElementNode Element()
        {

            //Create root node
            CElementNode element_node = new CElementNode(TOKEN_TYPES.ELEMENT);
            //Because we can not predict whether this file is in XML format, so we try in parsing.
            try
            {
                //Ignore the space before parsing each token.
                this.lexer.IgnoreSpace();


                //Try Match "<"
                this.lexer.Match_OPEN_TOKEN();
                //Parsing the Identifier
                CBaseTokenNode firstIdentifier = this.lexer.Match_IDENTIFY_TOKEN();
                //add the Identifier string to the root node
                string strFirstIdentifier = firstIdentifier.getText();
                element_node.strList_Identifiers.Add(strFirstIdentifier);
                
                

                this.lexer.Match_CLOSE_TOKEN();
                this.lexer.IgnoreSpace();
                this.lexer.SetCurrentCharPosition(this.lexer.GetCurrentCharPosition() + 1);
                int nRecord = 0;

                //peek on char after "greater than sign" ex: <person attr="123"> `here`
                this.lexer.IgnoreSpace();
                char ch = this.lexer.PeekOneChar();


                //If the next char after the  "greater than sign" is not the "less than sign", it must be the chardata
                if (!ch.Equals('<'))
                {
                    CBaseTokenNode CharDataNode = lexer.TryMatch_CHARDATA_TOKEN();
                    Console.WriteLine(CharDataNode.getText());

                }
                //It must be the BEGIN_ELEMENT the next two char match the constrain below, and a BEGIN_ELEMENT means there will be a child node
                else if (ch.Equals('<') && (this.lexer.PeekOneCharAt(this.lexer.GetCurrentCharPosition() + 1) != '/'))
                {
                    //<without slash >
                    nRecord = this.lexer.GetCurrentCharPosition();

                    element_node.ChildNodes.Add(Element());
                    this.lexer.SetCurrentCharPosition(this.lexer.GetCurrentCharPosition() + 1);
                }
                //Read an onexpect the charater
                else
                {
                    this.lexer.IgnoreSpace();
                    string strErrorMessage = String.Format(" Unexpected character detected {0}, not a valid XML node", ch);
                    Console.WriteLine(strErrorMessage);
                    throw new TryParseFailException();

                }

                
                //after parsing a child node or char data, there must be another child node(BEGIN_ELEMENT) or TERMINATE_ELEMENT
                //the difference between the BEGIN_ELEMENT and TERMIATE_ELEMENT is that string begine with '<' or '</'
                //Try match first two char and we can tell what type of token next
                this.lexer.Match_OPEN_TOKEN();
                if (!this.lexer.Match_SLASH_TOKEN())
                {

                THERE_IS_ANOTHER_NODE:
                    element_node.ChildNodes.Add(Element());
                    this.lexer.SetCurrentCharPosition(this.lexer.GetCurrentCharPosition() + 1);
                    this.lexer.Match_OPEN_TOKEN();
                    //Detect there is another sibling node
                    if (!this.lexer.Match_SLASH_TOKEN())
                    {
                        goto THERE_IS_ANOTHER_NODE;
                    }
                }



                //According to the XML grammar, If not the child node, it must be the TERMINATE_ELEMENT
                CBaseTokenNode second_Identifier = this.lexer.Match_IDENTIFY_TOKEN();
                string strSecondIdentifier = second_Identifier.getText();
                element_node.strList_Identifiers.Add(strSecondIdentifier);
                this.lexer.Match_CLOSE_TOKEN();
            }
            catch (TryParseFailException)
            {
                //Try parse fail, do nothing, continue
                //If user want to  get more parsing fail message, this exception will become usefull
                Console.WriteLine("TryParseFailException");
            }
            catch (Exception ex)
            {
                //Console print out the exception
                Console.WriteLine(ex);
            }
            return element_node;
        }
        public List<CBaseTokenNode> get_tokens() { return this.lexer.get_tokens(); }
    }
}