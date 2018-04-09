#include <windows.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include "wavread.h"

#pragma warning(disable:4996)

/* wavread�v������ֲ */
double * wavread(char* filename, int *fs, int *Nbit, int *waveLength)
{
	FILE *fp;
	char dataCheck[5]; // �٤�����
	unsigned char forIntNumber[4];
	double tmp, signBias, zeroLine;
	int quantizationByte;
	double *waveForm;
	int i;
	dataCheck[4] = '\0'; // �������պϤΤ��ᣬ����˽K�����֤����룮
//	fp = fopen(filename, "rb");
	fp = fopen(filename, "rb");
	if(NULL == fp) 
	{
		printf("�ե�����Υ�`�ɤ�ʧ����%s\n", filename);
		return NULL;
	}
	//�إå��Υ����å�
	fread(dataCheck, sizeof(char), 4, fp); // "RIFF"
	if(0 != strcmp(dataCheck,"RIFF"))
	{
		fclose(fp);
		printf("�إå�RIFF������\n");
		return NULL;
	}
	fseek(fp, 4, SEEK_CUR); // 4�Х����w�Ф�
	fread(dataCheck, sizeof(char), 4, fp); // "WAVE"
	if(0 != strcmp(dataCheck,"WAVE"))
	{
		fclose(fp);
		printf("�إå�WAVE������\n");
		return NULL;
	}
	fread(dataCheck, sizeof(char), 4, fp); // "fmt "
	if(0 != strcmp(dataCheck,"fmt "))
	{
		fclose(fp);
		printf("�إå�fmt ������\n");
		return NULL;
	}
	fread(dataCheck, sizeof(char), 4, fp); //1 0 0 0
	if(!(16 == dataCheck[0] && 0 == dataCheck[1] && 0 == dataCheck[2] && 0 == dataCheck[3]))
	{
		fclose(fp);
		printf("�إå�fmt (2)������\n");
		return NULL;
	}
	fread(dataCheck, sizeof(char), 2, fp); //1 0
	if(!(1 == dataCheck[0] && 0 == dataCheck[1]))
	{
		fclose(fp);
		printf("�ե��`�ޥå�ID������\n");
		return NULL;
	}
	fread(dataCheck, sizeof(char), 2, fp); //1 0
	if(!(1 == dataCheck[0] && 0 == dataCheck[1]))
	{
		fclose(fp);
		printf("���ƥ쥪�ˤό��ꤷ�Ƥ��ޤ���\n");
		return NULL;
	}

	// ����ץ���ܲ���
	fread(forIntNumber, sizeof(char), 4, fp);
	*fs = 0;
	for(i = 3;i >= 0;i--)
	{
		*fs = *fs*256 + forIntNumber[i];
	}
	// ���ӻ��ӥå���
	fseek(fp, 6, SEEK_CUR); // 6�Х����w�Ф�
	fread(forIntNumber, sizeof(char), 2, fp);
	*Nbit = forIntNumber[0];
	// �إå�
	fread(dataCheck, sizeof(char), 4, fp); // "data"
	if(0 != strcmp(dataCheck,"data"))
	{
		fclose(fp);
		printf("�إå�data������\n");
		return NULL;
	}
	// ����ץ�����
	fread(forIntNumber, sizeof(char), 4, fp); // "data"
	*waveLength = 0;
	for(i = 3;i >= 0;i--)
	{
		*waveLength = *waveLength*256 + forIntNumber[i];
	}
	*waveLength /= (*Nbit/8);

	// ���Τ�ȡ�����
	waveForm = (double *)malloc(sizeof(double) * *waveLength);
	if(waveForm == NULL) return NULL;

	quantizationByte = *Nbit/8;
	zeroLine = pow(2.0,*Nbit-1);
	for(i = 0;i < *waveLength;i++)
	{
		signBias = 0.0;
		tmp = 0.0;
		fread(forIntNumber, sizeof(char), quantizationByte, fp); // "data"
		// ���Ťδ_�J
		if(forIntNumber[quantizationByte-1] >= 128)
		{
			signBias = pow(2.0,*Nbit-1);
			forIntNumber[quantizationByte-1] = forIntNumber[quantizationByte-1] & 0x7F;
		}
		// �ǩ`�����i���z��
		for(int j = quantizationByte-1;j >= 0;j--)
		{
			tmp = tmp*256.0 + (double)(forIntNumber[j]);
		}
		waveForm[i] = (double)((tmp - signBias) / zeroLine);
	}
	// �ɹ�
	fclose(fp);
	return waveForm;
}

