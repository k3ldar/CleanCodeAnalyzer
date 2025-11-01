using System.Reflection;

namespace ConsoleApp1
{
    /// <summary>
    /// Class to test errors
    /// </summary>
    internal class TestErrors
    {
        protected const int _constantValue = 42;
        protected readonly string _readonlyString = "Hello, World!";
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



    /// <summary>
    /// Log Levels, defines the type of log entry being made by ILogger interface.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log entry is information only.
        /// </summary>
        Information = 1,

        /// <summary>
        /// Log entry is a warning condition.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Log entry represents an error that has occurred.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Log entry is a critical error within the system.
        /// </summary>
        Critical = 4,

        /// <summary>
        /// Log entry is informing that a plugin module has been successfully loaded.
        /// </summary>
        PluginLoadSuccess = 5,

        /// <summary>
        /// Log entry is informing that a plugin module has failed to load.
        /// </summary>
        PluginLoadFailed = 6,

        /// <summary>
        /// Log entry is informing that a generic error occurred when loading a plugin module.
        /// </summary>
        PluginLoadError = 7,

        /// <summary>
        /// Log entry is informing that there is a configuration error with a plugin module.
        /// </summary>
        PluginConfigureError = 8,

        /// <summary>
        /// Log entry informing that an Ip address has had restriction imposed upon it within the RestrictIp.Plugin module.
        /// </summary>
        IpRestricted = 9,

        /// <summary>
        /// Indicates that an error occurred within CacheControl.Plugin module.
        /// </summary>
        CacheControlError = 10,

        /// <summary>
        /// The event was raised by the ThreadManager
        /// </summary>
        ThreadManager = 11,

        /// <summary>
        /// Indicates that an error occurred when translating a string using Localization.Plugin.
        /// 
        /// This is usually an indication that a localized string is missing.
        /// </summary>
        Localization = 12,
    }

}
