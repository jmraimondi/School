TITLE      (equation.asm)

; This program
; Last update:
; John Michael Raimondi

INCLUDE Irvine32.inc
.data
A	word	20
B	word	12
C	word	36
D	word	52
E	word	?	;to hold (a+b)/(d-c) then answer
F	word	?	;to hold (a-b)/(d+c)
G	word	9
H	word	?	;to hold 9^4
.code
main PROC
mov	eax,G	;9 to eax 9*9*9*9 ((((9+9)9x)9x)9x) 6561 = 9+9 364.5x(times)
add	eax,eax	;18 (9+9)(1x)
add	eax,eax	;36 2x
add	eax,eax	;72	4x save or + c + c
mov	H,eax	;72 to H for later
add	eax,eax	;144 8x
add	eax,eax	;288 16x
add	eax,eax	;576 32x save
mov	ebx,eax	;576 to ebx
add	eax,eax	;1152 64x 
add	eax,eax	;2304 128x
add	eax,eax	;4608 256x  364.5 - 256 = 108.5 - 64 = 44.5 - 32 = 12.5 - 8 = 4.5 - 4 = 0.5 = G
add	eax,ebx ;4608 + 576 = 5184 (256x + 32x = 288x)
add	eax,ebx ;5184 + 576 = 5760 (256x + 64x = 320) 44.5x to go
add	eax,ebx	;5760 + 576 = 6336 (320x + 32x = 352) 12.5x
add	eax,H	;6336 + 72(4x) = 6408
add	eax,H	;6408 + 72(4x) = 6480 (352x + 8x = 360x) 4.5x
add	eax,H	;6480 + 72(4x) = 6552 (360x + 4x = 364x) .5x
add	eax,G	;6552 + 9(1/2x) = 6561(9^4) (364 + .5)
mov	H,eax	;9^4 to H
mov	eax,A	;A to eax
add	eax,B	;A+B
mov	ebx,eax	;copy (A+B) to ebx
add	eax,ebx
add	eax,ebx	;*3
mov	E,eax	;store (A+B)*3
mov	eax,D	;D to eax
sub	eax,C	;D-C
add	eax,eax	;*2
add eax,E
add	eax,H	;1st eq completed
mov E,eax	;hold 1st answer in E
mov	eax,A	;A to eax
sub	eax,B	;A-B
mov ebx,eax	;copy (A-B) to ebx
add	eax,ebx
add	eax,ebx	;*3
mov	F,eax	;store (A+B)*3 to F
mov	eax,D	;D to eax
add	eax,C	;D+C
add	eax,eax	;*2
add	eax,F
sub eax,H	;2nd eq complete
mov	ebx,eax	;2nd answer to ebx
mov eax,E	;1st answer to eax
call DumpRegs
	exit
main ENDP
END main