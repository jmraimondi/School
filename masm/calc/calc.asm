TITLE      (calc.asm)

; This program evaluates simple calculations
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

buf	byte ?
outmem	byte	100	DUP(?)
buf2 byte ?
calcmem	dword	3	DUP(?)
.code
;----------------------------------------------------------------
myWriteLine PROC	;prints starting at esi stopping at a 0
Next:
	mov	al,[esi]
	cmp	al,0
	je	lineout
	call	WriteChar
	inc	esi
	jmp	Next
lineout:
	ret
myWriteLine	ENDP
;----------------------------------------------------------------
ItoA	PROC	;eax has #
	mov	ebx,10
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	add	dl,30h
	dec	esi
	mov	[esi],dl
	jmp	next
done:
	ret
ItoA	ENDP
;----------------------------------------------------------------
countDigits	PROC
	mov	ebx,10
	mov	ecx,0
next:
	cmp	eax,0
	je	done
	mov	edx,0
	div	ebx
	inc	ecx
	jmp	next
done:
	ret
countDigits	ENDP
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
myReadLine	PROC
nextread:
	call	ReadChar
	mov	[esi],al
	call	WriteChar
	cmp	al,CR
	je	outofhere
	inc	esi
	jmp	nextread
outofhere:
	ret
myReadLine	ENDP
;----------------------------------------------------------------
jmAtoI	PROC
	mov	ebx,0
	mov	eax,0
nextdigit:
	mov	edx,10
	cmp	edi,esi
	je	alldone
	mov	bl,[edi]
	cmp	bl,'0'
	jl	alldone
	cmp	bl,'9'
	jg	alldone
	mul	edx
	sub	bl,30h
	add	eax,ebx
	inc	edi
	jmp	nextdigit
alldone:
	cmp	eax,0
	jne	store
	ret
store:
	mov	[ecx],eax
	add	ecx,4
	mov	eax,0
	jmp	alldone
jmAtoI	ENDP
;----------------------------------------------------------------
jmReadLine	PROC
nextread:
	call	ReadChar
	mov	[esi],al
	call	WriteChar
	cmp	al,ESCAPE
	je	outofhere
	cmp	al,SPC
	je	jmConvert
	cmp	al,multOP
	je	storeSign
	cmp	al,addOP
	je	storeSign
	cmp	al,subOP
	je	storeSign
	cmp	al,divOP
	je	storeSign
	cmp	al,CR
	je	calc
	inc	esi
	jmp	nextread
outofhere:
	ret
jmConvert:
	call	jmAtoI
	inc	esi
	jmp	nextread
storeSign:
	mov	ecx,offset	calcmem
	add	ecx,4
	mov	[ecx],al
	add	ecx,4
	inc	esi
	mov	edi,esi	;catch edi back up
	jmp	nextread
calc:
	mov	al,LF
	call	WriteChar
	inc	edi
	call	jmAtoI
	mov	ecx,offset	calcmem	;reset ecx ptr
	mov	al,SPC
	mov	[esi],al
	inc	esi
	mov	al,'='
	mov	[esi],al
	inc	esi
	mov	eax,[ecx]	;first number
	mov	dl,[ecx+4]	;op
	mov	ebx,[ecx+8]	;second number
	call	jmCalc
	mov	edi,offset	calcmem
	mov	ebx,0
	mov	[edi],ebx
	mov	[edi+4],ebx
	mov	[edi+8],ebx
	jmp	nextread
jmReadLine	ENDP
;----------------------------------------------------------------
jmCalc	PROC
	cmp	dl,addOP
	je	calcAdd
	cmp	dl,subOP
	je	calcSub
	cmp	dl,multOP
	je	calcMult
	cmp	dl,divOP
	je	calcDiv
done:
	ret
calcAdd:
	add	eax,ebx
	jmp	store
calcSub:
	sub	eax,ebx
	jmp	store
calcMult:
	mul	ebx
	jmp	store
calcDiv:
	mov	edx,0
	div	ebx
	jmp	divStore
store:
	mov	[ecx],eax	;hold answer
	call	countDigits
	mov	al,SPC
	mov	[esi],al
	inc	ecx
	add	esi,ecx
	mov	ebx,offset	calcmem
	mov	eax,[ebx]	;restore answer
	call	ItoA
	dec	ecx
	add	esi,ecx	;esi past answer
	mov	al,CR
	mov	[esi],al
	inc	esi
	mov	al,LF
	mov	[esi],al
	inc	esi
	mov	edi,esi
	jmp done
divStore:
	mov	[ecx],eax	;hold answer
	mov	[ecx+4],edx	;hold remainder
	call	countDigits
	mov	al,SPC
	mov	[esi],al
	inc	ecx
	add	esi,ecx
	mov	ebx,offset	calcmem
	mov	eax,[ebx]	;restore answer
	call	ItoA
	dec	ecx
	add	esi,ecx	;esi past answer
	mov	al,SPC
	mov	[esi],al
	inc	esi
	mov	al,'R'
	mov	[esi],al
	inc	esi
	mov	al,':'
	mov	[esi],al
	inc	esi
	mov	ecx,offset	calcmem
	mov	eax,[ecx+4]	;remainder to eax
	call	countDigits
	mov	al,SPC
	mov	[esi],al
	inc	ecx
	add	esi,ecx
	mov	ebx,offset	calcmem
	mov	eax,[ebx+4]	;remainder to eax again
	call	ItoA
	dec	ecx
	add	esi,ecx
	mov	al,CR
	mov	[esi],al
	inc	esi
	mov	al,LF
	mov	[esi],al
	inc	esi
	mov	edi,esi
	jmp done
jmCalc	ENDP
;----------------------------------------------------------------
main PROC
	mov	ecx,offset calcmem
	mov	esi,offset outmem
	mov	edi,offset outmem
	call	jmReadLine
	mov	al,0
	mov	[esi],al
	mov	edx,offset	ofilename
	call	CreateOutputFile
	mov	fd,eax
	mov	edx,offset	outmem
	call	myWriteToFile
	mov	eax,fd
	call	CloseFile
	exit
main ENDP
END main