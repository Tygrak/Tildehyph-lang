using System;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApplication{
    public class Program{
        public static void Main(string[] args){
            string path = Directory.GetCurrentDirectory()+"\\vocprosil.~-";
            if(args.Length > 0){
                Console.WriteLine(args[0]);
                path = Directory.GetCurrentDirectory()+"\\"+args[0];
            } else{
                Console.WriteLine("Choose file to run: ");
                path = Directory.GetCurrentDirectory()+"\\"+Console.ReadLine();
            }
            //path = Directory.GetCurrentDirectory()+"\\bottles.~-";
            string[] inputText = File.ReadAllLines(path);
            tildeProgram prog = new tildeProgram(inputText);
            prog.compileProgram();
        }
    }

    public class tildeVar{
        public string name = "";
        public tildeVar(){
        }
        public int value{
            get{return 0;}
            set{}
        }
        public string tildeValue{
            get{return "0";}
            set{}
        }
        public static int tildeToInt(string tildeValue){
            tildeValue = tildeValue.Replace("~","1").Replace("-","0");
            return Convert.ToInt32(tildeValue, 2);
        }
        public static string intToTilde(int value){
            return Convert.ToString(value, 2).Replace("1","~").Replace("0","-");
        }
        public static List<tildeVar> tildeToList(string tildeValue){
            string[] tildes = tildeValue.Split(new string[] {"--"}, StringSplitOptions.None);
            List<tildeVar> tildeList = new List<tildeVar>();
            for (int i = 0; i < tildes.Length; i++){
                tildeList.Add(new tildeInt(null, tildes[i]));
            }
            return tildeList;
        }
        public static List<tildeVar> stringToList(string toConvert){
            List<tildeVar> tildeList = new List<tildeVar>();
            for (int i = 0; i < toConvert.Length; i++){
                tildeList.Add(new tildeInt(null, toConvert[i]));
            }
            return tildeList;
        }
        public static string arrayToTilde(tildeVar[] tildeArray){
            string tildeValue = "";
            for (int i = 0; i < tildeArray.Length; i++){
                tildeVar a = tildeArray[i];
                if(a is tildeInt){
                    tildeValue += tildeArray[i].tildeValue + "--";
                }
            }
            return tildeValue;
        }
    }

    public class tildeInt : tildeVar{
        private string tildeVal = "";
        private int val = 0;
        public tildeInt(string name, string tildeValue){
            this.name = name;
            this.tildeVal = tildeValue;
            this.val = tildeToInt(tildeValue);
        }
        public tildeInt(string name, int value){
            this.name = name;
            this.val = value;
            this.tildeVal = intToTilde(value);
        }
        public new int value{
            get{return this.val;}
            set{
                this.val = value;
                this.tildeVal = intToTilde(val);
            }
        }
        public new string tildeValue{
            get{return this.tildeVal;}
            set{
                this.tildeVal = value;
                this.val = tildeToInt(tildeVal);
            }
        }
    }

    public class tildeList : tildeVar{
        private string tildeVal = "";
        public List<tildeVar> items = new List<tildeVar>();
        public tildeList(string name, string tildeValue){
            this.name = name;
            this.tildeVal = tildeValue;
            this.items = tildeToList(tildeValue);
        }
        public tildeList(string name, List<tildeVar> tildeVars){
            this.name = name;
            this.items = tildeVars;
            this.tildeVal = arrayToTilde(items.ToArray());
        }
        public tildeList(string name){
            this.name = name;
            this.items = new List<tildeVar>();
            this.tildeVal = arrayToTilde(items.ToArray());
        }
        public new int value{
            get{
                int sum = 0;
                for (int i = 0; i < items.Count; i++){
                    tildeVar item = items[i];
                    if(item is tildeInt){
                        sum += ((tildeInt) item).value;
                    } else if(item is tildeList){
                        sum += ((tildeList) item).value;
                    } 
                }
                return sum;
            }
        }
        public new string tildeValue{
            get{return this.tildeVal;}
            set{
                this.tildeVal = value;
                this.items = tildeToList(tildeValue);
            }
        }
        public string text{
            get{
                string read = "";
                for (int i = 0; i < items.Count; i++){
                    if(items[i] is tildeInt){
                        read += (char) ((tildeInt) items[i]).value;
                    }
                }
                return read;
            }
        }
        public tildeVar this[int key]{
            get{return items[key];}
            set{items[key] = value;}
        }
        public void Add(tildeVar var){
            items.Add(var);
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
        private int printMode = 1;
        private int i = 0;
        private int j = 0;
        private int jToSet = 0;
        private List<string> arguments = new List<string>();

        public tildeProgram(string[] lines){
            this.lines = lines;
        }

        public tildeVar findTildeVar(string name){
            tildeVar tilde = variables.Find(x => x.name.Equals(name));
            return tilde;
        }
        public int findTildeVarIndex(string name){
            int id = variables.FindIndex(x => x.name.Equals(name));
            return id;
        }

        public tildeInt findTildeInt(string name){
            tildeVar tilde = variables.Find(x => x.name.Equals(name));
            if(tilde is tildeInt){
                return (tildeInt) tilde;
            } else{
                return null;
            }
        }

        public tildeList findTildeList(string name){
            tildeVar tilde = variables.Find(x => x.name.Equals(name));
            if(tilde is tildeList){
                return (tildeList) tilde;
            } else{
                return null;
            }
        }

        public void compileProgram(){
            currentString = "";
            for (i = 0; i<lines.Length; i++){
                string line = lines[i];
                line = line.Split(new string[] {"//"}, StringSplitOptions.None)[0];
                line = line.Replace(" ", "");
                //Console.WriteLine(line);
                for (j = 0; j<line.Length; j++){
                    if(jToSet>0){
                        j = jToSet;
                        jToSet = 0;
                    }
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
                            if(j+4 < line.Length && lastCommand == "" && line.Substring(j, 5) == "-----"){
                                j += 4;
                                depth -= 1;
                                if(loops.Count != 0 && loops[loops.Count-1][2] == depth){
                                    i = loops[loops.Count-1][0]-1;
                                    jToSet = loops[loops.Count-1][1];
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
                        } else if(j+3 < line.Length && j-4 > -1 && line.Substring(j-4, 8) == "----~~~-"){
                            j += 3;
                            depth+=1;
                            continue;
                        } else if(j == 0 && 4 < line.Length && line.Substring(j, 5) == "~~~~-"){
                            j += 4;
                            depth+=1;
                            continue;
                        } else if(j+4 < line.Length && j-4 > -1 && line.Substring(j-4, 9) == "----~~~~-"){
                            j += 4;
                            depth+=1;
                            continue;
                        } else if(5 == line.Length && line.Substring(j, 5) == "-----"){
                            j += 4;
                            depth-=1;
                            if(depth==startDepth){
                                readMode = 0;
                            }
                            continue;
                        } else if(j+7 < line.Length && line.Substring(j, 8) == "~-------"){
                            j += 7;
                            continue;
                        } else if(j+4 < line.Length && line.Substring(j, 5) == "-----"){
                            j += 4;
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
            } else if(currentString == "~~-~"){ //Creates variable list : arg1=mode, arg2=name, arg3=value
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~"){ //Numeric operation +,-,*,/ : arg1=mode, arg2=var1, arg3=var2/number
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~-~~"){ //Array numeric operation +,-,*,/ : arg1=mode, arg2=var1, arg3=var list, arg4=list index
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
            } else if(currentString == "~~~~~~"){ //Input int, char or tildeValue : arg1=mode, arg2=var
                readMode = 1;
                arguments.Clear();
            } else if(currentString == "~~~~~~~~"){ //Set print mode : arg1=mode
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
            } else if(lastCommand == "~~-~"){
                arguments.Add(currentString);
                if(arguments.Count == 3){
                    readMode = 0;
                }
            } else if(lastCommand == "~~"){
                arguments.Add(currentString);
                if(arguments.Count == 3){
                    readMode = 0;
                }
            } else if(lastCommand == "~~-~~"){
                arguments.Add(currentString);
                if(arguments.Count == 4){
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
            } else if(lastCommand == "~~~~~~"){
                arguments.Add(currentString);
                if(arguments.Count == 2){
                    readMode = 0;
                }
            } else if(lastCommand == "~~~~~~~~"){
                arguments.Add(currentString);
                if(arguments.Count == 1){
                    readMode = 0;
                }
            }
        }

        public void runCommand(){
            if(lastCommand == "~"){
                if(arguments.Count == 2){
                    tildeInt v = findTildeInt(arguments[0]);
                    if(v!=null){
                        v.tildeValue = arguments[1];
                    } else{
                        variables.Add(new tildeInt(arguments[0], arguments[1]));
                    }
                }
            } else if(lastCommand == "~~-~"){
                if(arguments.Count == 3){
                    tildeList v1 = findTildeList(arguments[1]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        if(v1!=null){
                            v1.tildeValue = arguments[2];
                        } else{
                            variables.Add(new tildeList(arguments[1], arguments[2]));
                        }
                    } else if(arg0 == "~-~"){
                        if(v1==null){
                            variables.Add(new tildeList(arguments[1]));
                        }
                    } else if(arg0 == "~~"){
                        tildeVar v2 = findTildeVar(arguments[2]);
                        if(v2 is tildeInt){
                            v1.Add(new tildeInt(((tildeInt)v2).name, ((tildeInt)v2).value));
                        } else if(v2 is tildeList){
                            v1.Add(new tildeList(((tildeList) v2).name, new List<tildeVar>(((tildeList) v2).items)));
                        }
                    } else if(arg0 == "~-~~"){
                        v1.Add(new tildeInt(arguments[1], arguments[2]));
                    } else if(arg0 == "~~~"){
                        tildeVar v2 = findTildeList(arguments[2]);
                        if(v1!=null){
                            v1 = new tildeList(((tildeList) v2).name, new List<tildeVar>(((tildeList) v2).items));
                        } else{
                            variables.Add(new tildeList(arguments[1], new List<tildeVar>(((tildeList) v2).items)));
                        }
                    } else if(arg0 == "~~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                       v1.items.RemoveAt(v2.value);
                    } else if(arg0 == "~-~~~~"){
                       v1.items.RemoveAt(tildeInt.tildeToInt(arguments[2]));
                    }
                }
            } else if(lastCommand == "~~"){
                if(arguments.Count == 3){
                    tildeInt v1 = findTildeInt(arguments[1]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        v1.value = v1.value+v2.value;
                    } else if(arg0 == "~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        v1.value = v1.value-v2.value;
                    } else if(arg0 == "~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        v1.value = v1.value*v2.value;
                    } else if(arg0 == "~~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        v1.value = v1.value/v2.value;
                    } else if(arg0 == "~~~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        v1.value = v1.value%v2.value;
                    } else if(arg0 == "~~~~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1 == null){
                            variables.Add(new tildeInt(arguments[1], v2.tildeValue));
                        } else{
                            v1.value = v2.value;
                        }
                    } else if(arg0 == "~-~"){
                        v1.value = v1.value+tildeInt.tildeToInt(arguments[2]);
                    } else if(arg0 == "~-~~"){
                        v1.value = v1.value-tildeInt.tildeToInt(arguments[2]);
                    } else if(arg0 == "~-~~~"){
                        v1.value = v1.value*tildeInt.tildeToInt(arguments[2]);
                    } else if(arg0 == "~-~~~~"){
                        v1.value = v1.value/tildeInt.tildeToInt(arguments[2]);
                    } else if(arg0 == "~-~~~~~"){
                        v1.value = v1.value%tildeInt.tildeToInt(arguments[2]);
                    } 
                }
            } else if(lastCommand == "~~-~~"){
                if(arguments.Count == 4){
                    tildeInt v1 = findTildeInt(arguments[1]);
                    tildeInt v3 = findTildeInt(arguments[3]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        v1.value = v1.value+((tildeInt) v2[v3.value]).value;
                    } else if(arg0 == "~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        v1.value = v1.value-((tildeInt) v2[v3.value]).value;
                    } else if(arg0 == "~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        v1.value = v1.value*((tildeInt) v2[v3.value]).value;
                    } else if(arg0 == "~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        v1.value = v1.value/((tildeInt) v2[v3.value]).value;
                    } else if(arg0 == "~~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        v1.value = v1.value%((tildeInt) v2[v3.value]).value;
                    } else if(arg0 == "~~~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        if(v1 == null){
                            variables.Add(new tildeInt(arguments[1], ((tildeInt) v2[v3.value]).tildeValue));
                        } else{
                            string name = v1.name;
                            int id1 = findTildeVarIndex(arguments[1]);
                            tildeVar a2 = findTildeList(arguments[2])[v3.value];
                            variables[id1] = a2;
                            variables[id1].name = name;
                        }
                    } else if(arg0 == "~-~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value += v1.value;
                    } else if(arg0 == "~-~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value -= v1.value;
                    } else if(arg0 == "~-~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value *= v1.value;
                    } else if(arg0 == "~-~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value /= v1.value;
                    } else if(arg0 == "~-~~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value %= v1.value;
                    } else if(arg0 == "~-~~~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        ((tildeInt) v2[v3.value]).value = v1.value;
                    }
                }
            } else if(lastCommand == "~~~"){
                if(arguments.Count == 3){
                    tildeInt v1 = findTildeInt(arguments[1]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value<v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value==v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value>v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~~~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value!=v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~"){
                        if(v1.value<tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~"){
                        int a = tildeInt.tildeToInt(arguments[2]);
                        if(v1.value==tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~~"){
                        if(v1.value>tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    } else if(arg0 == "~-~~~~"){
                        if(v1.value!=tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1;
                        }
                    }
                }
            } else if(lastCommand == "~~~~"){
                if(arguments.Count == 3){
                    loops[loops.Count-1][3] += 1;
                    tildeInt v1 = findTildeInt(arguments[1]);
                    if(v1 == null){
                        v1 = new tildeInt(arguments[1], 1);
                        variables.Add(v1);
                    }
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value -= 1;
                        }
                        v1.value += 1;
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value<v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~~"){
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value==v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~~~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value += 1;
                        }
                        v1.value -= 1;
                        tildeInt v2 = findTildeInt(arguments[2]);
                        if(v1.value>v2.value){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~~~~"){
                        tildeList v2 = findTildeList(arguments[2]);
                        int id = loops[loops.Count-1][3];
                        v1.value = v2[id-1].value;
                        if(v2.items.Count > id-1){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~-~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value -= 1;
                        }
                        v1.value += 1;
                        if(v1.value<tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~-~~"){
                        if(v1.value==tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    } else if(arg0 == "~-~~~"){
                        if(loops[loops.Count-1][3] == 1){
                            v1.value += 1;
                        }
                        v1.value -= 1;
                        if(v1.value>tildeInt.tildeToInt(arguments[2])){
                            depth+=1;
                        } else{
                            readMode = 2; startDepth = depth; depth+=1; loops.RemoveAt(loops.Count-1);
                        }
                    }
                }
            } else if(lastCommand == "~~~~~"){
                if(arguments.Count == 2){
                    tildeVar v = findTildeVar(arguments[1]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        if(printMode == 1){
                            if(v is tildeInt){
                                Console.WriteLine(((tildeInt) v).value);
                            } else if(v is tildeList){
                                Console.WriteLine(((tildeList) v).value);
                            }
                        } else if(printMode == 2){
                            if(v is tildeInt){
                                Console.Write(((tildeInt) v).value);
                            } else if(v is tildeList){
                                Console.Write(((tildeList) v).value);
                            }
                        }
                    } else if(arg0 == "~~"){
                        if(printMode == 1){
                            if(v is tildeInt){
                                Console.WriteLine((char) ((tildeInt) v).value);
                            } else if(v is tildeList){
                                Console.WriteLine(((tildeList) v).text);
                            }
                        } else if(printMode == 2){
                            if(v is tildeInt){
                                Console.Write((char) ((tildeInt) v).value);
                            } else if(v is tildeList){
                                Console.Write(((tildeList) v).text);
                            }
                        }
                    } else if(arg0 == "~~~"){
                        if(printMode == 1){
                            if(v is tildeInt){
                                Console.WriteLine(((tildeInt) v).tildeValue);
                            } else if(v is tildeList){
                                Console.WriteLine(((tildeList) v).tildeValue);
                            }
                        } else if(printMode == 2){
                            if(v is tildeInt){
                                Console.Write(((tildeInt) v).tildeValue);
                            } else if(v is tildeList){
                                Console.Write(((tildeList) v).tildeValue);
                            }
                        }
                    } else if(arg0 == "~~~~~"){
                        if(printMode == 1){
                            if(v is tildeInt){
                                if(((tildeInt) v).value > 0){
                                    Console.WriteLine("true");
                                } else if(((tildeInt) v).value == 0){
                                    Console.WriteLine("false");
                                }
                            } else if(v is tildeList){
                                if(((tildeList) v).value > 0){
                                    Console.WriteLine("true");
                                } else if(((tildeList) v).value == 0){
                                    Console.WriteLine("false");
                                }
                            }
                        } else if(printMode == 2){
                            if(v is tildeInt){
                                if(((tildeInt) v).value > 0){
                                    Console.Write("true");
                                } else if(((tildeInt) v).value == 0){
                                    Console.Write("false");
                                }
                            } else if(v is tildeList){
                                if(((tildeList) v).value > 0){
                                    Console.Write("true");
                                } else if(((tildeList) v).value == 0){
                                    Console.Write("false");
                                }
                            }
                        }
                    }
                }
            } else if(lastCommand == "~~~~~~"){
                if(arguments.Count == 2){
                    tildeVar v = findTildeVar(arguments[1]);
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        int inInt;
                        if(int.TryParse(Console.ReadLine(), out inInt)){
                            if(v is tildeInt){
                                tildeInt l = (tildeInt) v;
                                l.value = inInt;
                            } else if(v is tildeList){
                                tildeList l = (tildeList) v;
                                l.Add(new tildeInt(null, tildeInt.intToTilde(inInt)));
                            } else{
                                variables.Add(new tildeInt(arguments[1], tildeInt.intToTilde(inInt)));
                            }
                        }
                    } else if(arg0 == "~~"){
                        char inChar = (char) Console.Read();
                        if(v is tildeInt){
                            tildeInt l = (tildeInt) v;
                            l.value = inChar;
                        } else if(v is tildeList){
                            tildeList l = (tildeList) v;
                            l.Add(new tildeInt(null, tildeInt.intToTilde(inChar)));
                        } else{
                            variables.Add(new tildeInt(arguments[1], tildeInt.intToTilde(inChar)));
                        }
                    } else if(arg0 == "~~~"){
                        string inString = Console.ReadLine();
                        if(v is tildeInt){
                            tildeInt l = (tildeInt) v;
                            l.tildeValue = inString;
                        } else if(v is tildeList){
                            tildeList l = (tildeList) v;
                            l.Add(new tildeInt(null, inString));
                        } else{
                            variables.Add(new tildeInt(arguments[1], inString));
                        }
                    } else if(arg0 == "~~~~"){
                        string inString = Console.ReadLine();
                        if(v is tildeList){
                            tildeList l = (tildeList) v;
                            l = new tildeList(arguments[1], tildeList.stringToList(inString));
                        } else{
                            variables.Add(new tildeList(arguments[1], tildeList.stringToList(inString)));
                        }
                    } else if(arg0 == "~~~~~"){
                        string inString = Console.ReadLine();
                        int a = inString == "true" ? 1 : inString == "false" ? 0 : -1;
                        if(v is tildeInt){
                            tildeInt l = (tildeInt) v;
                            l.value = a;
                        } else if(v is tildeList){
                            tildeList l = (tildeList) v;
                            l.Add(new tildeInt(null, a));
                        } else{
                            variables.Add(new tildeInt(arguments[1], a));
                        }
                    }
                }
            } else if(lastCommand == "~~~~~~~~"){
                if(arguments.Count == 1){
                    string arg0 = arguments[0];
                    if(arg0 == "~"){
                        printMode = 1;
                    } else if(arg0 == "~~"){
                        printMode = 2;
                    }
                }
            }
        }
    }
}
