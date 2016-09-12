using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotlightImages
{
    public class UiDrawer
    {
        public UiDrawer()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        public string DrawFolderDialog(string destinationFolder)
        {
            Console.WriteLine("Saving spotlight images in: \"" + destinationFolder + "\"");

            var isInputYes = this.DrawYesNoQuestion("Would you like to change the folder? ");

            // todo: validate input
            if (isInputYes)
            {
                Console.Write("Please enter new location path: ");
                return Console.ReadLine().Replace('/', '\\');
            }

            return string.Empty;
        }

        public void DrawResult(int copiedLandscapesCount, int existingLandscapesCount, int copiedPortraitsCount, int existingPortraitsCount)
        {
            Console.WriteLine("┌───────────────────────────┬────────────────────────┐");
            Console.WriteLine("│New landscape images :{0} │ Already existing :{1} │", copiedLandscapesCount.ToString().PadLeft(4), existingLandscapesCount.ToString().PadLeft(4));
            Console.WriteLine("├───────────────────────────┼────────────────────────┤");
            Console.WriteLine("│New portrait images  :{0} │ Already existing :{1} │", copiedPortraitsCount.ToString().PadLeft(4), existingPortraitsCount.ToString().PadLeft(4));
            Console.WriteLine("└───────────────────────────┴────────────────────────┘");
        }

        public bool DrawOpenFolderDialog()
        {
            return this.DrawYesNoQuestion("Would you like to open the folder? ");
        }

        private bool DrawYesNoQuestion(string question)
        {
            var selected = false;
            var isYes = false;

            while (!selected)
            {
                if (isYes)
                {
                    Console.WriteLine(string.Format("┌─────┐     ").PadLeft(question.Length + 13));
                    Console.WriteLine(question + " │ Yes │  No ");
                    Console.WriteLine(string.Format("└─────┘     ").PadLeft(question.Length + 13));

                }
                else
                {
                    Console.WriteLine(string.Format("┌──────┐").PadLeft(question.Length + 13));
                    Console.WriteLine(question + " Yes │  No  │");
                    Console.WriteLine(string.Format("└──────┘").PadLeft(question.Length + 13));
                }

                var key = Console.ReadKey();
                if ((isYes && key.Key == ConsoleKey.RightArrow) || (!isYes && key.Key == ConsoleKey.LeftArrow))
                {
                    isYes = !isYes;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    selected = true;
                    continue;
                }

                Console.SetCursorPosition(0, Console.CursorTop - 3);
            }

            return isYes;
        }
    }
}
