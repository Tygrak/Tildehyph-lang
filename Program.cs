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
        }
    }

    public class tildeVar{
        public string name = "";
        public string tildeVal = "";
        private int val = 0;
        public tildeVar(string name, string tildeValue){
            this.name = name;
            this.tildeVal = tildeValue;
            this.val = getTildeValue(tildeValue);
        }
        public int value{
            get{return this.val;}
            set{
                this.val = value;
                this.tildeVal = intToTilde(val);
            }
        }
        public string tildeValue{
            get{return this.tildeVal;}
            set{
                this.tildeVal = value;
                this.val = getTildeValue(tildeVal);
            }
        }

        public static int getTildeValue(string tildeValue){
            tildeValue = tildeValue.Replace("~","1").Replace("-","0");
            return Convert.ToInt32(tildeValue, 2);
        }
        public static string intToTilde(int tildeValue){
            return Convert.ToString(tildeValue, 2).Replace("1","~").Replace("0","-");
        }
    }

    public class tildeProgram{
        public string[] lines;
        public List<tildeVar> variables = new List<tildeVar>();
        private string currentString = "";
        private string lastCommand = "";
        private int readMode = 0;
        private List<string> arguments = new List<string>();

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
                                prepareCommand();
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
                                prepareCommand();
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

        public void prepareCommand(){
            lastCommand = currentString;
            if(currentString == "~"){ //Creates variable : arg1=name, arg2=value
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~"){ //Numeric operation +,-,*,/ : arg1=mode, arg2=var1, arg3=var2/number
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~~~~"){ //Print as int or char : arg1=mode, arg2=var
                readMode = 1;
                arguments.Clear();
            }
        }

        public void interpretString(){
            if(lastCommand == "~"){
                arguments.Add(currentString);
                if(arguments.Count == 2){
                    readMode = 0;
                }
            } else if(lastCommand == "~~"){
                arguments.Add(currentString);
                if(arguments.Count == 3){
                    readMode = 0;
                }
            } else if(lastCommand == "~~~~~"){
                arguments.Add(currentString);
                if(arguments.Count == 2){
                    readMode = 0;
                }
            }
        }

        public void runCommand(){
            if(lastCommand == "~"){
                if(arguments.Count == 2){
                    tildeVar v = variables.Find(x => x.name.Equals(arguments[0]));
                    if(v!=null){
                        v.tildeValue = arguments[1];
                    } else{
                        variables.Add(new tildeVar(arguments[0], arguments[1]));
                    }
                    readMode = 0;
                }
            } else if(lastCommand == "~~"){
                if(arguments.Count == 3){
                    tildeVar v1 = variables.Find(x => x.name.Equals(arguments[1]));
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v1.value+v2.value;
                    } else if(arg0 == "~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v1.value-v2.value;
                    } else if(arg0 == "~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v1.value*v2.value;
                    } else if(arg0 == "~~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v1.value/v2.value;
                    } else if(arg0 == "~-~"){
                        v1.value = v1.value+tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~"){
                        v1.value = v1.value-tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~~"){
                        v1.value = v1.value*tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~~~"){
                        v1.value = v1.value/tildeVar.getTildeValue(arguments[2]);
                    } 
                }
            } else if(lastCommand == "~~~~~"){
                if(arguments.Count == 2){
                    tildeVar v = variables.Find(x => x.name.Equals(arguments[1]));
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        Console.WriteLine(v.value);
                    } else if(arg0 == "~~"){
                        Console.WriteLine((char) v.value);
                    } else if(arg0 == "~~~"){
                        Console.WriteLine(v.tildeValue);
                    } 
                }
            }
        }
    }
}
