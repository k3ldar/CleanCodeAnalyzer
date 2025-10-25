namespace ConsoleApp1
{
    internal class TestErrors
    {
        protected int _someValue = 10;
        public void ThisWillBeAReallyReallyLongLineThatExceedsTheRecommendedMaximumLineLengthOfOneHundredAndTwentyCharactersWhichShouldTriggerAStyleWarningInTheAnalyzer()
        {
            _someValue += 5;
            Console.WriteLine("This is a test."); Console.WriteLine("This is another test.");
        }

        internal void Test()
        {
            int a = _someValue + 1;
            Console.WriteLine("Some Value: ", _someValue);
        }

        internal void ChainedAssignmentTest()
        {
            _someValue = 20;
            int x, y, z;
            x = y = z = 42;
        }

        internal void DocumentCommentAsComment()
        {
            /// this is not a valid documentation comment
            int a = 1;
        }

        internal void IfTestWithoutSpaceAbove()
        {
            int x = 5;
            if (x > 0)
            {
                Console.WriteLine("x is positive");
            }
            if (x == 5)
            {
                Console.WriteLine("x is five");
            }
            if (x < 6)
            {
                Console.WriteLine("x is less than 6");
            }
            x += 3;

            if (x == 8)
            {
                Console.WriteLine("x is now 8");
            }
        }

        internal void BlockWithoutSpaceBelow()
        {
            int x = 5;

            switch (x)
            {
                case 5:
                    Console.WriteLine("x is five");
                    break;
                default:
                    Console.WriteLine("x is something else");
                    break;
            }

        }
    }
}
