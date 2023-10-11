// REGISTER.C
#include "sysp_com.h"


unsigned  com_address(int port) {

	unsigned base_address;

	// возвращаем -1, если заданный асинхронный порт
	// не COM1, не COM2, не COM3 и не COM4

	if ((port > 4) || (port < 0)) return(-1);

	// считываем из области переменных BIOS базовый адрес данного порта

	base_address = *((unsigned _far*) FP_MAKE(0x40, port * 2));

	return(base_address);
}