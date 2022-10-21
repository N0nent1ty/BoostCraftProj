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



    class CBaseTokenNode{
        private TOKEN_TYPES Token_type;
        private string text="";
        public CBaseTokenNode(TOKEN_TYPES n) {
            this.Token_type = n;
        }


        public CBaseTokenNode( TOKEN_TYPES n, string s) { 
            this.Token_type = n;
            this.text = s;
        }
        public TOKEN_TYPES get_type() { return this.Token_type; }
        public string getText() { return this.text; }
    }



    class CElementNode : CBaseTokenNode  {

        public List<string> strList_Identifiers = new List<string>();
        public List<string> strList_Properties =new List<string>();
        public CElementNode childNode;

        public CElementNode(TOKEN_TYPES n) : base(n) { 
            
        }





    }


    //================================================================
    //Class Lexer
    //
    //================================================================

    class Lexer {
        public string strInputString;
        private int nCurrentCharPosition;
        private int nInputLength;
        private List<CBaseTokenNode> tokens;
        private bool bHasIdentifier = false;
        //Constructor of Lexer
        public Lexer(string strInputString) {
            this.strInputString = strInputString;
            //error handling with input length 0 problem.
            if (string.IsNullOrEmpty(this.strInputString))
                throw new ArgumentException("Lexer constructor error, invalid input, string can not be empty or null.", nameof(strInputString));

            this.nCurrentCharPosition = 0;
            this.nInputLength = strInputString.Length;
            this.tokens = new List<CBaseTokenNode>();
        }

        public int Get_Current_Char_Position() { return nCurrentCharPosition; }
        public void Set_Current_Char_Position(int n) { nCurrentCharPosition = n; }

        public List<CBaseTokenNode> get_tokens() { return this.tokens; }



        //Return the character on the Index
        public char Peek_One_Char() {
            return this.strInputString[nCurrentCharPosition];
        }

        //Specify the index
        public char Peek_One_Char_at(int nIndex)
        {
            return this.strInputString[nIndex];
        }



        public bool Match_OPEN_TOKEN() {
            Ignore_space();
            char ch = Peek_One_Char();
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
            else {
                //handle not match error
                string strErrorMessage = String.Format("Expect the  OPEN token but a {0}, not a valid XML node", ch);
                //throw new ApplicationException(strErrorMessage);
                Console.WriteLine(strErrorMessage);
                return false;
            }
        }

        public void Ignore_space() {
            char ch;
            // space or alphabet or digits
            while(true)
            {
                if (this.nCurrentCharPosition == this.nInputLength)
                {
                    throw new ApplicationException("Expect a token but reach EOF, not a valid XML node");
                }


                ch = Peek_One_Char();
                if (Char.IsWhiteSpace(ch))
                {
                    nCurrentCharPosition++;
                }
                else {
                    break;
                }

            };

        }


        public CBaseTokenNode Match_IDENTIFY_TOKEN() {
            Ignore_space();
            char ch ;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier
            while (true) {
                ch = Peek_One_Char();
                if (Char.IsLetterOrDigit(ch))
                {
                    bHasIdentifier = true;
                    nTokenLengthCount++;
                    nCurrentCharPosition++;
                    if (nCurrentCharPosition == this.nInputLength) {
                        string strErrorMessage = String.Format("Expect the  IDENTIFIER token but reach to EOF token \'>\' , not a valid XML node");
                        throw new ApplicationException(strErrorMessage);
                    }
                }
                else if (ch.Equals('>'))
                {
                    //Console.WriteLine(bHasIdentifier);
                    if (bHasIdentifier == false) {
                        string strErrorMessage = String.Format("Expect the  IDENTIFIER token but a CLOSE token \'>\' , not a valid XML node");
                        throw new ApplicationException(strErrorMessage);
                    }

                    string s = strInputString.Substring(nTokenBeginPosition, nTokenLengthCount);
                    CBaseTokenNode Identifier_node = new CBaseTokenNode(TOKEN_TYPES.IDENTIFIER, s);
                    this.tokens.Add(Identifier_node);
                    return Identifier_node;

                }
                else if (Char.IsWhiteSpace(ch)) {
                    //try_parse_optional_attr();
                    Ignore_space();
                    Ignore_attribute();
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




        public CBaseTokenNode Try_Match_IDENTIFY_TOKEN()
        {
            Ignore_space();
            // remember the current position, inoder to rollback
            int nRollBackPosition = this.nCurrentCharPosition;



            char ch;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier

            while (true)
            {
                ch = Peek_One_Char();
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
                    Ignore_space();
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



        public CBaseTokenNode Try_Match_CHARDATA_Token() {
            Ignore_space();
            int nRollbackPosition = this.nCurrentCharPosition;
            int nCharDataLengthCount = 0;
            int nCharDataBeginPosition = this.nCurrentCharPosition;
            while (true) {
                char ch = Peek_One_Char();

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
                else { 
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





        public void Match_CLOSE_TOKEN() {
            Ignore_space();
            char ch = Peek_One_Char();
            if (ch.Equals('>'))
            {
                CBaseTokenNode close_node = new CBaseTokenNode(TOKEN_TYPES.CLOSE, ">");
                this.tokens.Add(close_node);
            }
            else {
                string strErrorMessage = String.Format("Expect the  CLOSE token but a {0}, not a valid XML node", ch);
                throw new ApplicationException(strErrorMessage);
            }
        }




        public bool Match_SLASH_TOKEN()
        {
            Ignore_space();
            char ch = Peek_One_Char();
            char chOpen = '/';
            //Console.WriteLine("ch is " + ch);
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
                string strErrorMessage = String.Format("Expect the  SLASH token but a {0}, not a valid XML node", ch);
                //throw new ApplicationException(strErrorMessage);
                Console.WriteLine(strErrorMessage);
                return false;
            }
        }


        public void Ignore_attribute() {
            Ignore_space();
            while (true)
            {
                char ch = Peek_One_Char();
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


                if (nCurrentCharPosition == this.nInputLength) {
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
        private bool bAlreadyParseCharData = false;
        private Lexer lexer;
        public Parser(string strInput)
        {
            this.lexer = new Lexer(strInput);
        }

        public CElementNode Element()
        {

            CElementNode element_node = new CElementNode(TOKEN_TYPES.ELEMENT);
            try
            {

                //if (!this.lexer.Match_OPEN_TOKEN()) { return null; }
                this.lexer.Match_OPEN_TOKEN();
                CBaseTokenNode firstIdentifier =this.lexer.Match_IDENTIFY_TOKEN();
                string strFirstIdentifier = firstIdentifier.getText();
                element_node.strList_Identifiers.Add(strFirstIdentifier);

                char ch = this.lexer.Peek_One_Char_at(this.lexer.Get_Current_Char_Position() + 1);

                this.lexer.Match_CLOSE_TOKEN();
                this.lexer.Ignore_space();
                this.lexer.Set_Current_Char_Position(this.lexer.Get_Current_Char_Position() + 1);
                ch = this.lexer.Peek_One_Char();
                int nRecord = 0;
                //peek on char after <person attr="123"> `here`
                this.lexer.Ignore_space();
                if (!ch.Equals('<'))
                {
                    //not parse chardata yet
                    if (bAlreadyParseCharData == false)
                    {
                        CBaseTokenNode CharDataNode = lexer.Try_Match_CHARDATA_Token();
                        if (CharDataNode != null)
                        {
                            bAlreadyParseCharData = true;
                        }
                    }
                    else
                    {
                        //already parse chardata but char data appear, ex:<person>wsafa<data>asdasd
                        string strErrorMessage = String.Format("Expect the </IDENTIFIER> or <IDENTIFIER> token but get a {0}, not a valid XML node", ch);
                        throw new ApplicationException(strErrorMessage);
                    }
                }
                else if (ch.Equals('<') && (this.lexer.Peek_One_Char_at(this.lexer.Get_Current_Char_Position() + 1) != '/'))
                {
                    //<without slash >
                    nRecord = this.lexer.Get_Current_Char_Position();
                    element_node.childNode = Element();
                    this.lexer.Set_Current_Char_Position(this.lexer.Get_Current_Char_Position() + 1);
                }               
                else
                {
                    this.lexer.Ignore_space();
                    string strErrorMessage = String.Format("Unexpected character detected {0}, not a valid XML node", ch);
                    throw new ApplicationException(strErrorMessage);

                }
                this.lexer.Match_OPEN_TOKEN();
                this.lexer.Match_SLASH_TOKEN();
                CBaseTokenNode second_Identifier = this.lexer.Match_IDENTIFY_TOKEN();
                string strSecondIdentifier = second_Identifier.getText();
                element_node.strList_Identifiers.Add(strSecondIdentifier);
                this.lexer.Match_CLOSE_TOKEN();


            }
            catch (Exception ex) {
                if (ex.GetType().FullName == "TryParseFailException")
                {
                    //Try parse fail, do nothing, continue
                    Console.WriteLine("TryParseFailException");
                }
                else
                {
                    //Console print out the exception
                    Console.WriteLine(ex);
                }
            }
            return element_node;
        }
        public List<CBaseTokenNode> get_tokens() { return this.lexer.get_tokens(); }
    }
}