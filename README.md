# Tildehyph-lang
The programming languages using only tildes and hyphens.


Latest windows 7 build: https://drive.google.com/open?id=0B5j70jka51uTVkN2bFVNdnkydUE

Latest windows 10 build: https://drive.google.com/open?id=0B5j70jka51uTQ0NBZzRRN3ZWWXM

# Basic info
You can run the interpreter (Tildehyph-lang) and then enter a file to open or enter a file as an arg. It's is recommended to open the interpreter in cmd.
Your code can contain only tildes and hyphens, it can also contain whitespace to make the code more readable. Comments are one line only started by "//".
Values are entered as binary numbers using ~ as a 1 and - as a 0, these are called tildeValues. For example ~ = 1, ~~ = 3, ~-~ = 5. Values cant begin or end with a hyphen, 
so when you want to have a variable with the value 2 you have to first set it to 1 and then add 1 to it. Ifs and fors are ended by five hyphens.
## Command syntax
A command begins with the command name followed by two hyphens. After that there is a number of arguments seperated by three hyphens.
The command is ended by 4 hyphens. For example `~--~---~-------` sets variable "~" to value 1.

# Cheatsheet
| Command  | Arguments | Description  |
|---|---|---|
| ~ | 2 (name, value) | Create a new tildeInt variable. |
| ~~-~ | 3 (mode, name, value) | TildeList operation. |
| ~~ | 3 (mode, var1, number/var2) | Mathematic operation. |
| \~~-\~~ | 4 (mode, tildeInt, tildeList, list index) | TildeList mathematic operation. |
| ~~~ | 3 (mode, var1, number/var2) | If. |
| ~~~~ | 3 (mode, var1, number/var2) | For/While. |
| ~-~~~~ | 0 | Continue. |
| ~~~~~ | 2 (mode, var) | Print. |
| ~~~~~~ | 2 (mode, var) | Input. |
| ~-~~~~~~ | 3 (mode, var, file name) | Input from file. |
| ~~~~~~~~ | 1 (mode) | Set print mode. |

# Command explanation
## ~
Creates a tildeInt variable with the chosen name and value. If the variable already exists instead set it to the value.
## ~~
Mathematical operation.  
**~** : Sets var1 to var1+var2.  
**~-~** : Sets var1 to var1+number.  
**~~** : Sets var1 to var1-var2.  
**~-~~** : Sets var1 to var1-number.  
**~~~** : Sets var1 to var1\*var2.  
**~~-~~~** : Sets var1 to its power of var2.  
**~-~~~** : Sets var1 to var1\*number.  
**~-~~-~~~** : Sets var1 to its power of number.  
**~~~~** : Sets var1 to var1/var2.  
**~~-~~~~** : Sets var1 to its square root. The additional argument is useless but cant be omitted.  
**~-~~~~** : Sets var1 to var1/number.  
**~~~~~** : Sets var1 to var1%var2.  
**~-~~~~~** : Sets var1 to var1%number.  
**~~~~~~** : Copies var2 to var1.  
## ~~~
If statement.  
**~** : If var1\<var2.  
**~-~** : If var1\<number.  
**~~** : If var1==var2.  
**~-~~** : If var1==number.  
**~~~** : If var1>var2.  
**~-~~~** : If var1>number.  
**~~~~** : If var1!=var2.  
**~-~~~~** : If var1!=number.  
## ~~~~
For/while loop. Repeats block until the condition isnt met.  
**~** : For as long as var1\<var2 increment var1 by 1.  
**~-~** : For as long as var1\<number increment var1 by 1.  
**~~** : While var1==var2.  
**~-~~** : While var1==number.  
**~~~** : For as long as var1>var2 decrease var1 by 1.  
**~-~~~** : For as long as var1>var2 decrease var1 by 1.  
**~~~~** : While var1!=var2.  
**~-~~~~** : While var1!=number.  
**~~~~~** : For each item in list var2, set var1 to the item.  
## ~-~~~~
Continue. Immediately stops and returns to beginning of the loop.  
## ~~~~~
Print variable.  
**~** : Print variable as int, if the variable is a list print the sum of the items.  
**~~** : Print variable as char, if the variable is a list print the list items as a string.  
**~~~** : Print variable as a tildeValue.  
**~~~~~** : Prints true if var is higher or equal to 1, prints false if var is equal to 0.  
**~~~~~~** : Waits for 0.1 * variable value, then clear the console.  
## ~~~~~~
Input into variable from stdin.  
**~** : Inputs a int from stdin, if the variable is a list add the int to the list.  
**~~** : Inputs the value of a char from stdin, if the variable is a list add the char to the list.  
**~~~** : Inputs a tildeValue from stdin, if the variable is a list add the tildeValue to the list.  
**~~~~** : Inputs values of characters from a string from stdin to a list.  
**~~~~~** : Inputs a boolean from stdin to a tildeInt or tildeList, if the input is the string "true" or a 1 set the value to 1, if the input is the string "false" or a 0 set the value to 0.  
## ~-~~~~~~
Input into variable from a textfile. The file has to end with .txt and has to have a name made from tildes and hyphens.  
**~** : Inputs a int from a textfile, if the variable is a list add the int to the list.  
**~~** : Inputs values of characters from a textfile to a list.  
**~~~** : Inputs a tildeValue from a textfile, if the variable is a list add the tildeValue to the list.  
## ~~-~
TildeList operation.  
**~** : Creates list with items initialized from argument 2. The initialization values have to be seperated by two hyphens. If the list already exists instead set it to the items.  
**~-~** : Creates a blank list. The additional argument is useless but cant be omitted.  
**~~** : Add var2 into the list.  
**~-~~** : Add a number into the list.  
**~~~** : Copies items from list var2 into the first list.  
**~~~~** : Removes from list at index equal to var2.  
**~-~~~~** : Removes from list at index equal to the arg2 number.  
**~~~~~** : Sets tildeInt with name arg2 to the count of items in list arg1.  
## \~~-\~~
TildeList mathematical operation.  
**~** : Sets var1 to var1+list[index].  
**~-~** : Sets list[index] to list[index]+var1.  
**~~** : Sets var1 to var1-list[index].  
**~-~~** : Sets list[index] to list[index]-var1.  
**~~~** : Sets var1 to var1\*list[index].  
**~~-~~~** : Sets var1 to its power of list[index].  
**~-~~~** : Sets list[index] to list[index]\*var1.  
**~-~~-~~~** : Sets list[index] to its power of var1.  
**~~~~** : Sets var1 to var1/list[index].  
**~-~~~~** : Sets list[index] to list[index]/var1.  
**~-~~-~~~~** : Sets list[index] to its square root. The var1 argument is useless but cant be omitted.  
**~~~~~** : Sets var1 to var1%list[index].  
**~-~~~~~** : Sets list[index] to list[index]%var1.  
**~~~~~~** : Copies list[index] into var1.  
**~-~~~~~~** : Copies var1 into list[index]. 
**~~~~~~~** : If list[index] exists set var1 to 1 else set it to 0.
## ~~~~~~~~
Sets print mode.  
**~** : Print now prints on seperate lines. This is the default mode.  
**~~** : Print now doesnt use newlines.  

# Comments
Some commands create the variable for you if it doesnt exist - for example when copying a variable with `~~--~~~~~~` or with a ~~~~ loop.  
Using a negative index for a list selects items startting at the end of the list.  
The interpreter can detect some errors for you but it is far from perfect.  