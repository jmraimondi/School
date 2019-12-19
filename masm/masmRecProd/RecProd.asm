TITLE      (calc.asm)

; This program calculates the area of any polygon
; Last update:
; John Michael Raimondi

INCLUDE Irvine32.inc
.data
CR	EQU	0Dh
LF	EQU	0Ah
ESCAPE	EQU	1Bh
SPC	EQU	20h
multOP	EQU	2Ah
addOP	EQU	2Bh
subOP	EQU	2Dh
divOP	EQU	2Fh

fd	dword	?
ofilename	byte	"output.out", 0
ifilename	byte	"input.in",0
bytesRead	dword	?
numbers		dword	4	dup(0)
inputData		byte	80	dup(SPC)
output1		byte	"answer 1 = "
answer1		byte	20	dup(0)
output2		byte	"answer 2 = "
answer2		byte	20	dup(0)

.code
;----------------------------------------------------------------
betterItoA	PROC	;eax has #
	mov	ebx,10
	mov	ecx,0
	mov	edi,[esp+4] ;offset to store
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	add	dl,30h
	inc	ecx
	push	dx	;esp - 2
	jmp	next
done:
	cmp	ecx,0
	je	realDone
	pop	dx
	mov	[edi],dl
	inc	edi
	dec	ecx
	jmp	done
realDone:
	mov	dl,CR
	mov	[edi],dl
	inc	edi
	mov	dl,LF
	mov	[edi],dl
	ret
betterItoA	ENDP
;----------------------------------------------------------------
myWriteToFile	PROC
nextchar:
	mov	al,[edx]
	cmp	al,0
	je	alldone
	mov	eax,fd
	mov	ecx,1
	mov	ebx,edx	;save edx
	call	WriteToFile
	mov	edx,ebx	;restore edx
	inc	edx
	jmp	nextchar
alldone:
	ret
myWriteToFile	ENDP
;----------------------------------------------------------------
bestAtoI	PROC
	push	ebp
	mov	ebp,esp
	push	ecx	;save ecx
	push	esi	;save esi
	push	eax	;save eax
	push	edx
	push	ebx
	mov	ecx,[ebp+8]	;digits count to ecx
	mov	esi,ebp
	add	esi,8
	mov	eax,4	;multiply setup
	mul	ecx	;digits * 4
	add	esi,eax	;move esi to first digit
	mov	eax,0
	mov	edx,10
next:
	cmp	ecx,0
	je	done
	mov	ebx,[esi]
	mul	edx
	mov	edx,10
	sub	bl,30h
	add	eax,ebx
	sub	esi,4	;point at count on finish
	dec	ecx
	jmp	next
done:
	mov	[esi],eax	;replace count with int number
	pop	ebx
	pop	edx
	pop	eax
	pop	esi
	pop	ecx
	pop	ebp
	ret
bestAtoI	ENDP
;----------------------------------------------------------------
recursiveMultiply	PROC
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8]	;big #
	mov	ecx,[ebp+12]	;small #
	cmp	ecx,0
	jne	nextAdd
	pop	ebp
	ret
nextAdd:
	dec	ecx
	push	ecx
	push	eax
	call	recursiveMultiply
	pop	ebx
	pop	eax
	add	eax,ebx
	mov	[ebp+12],eax	;sum to old small
	pop	ebp
	ret
recursiveMultiply	ENDP
;----------------------------------------------------------------
getNumbers	PROC
	push	ebp
	mov	ebp,esp
	mov	ecx,bytesRead
	mov	esi,offset	inputData
	mov	edi,offset	numbers
nextchar:
	cmp	ecx,0
	je	done
	mov	al,[esi]
	dec	ecx
	inc	esi
	cmp	al,'0'
	jl	atoi
	cmp	al,'9'
	jg	atoi
	push	eax
	inc	edx
	jmp	nextchar
atoi:
	mov	bl,[esi-2];look at previous
	cmp	bl,'0'
	jl	nextchar
	cmp	bl,'9'
	jg	nextchar
	push	edx
	call	bestAtoI
	pop	eax
	mov	[edi],eax
	add	edi,4
	mov	edx,0
	jmp	nextchar
done:
	push	edx
	call	bestAtoI
	pop	eax
	mov	[edi],eax
	mov	esp,ebp	;remove junk from stack
	pop	ebp
	ret
getNumbers	ENDP
;----------------------------------------------------------------
main PROC
	mov	edx,offset	ifilename
	call	OpenInputFile	;offset of a filename in edx, returns eax = fd
	push	eax
	mov	edx,offset	inputData
	mov	ecx,80
	call	ReadFromFile
	mov	bytesRead,eax
	pop	eax
	call	CloseFile
	call	getNumbers
	mov	esi,offset	numbers
	mov	eax,[esi]
	add	esi,4
	mov	ebx,[esi]
	cmp	eax,ebx
	jg	swapNums
	push	eax	;smaller first
	push	ebx
	call	recursiveMultiply
	mov	eax,[esp+4]
	push	offset	answer1
	call	betterItoA
	jmp	nextMult
swapNums:
	push	ebx	;smaller first
	push	eax
	call	recursiveMultiply
	mov	eax,[esp+4]
	push	offset	answer1
	call	betterItoA
nextMult:
	add	esi,4
	mov	eax,[esi]
	add	esi,4
	mov	ebx,[esi]
	cmp	eax,ebx
	jg	swapNums2
	push	eax	;smaller first
	push	ebx
	call	recursiveMultiply
	mov	eax,[esp+4]
	push	offset	answer2
	call	betterItoA
	jmp	fin
swapNums2:
	push	ebx	;smaller first
	push	eax
	call	recursiveMultiply
	mov	eax,[esp+4]
	push	offset	answer2
	call	betterItoA
fin:
	mov	edx,offset	ofilename
	call	CreateOutputFile
	mov	fd,eax	;setup write after this
	mov	edx,offset	output1
	call	myWriteToFile
	mov	eax,fd
	mov	edx,offset	output2
	call	myWriteToFile
	mov	eax,fd
	call	CloseFile
	exit
main ENDP
END main