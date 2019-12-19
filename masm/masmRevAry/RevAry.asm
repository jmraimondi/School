TITLE      (RevAry.asm)

; This program recursively reverses and prints an array
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

recary		dword	3,6,9,12,15,18,21,24
output		byte	10	dup(0)

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
	mov	ecx,[ebp+8]	;count
	cmp	ecx,lengthof	recary
	je	noComma
	mov	dl,','
	mov	[edi],dl
	inc	edi
noComma:
	mov	dl,0
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
	mov	bl,0	;overwrite 0's as print goes on
	mov	[edx],bl;^^^^
	inc	edx
	jmp	nextchar
alldone:
	ret
myWriteToFile	ENDP
;----------------------------------------------------------------
recursiveReversePrint	PROC
	push	ebp
	mov	ebp,esp
	mov	ecx,[ebp+8]	;grab length
	cmp	ecx,0
	jne	next
	pop	ebp
	ret
next:
	mov	esi,[ebp+12]	;grab address of current #
	add	esi,4
	dec	ecx
	push	esi
	push	ecx
	call	recursiveReversePrint
	add	esp,4
	pop	esi
	mov	eax,[esi]
	push	offset	output
	call	betterItoA
	pop	edx
	call	myWriteToFile
	pop	ebp
	ret
recursiveReversePrint	ENDP
;----------------------------------------------------------------
main PROC
	mov	edx,offset	ofilename
	call	CreateOutputFile
	mov	fd,eax	;setup write after this
	mov	esi,offset	recary
	sub	esi,4
	push	esi
	mov	ecx,lengthof	recary
	;dec	ecx	;to use 0 instead of 1
	push	ecx
	call	recursiveReversePrint
	mov	eax,fd
	call	CloseFile
	exit
main ENDP
END main