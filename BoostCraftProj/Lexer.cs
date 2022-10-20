using System;



namespace Lexer_and_parser {

    class TryParseFailException : Exception
    {
        public TryParseFailException() { }

        public TryParseFailException(string strMessage)
            : base(strMessage))
        {

        }
    }


    enum TOKEN_TYPES
    {
        OPEN,
        CLOSE,
        OPEN_WITH_SLASH,
        IDENTIFIER
    }



    class cTOKEN_NODE{
        private TOKEN_TYPES Token_type;
        private string text;
        public cTOKEN_NODE( TOKEN_TYPES n, string s) { 
            this.Token_type = n;
            this.text = s;
        }
        public TOKEN_TYPES get_type() { return this.Token_type; }
        public string getText() { return this.text; }
    }


    //================================================================
    //Class Lexer
    //
    //================================================================

    class Lexer {
        private string strInputString;
        private int nCurrentCharPosition;
        private int nInputLength;
        private List<cTOKEN_NODE> tokens;

        //Constructor of Lexer
        public Lexer(string strInputString) { 
            this.strInputString= strInputString;
            //error handling with input length 0 problem.
            if (string.IsNullOrEmpty(this.strInputString))
                throw new ArgumentException("Lexer constructor error, invalid input, string can not be empty or null.", nameof(strInputString));

            this.nCurrentCharPosition= 0;
            this.nInputLength = strInputString.Length;
            this.tokens = new List<cTOKEN_NODE>();
        }


        public List<cTOKEN_NODE> get_tokens() { return this.tokens; }

        public char peek_One_Char() {
            return this.strInputString[nCurrentCharPosition];
        }



        public void match_OPEN_TOKEN() {
            ignore_space();
            char ch = peek_One_Char();
            char chOpen = '<';
            //Console.WriteLine("ch is " + ch);
            //Console.WriteLine(chOpen.Equals(ch));
            if (chOpen.Equals(ch))
            {
                
                nCurrentCharPosition += 1;
                cTOKEN_NODE node = new cTOKEN_NODE(TOKEN_TYPES.OPEN, "<");
                this.tokens.Add(node);
                return;
            }
            else {
                //handle not match error
                string strErrorMessage = String.Format("Expect the  OPEN token but a {0}, not a valid XML node", ch);
                throw new ApplicationException(strErrorMessage);
            }
        }

        public void ignore_space() {
            char ch;
            // space or alphabet or digits
            while(true)
            {
                ch = peek_One_Char();
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


        public void match_IDENTIFY_TOKEN() {
            ignore_space();
            char ch ;
            int nTokenBeginPosition = nCurrentCharPosition;
            int nTokenLengthCount = 0;
            //at least on identifier
            bool bHasIdentifier = false;
            while (true) {
                ch = peek_One_Char();
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
                    cTOKEN_NODE Identifier_node = new cTOKEN_NODE(TOKEN_TYPES.IDENTIFIER, s);
                    this.tokens.Add(Identifier_node);
                    return;

                }
                else if (Char.IsWhiteSpace(ch)) {
                    //try_parse_optional_attr();
                    ignore_space();
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

        public void match_CLOSE_TOKEN() {
            ignore_space();
            char ch = peek_One_Char();
            if (ch.Equals('>'))
            {
                cTOKEN_NODE close_node = new cTOKEN_NODE(TOKEN_TYPES.CLOSE, ">");
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
            try
            {
                this.lexer.match_OPEN_TOKEN();
                this.lexer.match_IDENTIFY_TOKEN();
                this.lexer.match_CLOSE_TOKEN();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        public List<cTOKEN_NODE> get_tokens() { return this.lexer.get_tokens(); }
    }
}