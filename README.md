# Finder

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Version](https://img.shields.io/badge/version-1.0.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Language](https://img.shields.io/badge/language-C%23-purple)

🔍 A C# method signature search tool inspired by [Hoogle](https://hoogle.haskell.org) for Haskell. Find methods by their parameter and return types instead of method names.

## 📦 Installation

```bash
git clone https://github.com/DilemaFixer/Finder.git
cd Finder
dotnet build
dotnet run
```

## 🚀 What the project does

CsFinder scans all `.cs` files in a specified folder and searches for methods matching a given signature. This is useful when you remember the parameter types but forgot the method name.

**Supported query formats:**
- `string,int` - methods with string and int parameters
- `string[],int -> (int,string)` - methods with string[] and int parameters returning (int,string) tuple
- `int -> string` - methods with one int parameter returning string

## 💻 Console usage example

```
Fello in CsFinder! Write exit to end 

Write path to folder
>> /Users/user/MyProject

Path successfully set!

Write query
>> string[],int -> (int,string)

File : /Users/user/MyProject/Test.cs 
Position in file : 7:12
public (int, string) Run(string[] args, int i);

>> int -> string

File : /Users/user/MyProject/Test.cs 
Position in file : 12:17
public string Mem(int mem);

File : /Users/user/MyProject/Helper.cs 
Position in file : 25:8
public string Convert(int value);

>> string,int

File : /Users/user/MyProject/Test.cs 
Position in file : 7:12
public (int, string) Run(string[] args, int i);

File : /Users/user/MyProject/Test.cs 
Position in file : 19:12
public int Run1(string[] args, int i);

>> exit
```
