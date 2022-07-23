using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConsoleScript : MonoBehaviour
{
    public Text consoleOutput;
    public void InterpretCommand(string command)
    {
        string result = "";
        //Debug.LogWarning(command);
        command = command.ToUpper();
        //Debug.LogWarning(command);
        //if (command == "ad") Debug.LogWarning("EZ");
        if (command.Length < 5)
        {
            if (command == "HELP")
            {
                consoleOutput.text += "Available commands:\n";
                consoleOutput.text += "Clear\n";
                consoleOutput.text += "Show";
            }
        }
        else
        {
            if (command.Substring(0, 5) == "SHOW ")
            {

                int firstArg = -1;
                int secondArg = 0;
                int endOfName = command.IndexOf(' ', 5);
                //Debug.Log("eon " + endOfName);
                if (endOfName == -1)
                {
                    endOfName = command.Length;
                    //Debug.LogWarning("eon == -1");
                }
                else
                {
                    //Debug.LogWarning("eon != -1");
                    // zakres
                    string rest = command.Substring(endOfName + 1);
                    //Debug.LogWarning(rest);
                    int endOfValue = rest.IndexOf(' ');
                    //Debug.LogWarning("eov = " + endOfValue);
                    if (endOfValue == -1)
                    {
                        endOfValue = rest.Length;
                        //Debug.Log("eov == -1 adn then its " + endOfValue);
                    }
                    else
                    {
                        //Debug.Log(secondArg + " parsed from __" + rest.Substring(endOfValue + 1) + "__");
                        try
                        {
                            secondArg = int.Parse(rest.Substring(endOfValue + 1));
                            //Debug.Log(secondArg);
                            //Debug.Log(secondArg + " parsed from __" + rest.Substring(endOfValue + 1) + "__");
                        }
                        catch (FormatException)
                        {
                            result = "'" + secondArg + "' nie jest liczbą.";
                            consoleOutput.text += result;
                            return;
                        }
                    }
                    //try
                    //{
                    //Debug.Log("rest" + rest.Substring(0, endOfValue));
                    if (rest.Substring(0, endOfValue) == "CM") firstArg = MyClub.GetCurrentMatchIndex();
                    else firstArg = int.Parse(rest.Substring(0, endOfValue));
                    //Debug.Log(firstArg);
                    /*}
                    catch (FormatException)
                    {
                        result = "'" + firstArg + "' nie jest liczbą.";
                        consoleOutput.text += result;
                        return;
                    }*/
                }
                // zmienna
                //Debug.Log(command.Substring(5, endOfName - 5));
                switch (command.Substring(5, endOfName - 5))
                {
                    case "MATCHES":
                        //Debug.Log("MATCHES");
                        if (secondArg == 0)
                        {
                            //Debug.Log("sa "+secondArg);
                            int mCount = MyClub.matches.Count;
                            if (Math.Abs(firstArg) > mCount) return;
                            if (firstArg > 0)
                            {
                                //Debug.Log("fa "+firstArg);
                                for (int i = 0; i < firstArg; i++)
                                {
                                    result += MyClub.matches[i].ToString() + "\n";
                                }
                                consoleOutput.text += result;
                            }
                            else
                            {
                                //Debug.Log(mCount + "  " + mCount + firstArg);
                                for (int i = mCount + firstArg; i < mCount; i++)
                                {
                                    result += MyClub.matches[i].ToString() + "\n";
                                }
                                consoleOutput.text += result;
                            }
                        }
                        else
                        {
                            int breakCondition;
                            if (firstArg <= -1)
                            {
                                return;
                            }
                            breakCondition = firstArg + secondArg;
                            int mCount = MyClub.matches.Count;
                            if (breakCondition > mCount) return;
                            if (secondArg > 0)
                            {
                                //Debug.Log("fa "+firstArg);
                                for (int i = firstArg; i < breakCondition; i++)
                                {
                                    result += MyClub.matches[i].ToString() + "\n";
                                }
                                consoleOutput.text += result;
                            }
                            else
                            {
                                //Debug.Log(mCount + "  " + mCount + firstArg);
                                for (int i = breakCondition; i < firstArg; i++)
                                {
                                    result += MyClub.matches[i].ToString() + "\n";
                                }
                                consoleOutput.text += result;
                            }
                        }
                        break;
                    case "CM": consoleOutput.text += MyClub.GetCurrentMatchIndex().ToString(); break;
                }
            }
            else if (command == "CLEAR")
            {
                consoleOutput.text = "";
            }
            else if (command.Substring(0, 5) == "HELP ")
            {
                int endOfName = command.IndexOf(' ', 5);
                if (endOfName == -1) endOfName = command.Length;
                switch (command.Substring(5, endOfName - 5))
                {
                    case "CLEAR": consoleOutput.text += "'Clear' command clears console window\n"; break;
                    case "SHOW":
                        consoleOutput.text += "'Show [variable]' command shows current value of variable\n";
                        consoleOutput.text += "'Show [list] [n]' command shows n elements from a list\n";
                        consoleOutput.text += "'Show [list] [i] [n]' command shows n elements from a list, beginning from the i-th element\n";
                        consoleOutput.text += "You can use 'cm' to get an index of a current match. If n is negative it will show n elements fro mthe end of the list\n";
                        break;
                }
            }
            else if (command.Substring(0, 5) == "SIZE ")
            {
                int endOfName = command.IndexOf(' ', 5);
                if (endOfName == -1) endOfName = command.Length;
                switch (command.Substring(5, endOfName - 5))
                {
                    case "MATCHES": consoleOutput.text += MyClub.matches.Count+"\n"; break;
                }
            }
        }
    }
}
