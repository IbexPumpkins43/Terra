using Terra.Forms;

namespace Terra
{
    internal static class Program
    {
        // Program data can just be placed in the working directory for debug builds but in a real
        // situation, it really should be placed in local AppData.
#if DEBUG
        public static string DataPath = "C:\\Users\\a\\Desktop\\Projects\\Terra\\Terra\\Data\\";
#else
        public static string DataPath = 
            $"{Environment.GetEnvironmentVariable("userdir")}\\AppData\\Local\\Terra\\";
#endif

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            FormContext context = new();
            context.MainForm = new WelcomeForm(context);

            Application.Run(context);
        }
    }
}