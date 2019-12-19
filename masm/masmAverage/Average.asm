TITLE      (equation.asm)

; This program averages a set of numbers
; Last update:
; John Michael Raimondi

INCLUDE Irvine32.inc
.data
ary	dword	100,-30,25,14,35,-92,82,134,193,99,-24,1,-5,30,35,81,94,143,0
sum	dword	?
.code
main PROC
	mov	esi,offset ary
	mov	eax,[esi]
	mov	ecx,1
	add	esi,4	;loop setup finished
sumloop:
	add	eax,[esi]	;start sum/count loop
	add ecx,1	;bump count
	add	esi,4	;bump ptr
	mov	ebx,[esi]	;lookahead for 0
	cmp	ebx,0
	jne sumloop	;end sum/count loop
	mov	sum,eax	;hold sum
	mov	ebx,0	;division count
Sloop:
	add	ebx,1
	sub	eax,ecx
	jns	Sloop	;jump if not negative
	add	eax,ecx	;fix remainder
	add	ebx,-1	;fix number of divides(avg)
	mov	ecx,eax	;remainder to ecx
	mov	eax,sum	;sum to eax
	call DumpRegs
	exit
main ENDP
END main