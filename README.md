# BoostCraftProj
### A XML node validator written in C#
## Demonstration of the execution result 
![alt text](https://github.com/N0nent1ty/BoostCraftProj/blob/master/demo_imgs/result_demo1.PNG)
![alt text](https://github.com/N0nent1ty/BoostCraftProj/blob/master/demo_imgs/result_demo2.PNG)
![alt text](https://github.com/N0nent1ty/BoostCraftProj/blob/master/demo_imgs/result_demo3.PNG)

## How to use it?
- first, you have to make sure there is a floder called "test_inputs" and executable file in same folder.
![alt text](https://github.com/N0nent1ty/BoostCraftProj/blob/master/demo_imgs/how_to_execute1.PNG)
- second, put all the file you want to validate into the "test_inputs" folder, with the extension name ".txt"
![alt text](https://github.com/N0nent1ty/BoostCraftProj/blob/master/demo_imgs/how_to_execute2.PNG)

## Feature
- You once mentioned that, in order to make it simplify, string like
```html
<person gender="female"></person>
```
Should be treated as an invalid one, I still hand out my diffcult-one version that able to deal with these attribute problem,
<person gender="female"></person> can be recognized as valid one in this version.


 - This program even able to point out which part of the XML node cause to error, for instance, the string like 
```html
<person><human><valid>this is a in valid node</invalid></human></person>
```
It will show the message enable user to know how to correct them.
```sh
Symmetry error detected: invalid should be valid
```
## Compilation
You can follow the link below to learn how to set up the ANTLR, a LL(K) parser generator,
https://www.antlr.org/download.html
Once the installation finish, use the following command to compile to .g4 grammar file into C# language.
```sh
antlr4 -Dlanguage=CSharp  -package AntrlXMLParser  -listener -visitor -o .\Generated XMLParser.g4 XMLLexer.g4 
```
Afterward, you can open the sln file with VisualStudio, and compile it.
If you don't want to set-up the ANTLR is also fine, I remain the C# grammar files in the generated floder. 


