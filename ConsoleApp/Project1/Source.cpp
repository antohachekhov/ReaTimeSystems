// REGISTER.C
#include "sysp_com.h"


unsigned  com_address(int port) {

	unsigned base_address;

	// ���������� -1, ���� �������� ����������� ����
	// �� COM1, �� COM2, �� COM3 � �� COM4

	if ((port > 4) || (port < 0)) return(-1);

	// ��������� �� ������� ���������� BIOS ������� ����� ������� �����

	base_address = *((unsigned _far*) FP_MAKE(0x40, port * 2));

	return(base_address);
}