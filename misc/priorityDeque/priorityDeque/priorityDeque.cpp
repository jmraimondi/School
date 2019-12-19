// priorityDeque.cpp : Defines the entry point for the console application.
//John Michael Raimondi

#include "stdafx.h"
#include <deque>

template <class T>
class thing
{
	T value;
	int priority;
};

template <class T>
class priorityQueue
{
	void insert(thing<T> x)
	{
		if (container.empty())
		{
			container.push_back(x);
		}
		else
		{
			for (std::deque<thing<T>>::iterator it = container.begin(); it != container.end(); ++it)
			{
				if (x->priority <= *it->priority)
				{
					container.insert(it, x);
					break;
				}
			}
			if (it == container.end())
			{
				container.push_back(x);
			}
		}

	}

	thing<T> top()
	{
		if (!container.empty()) {
			return container.back();
		}
	}
	void pop()
	{
		if (!container.empty()) {
			container.pop_back();
		}
	}
private:
	std::deque<thing<T>> container;
};
int main()
{
    return 0;
}

