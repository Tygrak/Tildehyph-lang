using System;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApplication{
    public class Program{
        public static void Main(string[] args){
            string path = Directory.GetCurrentDirectory()+"\\toCompile.txt";
            if(args.Length > 0){
                for (int i = 0; i < args.Length; i++){
                    Console.WriteLine(args[i]);
                }
                path = Directory.GetCurrentDirectory()+"\\"+args[0];
            }
            var inputText = File.ReadAllLines(path);
            tildeProgram prog = new tildeProgram(inputText);
            prog.compileProgram();
        }
    }

    public class tildeVar{
        public string name = "";
        private string tildeVal = "";
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
        public List<int[]> loops = new List<int[]>();
        private string currentString = "";
        private string lastCommand = "";
        private int startDepth = 0;
        private int depth = 0;
        private int readMode = 0;
        private int i = 0;
        private int j = 0;
        private List<string> arguments = new List<string>();

        public tildeProgram(string[] lines){
            this.lines = lines;
        }

        public void compileProgram(){
            currentString = "";
            for (i = 0; i<lines.Length; i++){
                string line = lines[i];
                line = line.Split(new string[] {"//"}, StringSplitOptions.None)[0];
                //Console.WriteLine(line);
                for (j = 0; j<line.Length; j++){
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
                            if(j+4 < line.Length && line.Substring(j, 5) == "-----"){
                                j += 4;
                                depth -= 1;
                                if(loops.Count != 0 && loops[loops.Count-1][2] == depth){
                                    i = loops[loops.Count-1][0]-1;
                                    j = loops[loops.Count-1][1];
                                    break;
                                }
                                continue;
                            } else if(j+3 < line.Length && line.Substring(j, 4) == "----"){
                                j += 3;
                                runCommand();
                                currentString = "";
                                lastCommand = "";
                                continue;
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
                    } else if(readMode == 2){ //Go to end of section, (line.Substring(j, 9) == "---------") might need to be changed
                        if(j == 0 && 3 < line.Length && line.Substring(j, 4) == "~~~-"){
                            j += 3;
                            depth+=1;
                            continue;
                        } else if(j+7 < line.Length && line.Substring(j, 8) == "----~~~-"){
                            j += 7;
                            depth+=1;
                            continue;
                        }  else if(5 == line.Length && line.Substring(j, 5) == "-----"){
                            j += 4;
                            depth-=1;
                            if(depth==startDepth){
                                readMode = 0;
                            }
                            continue;
                        } else if(j+8 < line.Length && line.Substring(j, 9) == "---------"){
                            j = line.IndexOf("~", j);
                            if(j == -1){
                                j = line.Length-1;
                            }
                            depth-=1;
                            if(depth==startDepth){
                                readMode = 0;
                            }
                            continue;
                        }
                    }
                }
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
            } else if(currentString == "~~~"){ //If : arg1=mode, arg2=var1, arg3=var2
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~~~"){ //For : arg1=mode, arg2=var1, arg3=var2
                int[] loopPos = {i, j-3, depth, 0};
                if(loops.FindIndex(x => x[0].Equals(i) && x[1].Equals(j-3)) == -1){
                    loops.Add(loopPos);
                }
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
            } else if(lastCommand == "~~~"){
                arguments.Add(currentString);
                if(arguments.Count == 3){
                    readMode = 0;
                }
            } else if(lastCommand == "~~~~"){
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
                    } else if(arg0 == "~~~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v1.value%v2.value;
                    } else if(arg0 == "~~~~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        v1.value = v2.value;
                    } else if(arg0 == "~-~"){
                        v1.value = v1.value+tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~"){
                        v1.value = v1.value-tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~~"){
                        v1.value = v1.value*tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~~~"){
                        v1.value = v1.value/tildeVar.getTildeValue(arguments[2]);
                    } else if(arg0 == "~-~~~~~"){
                        v1.value = v1.value%tildeVar.getTildeValue(arguments[2]);
                    } 
                }
            } else if(lastCommand == "~~~"){
                if(arguments.Count == 3){
                    tildeVar v1 = variables.Find(x => x.name.Equals(arguments[1]));
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value<v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value==v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value>v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~~~"){
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value!=v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~"){
                        if(v1.value<tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~"){
                        int a = tildeVar.getTildeValue(arguments[2]);
                        if(v1.value==tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~~"){
                        if(v1.value>tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~~~"){
                        if(v1.value!=tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    }
                }
            } else if(lastCommand == "~~~~"){
                if(arguments.Count == 3){
                    loops[loops.Count-1][3] += 1;
                    tildeVar v1 = variables.Find(x => x.name.Equals(arguments[1]));
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value -= 1;
                        }
                        v1.value += 1;
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value<v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value += 1;
                        }
                        v1.value -= 1;
                        tildeVar v2 = variables.Find(x => x.name.Equals(arguments[2]));
                        if(v1.value>v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~-~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value -= 1;
                        }
                        v1.value += 1;
                        if(v1.value<tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~-~~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value += 1;
                        }
                        v1.value -= 1;
                        int a = tildeVar.getTildeValue(arguments[2]);
                        if(v1.value>tildeVar.getTildeValue(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
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
