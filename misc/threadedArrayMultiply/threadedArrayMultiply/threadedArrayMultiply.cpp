// threadedArrayMultiply.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <thread>

void parallelArray(int *A, int *B, int *C, int n);
void calc(int *row, int *col, int *result, int n);

int main()
{
    return 0;
}

void parallelArray(int *A, int *B, int *C, int n)
{
	std::thread *myThreads = new std::thread[n*n]; //n^2 threads probably too many?
	int tc = 0;
	std::thread *tp = myThreads;

	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < n; i++) 
		{
			*tp = std::thread(calc, (A + (n*i)), (B + (n*j)), C + tc, n);
			++tp;
			++tc;
		}
	}

	tp = myThreads;
	for (int i = 0; i < tc; i++)
	{
		tp->join();
		++tp;
	}
}

void calc(int *row, int *col, int *result, int n)
{
	int tmp = 0;

	for (int i = 0; i < n; i++)
	{
		tmp += (*row * *col);
		++row;
		++col;
	}
	
	*result = tmp;
}