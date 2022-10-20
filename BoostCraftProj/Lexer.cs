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
        OPEN_WITH_SLASH,
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

        private List<string> strIdentifiers = new List<string>();
        private List<string> strProperties =new List<string>();
        private CElementNode childNode;

        public CElementNode(TOKEN_TYPES n) : base(n) { 
            
        }





    }


    //================================================================
    //Class Lexer
    //
    //================================================================

    class Lexer {
        private string strInputString;
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



        public void Match_OPEN_TOKEN() {
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
                return;
            }
            else {
                //handle not match error
                string strErrorMessage = String.Format("Expect the  OPEN token but a {0}, not a valid XML node", ch);
                throw new ApplicationException(strErrorMessage);
            }
        }

        public void Ignore_space() {
            char ch;
            // space or alphabet or digits
            while(true)
            {
                ch = Peek_One_Char();
                if (Char.IsWhiteSpace(ch))
                {
                    nCurrentCharPosition++;
                }
                else {
                    break;
                }
                if (this.nCurrentCharPosition == this.nInputLength)
                {
                    throw new ApplicationException("Expect a token but reach EOF, not a valid XML node");
                }
            };

        }


        public CBaseTokenNode Match_IDENTIFY_TOKEN() {
            Ignore_space();
            char ch ;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier
            bool bHasIdentifier = false;
            while (true) {
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
                    Console.WriteLine("Try parse attr");
                
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

        public void Element()
        {

            bool bAlreadyParseCharData=false;
            try
            {
                CElementNode element_node = new CElementNode(TOKEN_TYPES.ELEMENT);


                this.lexer.Match_OPEN_TOKEN();
                CBaseTokenNode firstIdentifier =this.lexer.Match_IDENTIFY_TOKEN();
                string strFirstIdentifier = firstIdentifier.getText();
                this.lexer.Match_CLOSE_TOKEN();
                this.lexer.Ignore_space();
                this.lexer.Set_Current_Char_Position(this.lexer.Get_Current_Char_Position() + 1);
                char ch = this.lexer.Peek_One_Char();

                //peek on char after <person attr="123"> `here`
                if (Char.IsLetterOrDigit(ch))
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
                    return;
                }
                else if (ch.Equals('<')) { 
                    
                
                
                
                }

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
        }
        public List<CBaseTokenNode> get_tokens() { return this.lexer.get_tokens(); }
    }
}