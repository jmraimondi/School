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
inputData		byte	80	dup(SPC)

stk			dword	80	dup(?)
			byte	?
outputdata	byte	80	dup(SPC)

.code
;----------------------------------------------------------------
betterAtoI	PROC
	mov	ebx,0
	mov	eax,0
	mov	edx,10
	mov	edi,[esp+4]	;start of number back to edi
nextDigit:
	mov	bl,[edi]	;first char to bl
	mul	edx
	sub	bl,30h
	add	eax,ebx
	inc	edi
	dec	ecx
	cmp	ecx,0
	je	done
	jmp	nextDigit
done:
	call	myPush ;push number onto stack
	ret ;edi ready at next char
betterAtoI	ENDP
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
myPop	PROC
	cmp	esi,stk + sizeof stk
	je	outofhere
	mov	eax,[esi]
	add	esi,4
outofhere:
	ret
myPop	ENDP
;----------------------------------------------------------------
myPush	PROC
	cmp	esi,stk
	je	full
	add	esi,-4
	mov	[esi],eax
full:
	ret
myPush	ENDP
;----------------------------------------------------------------
Parse	PROC
	mov	eax,0
	mov	ebx,0
	mov	ecx,bytesRead
	mov	edx,0
	mov	edi,offset	outputData
	push	edi
	mov	edi,offset	inputData
nextchar:
	cmp	ecx,0
	je	alldone
	mov	al,[edi]
	dec	ecx	;1 byte read
	cmp	al,SPC
	je	skip
	cmp	al,CR
	je	skip
	cmp	al,LF
	je	skip2
	cmp	al,addOP
	je	opadd
	cmp	al,subOP
	je	opsub
	cmp	al,divOP
	je	opdiv
	cmp	al,multOP
	je	opmult
	push	ecx	;save bytes left to read
	push	edi ;save current spot in input
	call	StoreChar
	pop	edi	;input back to edi
	push	edi	;start of number ptr back on stack
	mov	ecx,1
morenums:
	mov	al,[edi+1]	;check next
	cmp	al,'0'
	jl	notnum
	cmp	al,'9'
	jg	notnum
	inc	ecx
	call	StoreChar
	pop	edi	;restore edi
	push	edi	;push start of num back onto stack
	add	edi,ecx	;fix spot
	dec	edi	;fix -1
	jmp	morenums
notnum:
	mov	eax,[esp+4]	;bytes read to eax
	mov	ebx,ecx	;extra digits read to ebx
	add	ebx,-1
	sub	eax,ebx	;fix bytes read to include digits read
	mov	[esp+4],eax ;back onto stack
	call	betterAtoI
	add	esp,4	;remove start of num
	pop	ecx	;number of bytes back to ecx
	jmp	nextchar
skip:
	push	ecx	;store ecx
	push	edi	;store current ptr
	call	storeChar
	pop	edi	;restore current spot
	pop	ecx	;restore bytes left to read
	inc	edi	;next char
	jmp	nextchar
skip2:
	push	ecx
	push	edi
	call	storeChar
	pop	edi
	pop	ecx
	pop	edx	;output to edx
	push	ecx
	push	edi
	push	edx
	call	myPop	;answer to eax
	call	betterItoA
	add	esp,4	;remove old output @
	pop	edx	;current spot to edx
	pop	ecx	;bytes left to read to ecx
	mov	al,CR
	mov	[edi],al
	inc	edi
	mov	al,LF
	mov	[edi],al
	inc	edi
	push	edi	;updated output position
	mov	edi,edx ;current back to edi
	inc	edi	;next char
	jmp	nextchar
opadd:
	push	ecx
	push	edi
	call	storeChar
	call	myPop	;2nd val to eax
	mov	ebx,eax	;2nd val to ebx
	call	myPop	;1st val to eax
	add	eax,ebx	;1st + 2nd
	call	myPush	;push answer onto my stack
	pop	edi
	pop	ecx
	inc	edi	;next char
	jmp	nextchar
opsub:
	push	ecx
	push	edi
	call	storeChar
	call	myPop	;2nd val to eax
	mov	ebx,eax	;2nd to ebx
	call	myPop	;1st to eax
	sub	eax,ebx	;1st - 2nd
	call	myPush	;answer to stack
	pop	edi
	pop	ecx
	inc	edi	;next
	jmp	nextchar
opdiv:
	push	ecx
	push	edi
	call	storeChar
	call	myPop	;2nd to eax
	mov	ebx,eax	;to ebx
	call	myPop	;1st to eax
	mov	edx,0	;prep div
	div	ebx	;eax (1st) / ebx (2nd)
	call	myPush
	pop	edi
	pop	ecx
	inc	edi
	jmp	nextchar
opmult:
	push	ecx
	push	edi
	call	storeChar
	call	myPop
	mov	ebx,eax
	call	myPop
	mul	ebx
	call	myPush
	pop	edi
	pop	ecx
	inc	edi
	jmp	nextchar
alldone:
	pop	edi	;output to edi done with input
	mov	al,CR
	mov	[edi],al
	inc	edi
	mov	al,LF
	mov	[edi],al
	inc	edi
	push	edi
	call	myPop	;answer to eax
	call	betterItoA
	mov	eax,0
	mov	[edi],eax
	ret
Parse	ENDP
;----------------------------------------------------------------
storeChar	PROC
	mov	edi,[esp+12] ;grab output @
	mov	[edi],al ;add character to output
	inc	edi ;bump to next free output
	mov	[esp+12],edi	;updated output to esp+4
	ret
storechar	ENDP
;----------------------------------------------------------------
main PROC
	mov	esi,	offset	stk + sizeof stk
	mov	ecx,0
	mov	eax,0
	mov	ebx,0

	mov	edx,offset	ifilename
	call	OpenInputFile
	push	eax
	mov	edx,offset	inputData
	mov	ecx,80
	call	ReadFromFile
	mov	bytesRead,eax
	pop	eax
	call	CloseFile
	call	Parse
	;work func
	mov	edx,offset	ofilename
	call	CreateOutputFile
	mov	fd,eax	;setup write after this
	mov	edx,offset	outputdata
	call	myWriteToFile
	mov	eax,fd
	call	CloseFile
	exit
main ENDP
END main