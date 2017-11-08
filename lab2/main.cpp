#include <stdio.h>
#include <stdlib.h>

#define bash "\
#/bin/bash \n\
sudo hdparm -I /dev/sda5 | grep -w 'Model\nSerial Number\nFirmware\nTransport:\nPIO:\nDMA:' \n\
df -hT / \
"

int main() {
system(bash);
return 0;
}