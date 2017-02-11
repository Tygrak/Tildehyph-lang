using System;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApplication{
    public class Program{
        public static void Main(string[] args){
            string path = Directory.GetCurrentDirectory()+"\\toCompile.txt";
            var inputText = File.ReadAllLines(path);
            tildeProgram prog = new tildeProgram(inputText);
            prog.compileProgram();
            Console.WriteLine("\nHi.");
        }
    }

    public class tildeVar{
        string name = "";
        string tildeValue = "";
        int value = 0;
        public tildeVar(string name, string tildeValue){
            this.name = name;
            this.tildeValue = tildeValue;
        }
    }

    public class tildeProgram{
        public string[] lines;
        public List<tildeVar> variables = new List<tildeVar>();
        private string currentString = "";
        private string lastCommand = "";
        private int readMode = 0;

        private string varName = "";
        private string varVal = "";

        public tildeProgram(string[] lines){
            this.lines = lines;
        }

        public void compileProgram(){
            currentString = "";
            foreach (string line in lines){
                for (int j = 0; j<line.Length; j++){
                    char c = line[j];
                    string character = c.ToString();
                    string nextCharacter = "";
                    if(j+1<line.Length){
                        nextCharacter = line[j+1].ToString();
                    }
                    if(readMode == 0){
                        if(character == "~"){
                            currentString += "~";
                            if(line.Substring(j+1, 2) == "--"){
                                interpretCommand();
                                currentString = "";
                                j += 2;
                                continue;
                            }
                        } else if(character == "-"){
                            if(line.Substring(j, 4) == "----"){
                                j += 3;
                                runCommand();
                                currentString = "";
                                lastCommand = "";
                                continue;
                            }
                            if(nextCharacter == "~"){
                                interpretCommand();
                                currentString = "";
                            }
                            currentString += "-";
                        }
                    } else if(readMode == 1){
                        if(character == "~"){
                            currentString += "~";
                            if(line.Substring(j+1, 3) == "---"){
                                interpretString();
                                j += 3;
                                currentString = "";
                                continue;
                            }
                        } else if(character == "-"){
                            currentString += "-";
                        }
                    }
                }
                Console.Write("\n");
            }
        }

        public void interpretCommand(){
            lastCommand = currentString;
            if(currentString == "~"){
                readMode = 1;
                varName = "";
                varVal = "";
            }
        }

        public void interpretString(){
            if(lastCommand == "~"){
                if(varName == ""){
                    varName = currentString;
                } else if(varVal == ""){
                    varVal = currentString;
                    readMode = 0;
                }
            }
        }

        public void runCommand(){
            if(lastCommand == "~"){
                if(varName != "" && varVal != ""){
                    variables.Add(new tildeVar(varName, varVal));
                    readMode = 0;
                    currentString = "";
                }
            }
        }
    }
}
