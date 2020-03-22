using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    internal class CharWithEmpty
    {
        private char what;
        private bool Empty;

        public CharWithEmpty(char i)
        {
            what = i;
            Empty = false;
        }
        public CharWithEmpty()
        {
            what = '0';
            Empty = true;
        }
        public override string ToString()
        {
            if (Empty)
            {
                return "epsilon";
            }
            else
            {
                return $"{what}";
            }
        }
    }

    internal class BeginState
    {
        public int internalState { get; set; }
        public BeginState() { internalState = 0; }
        public BeginState(int state)
        {
            internalState = state;
        }
    }

    internal class EndState
    {
        public List<Tuple<int, CharWithEmpty>> internalList { get; set; }

        public EndState() { internalList = new List<Tuple<int, CharWithEmpty>>(); }
        public EndState(int state, CharWithEmpty charWithEmpty)
        {
            internalList = new List<Tuple<int, CharWithEmpty>>();
            internalList.Add(new Tuple<int, CharWithEmpty>(state, charWithEmpty));
        }
        public void Add(int state, CharWithEmpty charWithEmpty)
        {
            internalList.Add(new Tuple<int, CharWithEmpty>(state, charWithEmpty));
        }

        public IEnumerator<Tuple<int, CharWithEmpty>> GetEnumerator(){
            return internalList.GetEnumerator();
        }

        public static EndState operator +(EndState a, EndState b)
        {
            var ret = new EndState();
            ret.internalList = a.internalList;
            foreach(var i in b.internalList)
            {
                ret.internalList.Add(i);
            }
            return ret;
        } 

    }
    internal class StateInStack
    {
        public BeginState Begin { get; set; }
        public EndState End { get; set; }
        public void PointEndTo(int state, in Nfa<CharWithEmpty> nfa)
        {
            PointEndToWithout(state, nfa);
            End.internalList.Clear();
        }
        public void PointEndToWithout(int state, in Nfa<CharWithEmpty> nfa)
        {
            foreach (var (estate, how) in End)
            {
                nfa.SetStateTransition(estate, state, how);
            }
        }
    }
    internal class StackElement
    {
        public StateInStack State { get; set; } = null;
        public CharWithEmpty symbol { get; set; } = null;

        public StackElement(in BeginState begin, in EndState end)
        {
            State = new StateInStack();
            State.Begin = begin;
            State.End = end;
        }
        public StackElement(char sym)
        {
            symbol = new CharWithEmpty( sym);
        }
        public StackElement() {
            State = new StateInStack();
        }
    }
    public class Regex
    {
        public string RegexStr {get;set;}
        static List<char> NormalAlphabet = 
            new List<char> 
            { 
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 
                'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 
                't', 'u', 'v', 'w', 'x', 'y', 'z', 
                '0','1', '2', '3', '4', '5', '6','7', '8', '9',
                '\n'
            };
        static HashSet<char> NormalMap = new HashSet<char>(NormalAlphabet);
        static List<char> OperatorAlphabet = 
            new List<char>
            {
                '*', '?', '+',
                '@', // '@' means concentration
                '|', '(', ')', '\\'
            };
        static HashSet<char> OperatorMap = new HashSet<char>(OperatorAlphabet);
        static List<char> SpecialAlphabet =
            new List<char>
            {
                '.'
            };
        static HashSet<char> SpecialMap = new HashSet<char>(SpecialAlphabet);
        private Dfa dfa;
        private Nfa<CharWithEmpty> nfa;

        public void GenNfaPng(string name)
        {
            nfa.GenPngFile(name);
        }
        public Regex() { }
        public Regex(string regexStr)
        {
            RegexStr = regexStr;
            AddConcertration();
            TurnToRPN();
            GenNfa();
        }
        void AddConcertration()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < RegexStr.Length; ++i)
            {
                if(RegexStr[i] == '@')
                {
                    stringBuilder.Append("\\@");
                }
                else
                {
                    stringBuilder.Append(RegexStr[i]);
                }
            }
            RegexStr = stringBuilder.ToString();
            stringBuilder.Clear();
            bool ifAdd = false;
            int next = 0;
            for(int i = 0; i < RegexStr.Length - 1;)
            {
                if(RegexStr[i] == '\\')
                {
                    next = i + 2;
                }
                else
                {
                    next = i + 1;
                }

                if(RegexStr[i] == '|' || RegexStr[i] == '(')
                {
                    ifAdd = false;
                }
                else if(NormalMap.Contains(RegexStr[next]) 
                    || SpecialMap.Contains(RegexStr[next]))
                {
                    ifAdd = true;
                }
                else if(OperatorMap.Contains(RegexStr[next]))
                {
                    if(RegexStr[next] == '\\' || RegexStr[next] == '(')
                    {
                        ifAdd = true;
                    }
                    else
                    {
                        ifAdd = false;
                    }
                }
                else
                {
                    throw new System.Exception("Regex Expression include unacceptable character");
                }

                for(int j = i; j < next; ++j)
                {
                    stringBuilder.Append(RegexStr[j]);
                }
                if (ifAdd)
                {
                    stringBuilder.Append('@');
                }
                i = next;
            }
            stringBuilder.Append(RegexStr[RegexStr.Length - 1]);
            RegexStr = stringBuilder.ToString();
        }
        public bool match(string target)
        {
            return false;
        }
        void TurnToRPN()
        {
            Dictionary<char, int> OperatorPre = new Dictionary<char, int>();
            OperatorPre.Add('|', 0);
            OperatorPre.Add('@', 1);
            OperatorPre.Add('*', 2);
            OperatorPre.Add('+', 2);
            OperatorPre.Add('?', 2);
            OperatorPre.Add('\\', 3);

            const bool Left = true;
            const bool Right = false;
            const bool None = true;
            Dictionary<char, bool> OperatorAssoci = new Dictionary<char, bool>();
            OperatorAssoci.Add('|', Left);
            OperatorAssoci.Add('@', Left);
            OperatorAssoci.Add('*', Right);
            OperatorAssoci.Add('+', Right);
            OperatorAssoci.Add('?', Right);
            OperatorAssoci.Add('/', None);

            StringBuilder stringBuilder = new StringBuilder();
            Stack<char> operatorStack = new Stack<char>();
            foreach(var i in RegexStr)
            {
                if(NormalMap.Contains(i) || SpecialMap.Contains(i))
                {
                    stringBuilder.Append(i);
                }
                else
                {
                    if(i == '(')
                    {
                        operatorStack.Push(i);
                    }
                    else if (i == ')')
                    {
                        var j = '0';
                        while(operatorStack.Peek() != '(')
                        {
                            j = operatorStack.Pop();
                            stringBuilder.Append(j);
                        }
                        operatorStack.Pop();
                    }
                    else
                    {
                        var j = '0';
                        var PreOfI = OperatorPre.GetValueOrDefault(i);
                        while (operatorStack.Count != 0)
                        {
                            j = operatorStack.Peek();
                            var PreOfJ = OperatorPre.GetValueOrDefault(j);
                            var AssociOfI = OperatorAssoci.GetValueOrDefault(i);
                            if((AssociOfI == Left && PreOfI < PreOfJ) ||
                                (AssociOfI == Right && PreOfI <= PreOfJ))
                            {
                                operatorStack.Pop();
                                stringBuilder.Append(j);
                            }
                            else
                            {
                                break;
                            }
                        }
                        operatorStack.Push(i);
                    }
                }
            }
            while(operatorStack.Count != 0)
            {
                stringBuilder.Append(operatorStack.Pop());
            }
            RegexStr = stringBuilder.ToString();
        }

        private void GenNfa()
        {
            nfa = new Nfa<CharWithEmpty>();
            Stack<StackElement> stack = new Stack<StackElement>();

            Func<StackElement, StackElement> FormStateFromChar = (what) =>
            {
                if(what.symbol == null) { return what; }
                var state = nfa.AddNewState();
                return new StackElement(new BeginState(state),
                    new EndState(state, what.symbol));
            };

            
            foreach(var i in RegexStr)
            {
                if (NormalMap.Contains(i) || SpecialMap.Contains(i))
                {
                    stack.Push(new StackElement(i));
                }
                else
                {
                    switch (i)
                    {
                        case '@':
                            {
                                var right = stack.Pop();
                                var left = stack.Pop();
                                right = FormStateFromChar(right);    
                                var P = new StackElement();
                                if(left.symbol != null)
                                {
                                    var state = nfa.AddNewState();
                                    var bstate = right.State.Begin.internalState;
                                    nfa.SetStateTransition(state, bstate, left.symbol);
                                    P.State.Begin = new BeginState(state);
                                }
                                else
                                {
                                    left.State.PointEndTo(right.State.Begin.internalState, nfa);
                                    P.State.Begin = left.State.Begin;
                                }
                                P.State.End = right.State.End;
                                stack.Push(P);
                            }
                            break;
                        case '|':
                            {
                                var right = stack.Pop();
                                var left = stack.Pop();
                                right = FormStateFromChar(right);
                                left = FormStateFromChar(left);

                                var P = new StackElement();
                                var state = nfa.Merge(right.State.Begin.internalState, 
                                    left.State.Begin.internalState);
                                P.State.Begin = new BeginState(state);
                                P.State.End = left.State.End + right.State.End;
                                stack.Push(P);
                            }
                            break;
                        case '*':
                            {
                                var now = stack.Pop();
                                now = FormStateFromChar(now);
                                var P = new StackElement();
                                now.State.PointEndTo(now.State.Begin.internalState, nfa);
                                now.State.End = new EndState(now.State.Begin.internalState, new CharWithEmpty());
                                stack.Push(now);
                            }
                            break;
                        case '+':
                            {
                                var now = stack.Pop();
                                now = FormStateFromChar(now);
                                now.State.PointEndToWithout(now.State.Begin.internalState, nfa);
                                stack.Push(now);
                            }
                            break;
                        case '?':
                            {
                                var now = stack.Pop();
                                now = FormStateFromChar(now);
                                now.State.End.Add(now.State.Begin.internalState, new CharWithEmpty());
                                stack.Push(now);
                            }
                            break;
                        case '\\':
                            break;
                        default:
                            throw new System.Exception("fuck!");
                    }
                }
            }
            var top = stack.Pop();
            if(stack.Count != 0) { throw new System.Exception("fuck!!"); }
            top = FormStateFromChar(top);
            var success = nfa.AddSuccessState();
            top.State.PointEndTo(success, nfa);
            nfa.BeginState = top.State.Begin.internalState;
        }

    }
}
