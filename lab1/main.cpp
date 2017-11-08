#include <stdio.h>
#include <stdlib.h>

#define bash "\
#/bin/bash \n\
lspci -vmm | grep -w 'Device\nVendor' \n\
"

int main() {
system(bash);
return 0;
}
