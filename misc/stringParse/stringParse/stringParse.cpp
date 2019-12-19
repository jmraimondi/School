// stringParse.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <string>
#include <vector>

std::vector<std::string> parseCmdLine(std::string inputstring);


int main()
{
	std::vector<std::string> test = parseCmdLine("i just wanted to test this again to make sure it works with a longer string!");


    return 0;
}

std::vector<std::string> parseCmdLine(std::string inputstring)
{
	std::string tmp = "";
	std::vector<std::string> pString;
	for (int i = 0; i < inputstring.length() + 1; i++)
	{
		if (inputstring[i] == ' ' || i == inputstring.length())
		{
			pString.push_back(tmp);
			tmp = "";
		}
		else
		{
			tmp += inputstring[i];
		}
	}
	return pString;
}