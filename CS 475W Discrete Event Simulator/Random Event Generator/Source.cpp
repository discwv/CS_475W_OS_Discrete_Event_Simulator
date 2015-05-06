#include <iostream>
#include <cstdlib>
#include <fstream>
#include <string>
#include <ctime>

using namespace std;

#define SIMILAR_THREADS		0
#define DIFFERENT_THREADS	1

void generateJobs(ofstream& outStream, int numCopies, int MaxCPUBurst, int maxIOBurst, int maxBursts);

int main()
{
	int numJobs, maxJobSize, maxCPUBurstLength, maxIOBurstLength, maxJobRequestTimeSeperation, maxBurstsPerThread;
	string outputFileName;

	cout << ">> outputFileName" << endl
		<< ">> numJobs" << endl
		<< ">> maxJobSize" << endl
		<< ">> maxCPUBurstLength" << endl
		<< ">> maxIOBurstLength" << endl
		<< ">> maxJobRequestTimeSeperation" << endl
		<< ">> maxBurstsPerThread" << endl;

	cin >> outputFileName 
		>> numJobs 
		>> maxJobSize 
		>> maxCPUBurstLength 
		>> maxIOBurstLength 
		>> maxJobRequestTimeSeperation 
		>> maxBurstsPerThread;

	ofstream output(outputFileName);

	output << numJobs << endl;

	int jobStartTime = 0, jobSize = 0, jobType = 0;
	srand(time(NULL));

	while (numJobs-- > 0)
	{
		jobStartTime += (rand() % maxJobRequestTimeSeperation);
		jobSize = (rand() % maxJobSize) + 1;
		jobType = rand() % 2;
		
		output << jobStartTime << "," << jobSize << endl;
		
		if (jobType == SIMILAR_THREADS)
		{
			generateJobs(output, jobSize, maxCPUBurstLength, maxIOBurstLength, maxBurstsPerThread);
		}
		else if (jobType == DIFFERENT_THREADS)
		{
			while (jobSize --> 0)
			{
				generateJobs(output, 1, maxCPUBurstLength, maxIOBurstLength, maxBurstsPerThread);
			}
		}

	}
}

void generateJobs(ofstream& outStream, int numCopies, int MaxCPUBurst, int maxIOBurst, int maxBursts)
{
	int threadBurstNum = (rand() % maxBursts) + 1;
	int* job = new int[threadBurstNum];

	for (int i = 0; i < threadBurstNum; i++)
	{
		if (i % 2 == 0) //Even -> CPU
		{
			job[i] = (rand() % MaxCPUBurst) + 1;
		}
		else			//Odd -> IO
		{
			job[i] = (rand() % maxIOBurst) + 1;
		}
	}
	for (int j = 0; j < numCopies; j++)
	{
		for (int k = 0; k < threadBurstNum; k++)
		{
			outStream << job[k];
			if (k != threadBurstNum - 1)
			{
				outStream << ",";
			}
			else if (k == threadBurstNum - 1)
			{
				outStream << endl;
			}
		}
	}
}